using AutoMapper;
using FollowUP.Core.Domain;
using FollowUP.Core.Repositories;
using FollowUP.Infrastructure.DTO;
using FollowUP.Infrastructure.Exceptions;
using FollowUP.Infrastructure.Extensions;
using InstagramApiSharp.API;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.SessionHandlers;
using InstagramApiSharp.Logger;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Services
{
    public class InstagramAccountService : IInstagramAccountService
    {

        private readonly IUserRepository _userRepository;
        private readonly IInstagramAccountRepository _instagramAccountRepository;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;

        public InstagramAccountService(IUserRepository userRepository,
                IInstagramAccountRepository instagramAccountRepository,
                IMapper mapper, IMemoryCache cache)
        {
            _userRepository = userRepository;
            _instagramAccountRepository = instagramAccountRepository;
            _mapper = mapper;
            _cache = cache;
        }
        public async Task CreateAsync(Guid Id, Guid userId, string username, string password)
        {
            var instagramAccountTest = await _instagramAccountRepository.GetAsync(userId);
            if(instagramAccountTest != null)
            {
                throw new ServiceException(ErrorCodes.AccountAlreadyExists, "The account is already being used by You or another person.");
            }

            var user = await _userRepository.GetAsync(userId);
            if (user == null)
            {
                throw new ServiceException(ErrorCodes.UserNotFound, "Cannot find desired user (not authenticated?)");
            }
            var instagramAccount = new InstagramAccount(Id, user, username, password);
            await _instagramAccountRepository.AddAsync(instagramAccount);
        }

        public async Task<IEnumerable<AccountDto>> GetAllByUserId(Guid userId)
        {
            var accounts = await _instagramAccountRepository.GetUsersAccountsAsync(userId);
            return _mapper.Map<IEnumerable<InstagramAccount>, IEnumerable<AccountDto>>(accounts);
        }

        public async Task LoginAsync(string username, string password, string phoneNumber, string twoFactorCode,
                                     string verificationCode, bool preferSMSVerification, bool replayChallenge)
        {
            // Get proper instagram account by given username
            var account = await _instagramAccountRepository.GetAsync(username);

            if(account == null)
            {
                throw new ServiceException(ErrorCodes.AccountDoesntExist, "Can't login to account that hasn't been created.");
            }

            // Create user credentials
            var userSession = new UserSessionData
            {
                UserName = username,
                Password = password
            };

            // Create new instance of InstaApi with given credentials, setting request delay and session handler for user
            var instaApi =  InstaApiBuilder.CreateBuilder()
                                        .SetUser(userSession)
                                        .UseLogger(new DebugLogger(LogLevel.Exceptions))
                                        .SetRequestDelay(RequestDelay.FromSeconds(0, 1))
                                        .SetSessionHandler(new FileSessionHandler() { FilePath = account.FilePath })
                                        .Build();

            // Check if there is an instaApi instance bound to the account...
            var instaApiCache = (IInstaApi)_cache.Get(account.Id);

            if(instaApiCache != null)
            {
                // ...if true, use it
                instaApi = instaApiCache;
            }
            
            // Get appropriate directories of the folder and file
            var fullPath = instaApi.SessionHandler.FilePath.Split(@"\");
            var directory = $@"{fullPath[0]}\{fullPath[1]}";
            
            // Create directory if it doesn't exist yet
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Create file if it doesn't exist yet
            if (!File.Exists(instaApi.SessionHandler.FilePath))
            {
                using (FileStream fs = File.Create(instaApi.SessionHandler.FilePath))
                {
                    Console.WriteLine($"Created file for {username}.");
                }
            }

            // Try logging in from session
            try
            {
                instaApi?.SessionHandler?.Load();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            // If user isn't authenticated (the session didn't work out)
            if (!instaApi.IsUserAuthenticated)
            {
                // Log in the user (only, if there is no two factor code)
                var logInResult = twoFactorCode.Empty() ? verificationCode.Empty()
                    ? await instaApi.LoginAsync() : new Result<InstaLoginResult>(false, InstaLoginResult.ChallengeRequired)
                    : new Result<InstaLoginResult>(false, InstaLoginResult.TwoFactorRequired);

                // If the login succeeded
                if (logInResult.Succeeded)
                {
                    instaApi.SaveSession();

                    // Set the authentiaction step to authenticated
                    await SetAuthenticationStepAndSaveAccount(AuthenticationStep.Authenticated, account);
                    return;
                }
                // If the login failed
                else
                {
                    // If the challenge is required
                    if (logInResult.Value == InstaLoginResult.ChallengeRequired)
                    {
                        // Start the challenge response
                        var challenge = await instaApi.GetChallengeRequireVerifyMethodAsync();

                        // If it succeeded
                        if (challenge.Succeeded)
                        {
                            // If the phone number submit is required
                            if (challenge.Value.SubmitPhoneRequired)
                            {
                                // Try submitting the phone number
                                try
                                {
                                    await SubmitPhone(instaApi, account, phoneNumber);
                                }
                                catch (Exception ex) { Console.WriteLine(ex.Message); }
                            }
                            else
                            {
                                // If we have valid email or phone number
                                if (challenge.Value.StepData != null && verificationCode.Empty())
                                {
                                    // If the user prefers verification via SMS
                                    if(preferSMSVerification)
                                    {
                                        if (!challenge.Value.StepData.PhoneNumber.Empty())
                                        {
                                            await RequestFromSMS(instaApi, account, verificationCode, replayChallenge);
                                        }
                                        else if(!challenge.Value.StepData.Email.Empty())
                                        {
                                            await RequestFromEmail(instaApi, account, verificationCode, replayChallenge);
                                        }
                                        else
                                        {
                                            await SetAuthenticationStepAndSaveAccount(AuthenticationStep.ChallengeRequired, account);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        if (!challenge.Value.StepData.Email.Empty())
                                        {
                                            await RequestFromEmail(instaApi, account, verificationCode, replayChallenge);
                                        }
                                        else if (!challenge.Value.StepData.PhoneNumber.Empty())
                                        {
                                            await RequestFromSMS(instaApi, account, verificationCode, replayChallenge);
                                        }
                                        else
                                        {
                                            await SetAuthenticationStepAndSaveAccount(AuthenticationStep.ChallengeRequired, account);
                                            return;
                                        }
                                    }
                                }
                                // If we have verification code
                                else if(!verificationCode.Empty())
                                {
                                    // Verify if the code is correct
                                    var verifyCodeLogin = await instaApi.VerifyCodeForChallengeRequireAsync(verificationCode);

                                    // If the login succeeded...
                                    if (verifyCodeLogin.Succeeded)
                                    {
                                        instaApi.SaveSession();

                                        // Remove unnecessary instaApi instance
                                        _cache.Remove(account.Id);
                                        await SetAuthenticationStepAndSaveAccount(AuthenticationStep.Authenticated, account);
                                        return;
                                    }
                                    else if (verifyCodeLogin.Info.Message == "Two factor required.")
                                    {
                                        await SetAuthenticationStepAndSaveAccount(AuthenticationStep.TwoFactorRequired, account);
                                        return;
                                    }
                                }
                                else
                                {
                                    await SetAuthenticationStepAndSaveAccount(AuthenticationStep.ChallengeRequired, account);
                                    return;
                                }
                            }
                        }
                        else
                        {
                            await SetAuthenticationStepAndSaveAccount(AuthenticationStep.ChallengeRequired, account);
                            return;
                        }
                    }
                    // If the user has two factor authentication enabled
                    else if (logInResult.Value == InstaLoginResult.TwoFactorRequired)
                    {
                        // Authenticate with two factor code
                        await TwoFactorLogin(instaApi, account, twoFactorCode);
                    }
                    // Every other error during logging in
                    else
                    {
                        Console.WriteLine($"Unable to login: {logInResult.Info.Message}");
                        await SetAuthenticationStepAndSaveAccount(AuthenticationStep.NotAuthenticated, account);
                        return;
                    }
                    return;
                }
            }
            account.SetAuthenticationStep(AuthenticationStep.Authenticated);
            await _instagramAccountRepository.UpdateAsync(account);
            return;
        }

        public async Task SetAuthenticationStepAndSaveAccount(AuthenticationStep step, InstagramAccount account)
        {
            // Set account's authentication step
            account.SetAuthenticationStep(step);

            // Update the account
            await _instagramAccountRepository.UpdateAsync(account);
        }

        public async Task RequestFromSMS(IInstaApi instaApi, InstagramAccount account, string verificationCode, bool replayChallenge = false)
        {
            var requestPhoneVerify = await instaApi.RequestVerifyCodeToSMSForChallengeRequireAsync(replayChallenge);
            if (requestPhoneVerify.Succeeded)
            {
                _cache.Set(account.Id, instaApi);
                await SetAuthenticationStepAndSaveAccount(AuthenticationStep.NeedCodeVerify, account);
                return;
            }
            else
            {
                await SetAuthenticationStepAndSaveAccount(AuthenticationStep.ChallengeRequired, account);
                return;
            }
        }

        public async Task RequestFromEmail(IInstaApi instaApi, InstagramAccount account, string verificationCode, bool replayChallenge = false)
        {
            var requestEmailVerify = await instaApi.RequestVerifyCodeToEmailForChallengeRequireAsync(replayChallenge);
            if (requestEmailVerify.Succeeded)
            {
                _cache.Set(account.Id, instaApi);
                await SetAuthenticationStepAndSaveAccount(AuthenticationStep.NeedCodeVerify, account);
                return;
            }
            else
            {
                await SetAuthenticationStepAndSaveAccount(AuthenticationStep.ChallengeRequired, account);
                return;
            }
        }

        public async Task TwoFactorLogin(IInstaApi instaApi, InstagramAccount account, string twoFactorCode)
        {
            // If this is the first time and we don't have the two factor code
            if (twoFactorCode.Empty())
            {
                // Remember the instaApi instance
                _cache.Set(account.Id, instaApi);
                await SetAuthenticationStepAndSaveAccount(AuthenticationStep.TwoFactorRequired, account);
                return;
            }

            // Two factor authentication
            var twoFactorLogin = await instaApi.TwoFactorLoginAsync(twoFactorCode);

            if (twoFactorLogin.Succeeded)
            {
                instaApi.SaveSession();

                await SetAuthenticationStepAndSaveAccount(AuthenticationStep.Authenticated, account);
                _cache.Remove(account.Id);
                return;
            }
            else
            {
                Console.WriteLine(twoFactorLogin.Info.Message);
                await SetAuthenticationStepAndSaveAccount(AuthenticationStep.TwoFactorRequired, account);
                return;
            }
        }

        public async Task SubmitPhone(IInstaApi instaApi, InstagramAccount account, string phoneNumber)
        {
            // Return phone required if it's empty
            if (phoneNumber.Empty())
            {
                await SetAuthenticationStepAndSaveAccount(AuthenticationStep.PhoneRequired, account);
                return;
            }

            // Add the + if necessary
            if (!phoneNumber.StartsWith("+"))
                phoneNumber = $"+{phoneNumber}";

            var submitPhone = await instaApi.SubmitPhoneNumberForChallengeRequireAsync(phoneNumber);
            if (submitPhone.Succeeded)
            {
                var verifyPhoneAfterAdded = await instaApi.RequestVerifyCodeToSMSForChallengeRequireAsync();

                if (verifyPhoneAfterAdded.Succeeded)
                {
                    await SetAuthenticationStepAndSaveAccount(AuthenticationStep.NeedCodeVerify, account);
                    return;
                }
                else
                {
                    await SetAuthenticationStepAndSaveAccount(AuthenticationStep.ChallengeRequired, account);
                    return;
                }
            }
            else
            {
                Console.WriteLine(submitPhone.Info.Message);
                await SetAuthenticationStepAndSaveAccount(AuthenticationStep.PhoneRequired, account);
                return;
            }
        }
    }
}
