using AutoMapper;
using FollowUP.Core.Domain;
using FollowUP.Core.Repositories;
using FollowUP.Infrastructure.DTO;
using FollowUP.Infrastructure.Exceptions;
using FollowUP.Infrastructure.Extensions;
using FollowUP.Infrastructure.Settings;
using InstagramApiSharp;
using InstagramApiSharp.API;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Android.DeviceInfo;
using InstagramApiSharp.Classes.SessionHandlers;
using InstagramApiSharp.Enums;
using InstagramApiSharp.Helpers;
using InstagramApiSharp.Logger;
using Microsoft.Extensions.Caching.Memory;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Services
{
    public class InstagramAccountService : IInstagramAccountService
    {

        private readonly IUserRepository _userRepository;
        private readonly IInstagramAccountRepository _instagramAccountRepository;
        private readonly IProxyRepository _proxyRepository;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _cache;
        private readonly PromotionSettings _settings;

        public InstagramAccountService(IUserRepository userRepository,
                IInstagramAccountRepository instagramAccountRepository, IProxyRepository proxyRepository,
                IMapper mapper, IMemoryCache cache, PromotionSettings settings)
        {
            _userRepository = userRepository;
            _instagramAccountRepository = instagramAccountRepository;
            _proxyRepository = proxyRepository;
            _mapper = mapper;
            _cache = cache;
            _settings = settings;
        }

        /// <summary>
        /// Create Instagram account to store it in the database for further authentication
        /// </summary>
        /// <param name="Id">ID of the account to create</param>
        /// <param name="userId">ID of the user that owns the account</param>
        /// <param name="username">Login to the Instagram account to create</param>
        /// <param name="password">Password to the Instagram account to create</param>
        /// <returns></returns>
        public async Task CreateAsync(Guid Id, Guid userId, string username, string password)
        {
            // Check if the user is registered and logged in
            var user = await _userRepository.GetAsync(userId);
            if (user == null)
                throw new ServiceException(ErrorCodes.UserNotFound, "Cannot find desired user (not authenticated?)");

            // Check if the account is already created, if yes, don't create a new one
            // (as instagram accounts are unique and two users can't have the same one)
            var instagramAccountTest = await _instagramAccountRepository.GetAsync(username);
            if (instagramAccountTest != null)
            {
                if (instagramAccountTest.UserId != userId)
                    throw new ServiceException(ErrorCodes.AccountAlreadyExists, "Account is already taken by another user");
                else
                    return;
            }


            var proxies = await _proxyRepository.GetAllAsync();
            InstagramProxy instaProxy = null;
            foreach (var proxy in proxies)
            {
                if (proxy.ExpiryDate.ToUniversalTime() < DateTime.UtcNow || proxy.IsTaken)
                    continue;

                instaProxy = proxy;
                proxy.IsTaken = true;
                await _proxyRepository.UpdateAsync(proxy);
                break;
            }

            if (instaProxy == null)
                throw new ServiceException(ErrorCodes.NoProxyAvailable, "There is no proxy available for account creation.");

            // If the given account doesn't exist, create one and save it to the database
            var instagramAccount = new InstagramAccount(Id, user, username, password);
            await _instagramAccountRepository.AddAsync(instagramAccount);

            // Create account's settings and store it in the database
            var accountSettings = new AccountSettings(Guid.NewGuid(), instagramAccount.Id);
            await _instagramAccountRepository.AddAccountSettingsAsync(accountSettings);

            // Give the account proper proxy
            var accountProxy = new AccountProxy(Guid.NewGuid(), instaProxy.Id, instagramAccount.Id);
            await _proxyRepository.AddAccountsProxyAsync(accountProxy);
        }

        /// <summary>
        /// Gets all the accounts for given user ID
        /// </summary>
        /// <param name="userId">ID of the user that owns the accounts</param>
        /// <returns>List of Accounts that the user owns</returns>
        public async Task<IEnumerable<AccountDto>> GetAllByUserId(Guid userId)
        {
            // Get the accounts from repository
            var accounts = await _instagramAccountRepository.GetUsersAccountsAsync(userId);
            // Map the accounts and return them as a list of AccountDTO
            return _mapper.Map<IEnumerable<InstagramAccount>, IEnumerable<AccountDto>>(accounts);
        }

        /// <summary>
        /// Attempt to login to the given Instagram account, save it's <see cref="AuthenticationStep"/>
        /// and save the account's state to a file
        /// </summary>
        /// <param name="username">Login to the Instagram account to log in to</param>
        /// <param name="password">Password to the Instagram account to log in to</param>
        /// <param name="phoneNumber">Phone number associated with the account for further verification</param>
        /// <param name="twoFactorCode">Two-factor-authentication code to provide if 2FA enabled</param>
        /// <param name="verificationCode">Verification code received trough email or SMS</param>
        /// <param name="preferSMSVerification">Do the user prefer verification via SMS</param>
        /// <param name="replayChallenge">Determines if the user wants to resend the code to email or SMS</param>
        /// <returns></returns>
        public async Task LoginAsync(string username, string password, string phoneNumber, string twoFactorCode,
                                     string verificationCode, bool preferSMSVerification, bool replayChallenge)
        {
            // Get proper instagram account by given username
            var account = await _instagramAccountRepository.GetAsync(username);

            if(account == null)
                throw new ServiceException(ErrorCodes.AccountDoesntExist, "Can't login to account that hasn't been created.");

            // Set the user's credentials
            var userSession = new UserSessionData
            {
                UserName = username,
                Password = password
            };

            var instaPath = account.FilePath;
            instaPath = instaPath.Replace('\\', Path.DirectorySeparatorChar);
            instaPath = instaPath.Replace('/', Path.DirectorySeparatorChar);

            var accountProxy = await _proxyRepository.GetAccountsProxyAsync(account.Id);

            if (accountProxy == null)
            {
                Console.WriteLine($"User {account.Username} doesn't have any working proxy, skipping...");
                await Task.Delay(5000);
                return;
            }

            var proxyInfo = await _proxyRepository.GetAsync(accountProxy.ProxyId);

            if (proxyInfo.ExpiryDate < DateTime.UtcNow)
            {
                Console.WriteLine($"Proxy {proxyInfo.Ip} for user {account.Username} is expired, skipping...");
                await Task.Delay(5000);
                return;
            }

            var proxy = new InstaProxy(proxyInfo.Ip, proxyInfo.Port)
            {
                Credentials = new NetworkCredential(proxyInfo.Username, proxyInfo.Password)
            };

            // Now create a client handler which uses that proxy
            var httpClientHandler = new HttpClientHandler()
            {
                Proxy = proxy,
            };

            // Create new instance of InstaApi with given credentials, setting request delay and session handler for user
            var instaApi =  InstaApiBuilder.CreateBuilder()
                                        .SetUser(userSession)
                                        .UseLogger(new DebugLogger(InstagramApiSharp.Logger.LogLevel.Exceptions))
                                        .SetRequestDelay(RequestDelay.FromSeconds(0, 1))
                                        .SetSessionHandler(new FileSessionHandler() { FilePath = instaPath })
                                        .UseHttpClientHandler(httpClientHandler)
                                        .Build();

            // Check if there is an instaApi instance bound to the account...
            var instaApiCache = (IInstaApi)_cache.Get(account.Id);

            if(instaApiCache != null)
            {
                // ...if true, use it
                instaApi = instaApiCache;
            }

            instaApi.SetApiVersion(InstaApiVersionType.Version117);
            instaApi.SetDevice(AndroidDeviceGenerator.GetByName(AndroidDevices.XIAOMI_REDMI_NOTE_4X));

            // Get appropriate directories of the folder and file
            var fullPath = instaPath.Split(Path.DirectorySeparatorChar);
            var directory = Path.Combine(fullPath[0], fullPath[1]);
            
            // Create directory if it doesn't exist yet
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

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
            if (instaApi.IsUserAuthenticated)
            {
                account.SetAuthenticationStep(AuthenticationStep.Authenticated);
                await _instagramAccountRepository.UpdateAsync(account);
                await SendMockupRequests(instaApi);
                return;
            }
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
                await SendMockupRequests(instaApi);
                return;
            }
            // If the challenge is required
            if (logInResult.Value != InstaLoginResult.ChallengeRequired)
            {
                // If the user has two factor authentication enabled
                if (logInResult.Value == InstaLoginResult.TwoFactorRequired)
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
            }

            // Start the challenge response
            var challenge = await instaApi.GetChallengeRequireVerifyMethodAsync();

            // If it succeeded
            if (!challenge.Succeeded)
            {
                await SetAuthenticationStepAndSaveAccount(AuthenticationStep.ChallengeRequired, account);
                return;
            }

            // If the phone number submit is required
            if (challenge.Value.SubmitPhoneRequired)
            {
                // Try submitting the phone number
                try
                {
                    await SubmitPhone(instaApi, account, phoneNumber);
                    return;
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }
            }
            else
            {
                // If we have valid email or phone number
                if (challenge.Value.StepData != null && verificationCode.Empty())
                {
                    // If the user prefers verification via SMS
                    if (preferSMSVerification)
                    {
                        if (!challenge.Value.StepData.PhoneNumber.Empty())
                        {
                            await RequestFromSMS(instaApi, account, replayChallenge);
                        }
                        else if (!challenge.Value.StepData.Email.Empty())
                        {
                            await RequestFromEmail(instaApi, account, replayChallenge);
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
                            await RequestFromEmail(instaApi, account, replayChallenge);
                        }
                        else if (!challenge.Value.StepData.PhoneNumber.Empty())
                        {
                            await RequestFromSMS(instaApi, account, replayChallenge);
                        }
                        else
                        {
                            await SetAuthenticationStepAndSaveAccount(AuthenticationStep.ChallengeRequired, account);
                            return;
                        }
                    }
                }
                // If we have verification code
                else if (!verificationCode.Empty())
                {
                    // Verify if the code is correct
                    var verifyCodeLogin = await instaApi.VerifyCodeForChallengeRequireAsync(verificationCode);

                    // If the login succeeded...
                    if (verifyCodeLogin.Succeeded)
                    {
                        instaApi.SaveSession();
                        await SetAuthenticationStepAndSaveAccount(AuthenticationStep.Authenticated, account);
                        await SendMockupRequests(instaApi);
                        return;
                    }
                    else if (verifyCodeLogin.Value == InstaLoginResult.TwoFactorRequired)
                    {
                        await SetAuthenticationStepAndSaveAccount(AuthenticationStep.TwoFactorRequired, account);
                        return;
                    }
                    else if (verifyCodeLogin.Value == InstaLoginResult.ChallengeRequired)
                    {
                        var infoResponse = await instaApi.GetLoggedInChallengeDataInfoAsync();
                        var acceptRepsonse = await instaApi.AcceptChallengeAsync();
                        if (acceptRepsonse.Succeeded)
                            await LoginAsync(username, password, phoneNumber, twoFactorCode,
                            verificationCode, preferSMSVerification, replayChallenge);
                    }
                }
                else
                {
                    await SetAuthenticationStepAndSaveAccount(AuthenticationStep.ChallengeRequired, account);
                    return;
                }
            }

            return;
        }

        private async Task SendMockupRequests(IInstaApi instaApi)
        {
            var test = await instaApi.FeedProcessor.GetUserTimelineFeedAsync(PaginationParameters.MaxPagesToLoad(1));
            await instaApi.FeedProcessor.GetExploreFeedAsync(PaginationParameters.MaxPagesToLoad(1));
            await instaApi.UserProcessor.GetCurrentUserAsync();
            await instaApi.FeedProcessor.GetRecentActivityFeedAsync(PaginationParameters.MaxPagesToLoad(1));
            await instaApi.MessagingProcessor.GetDirectInboxAsync(PaginationParameters.MaxPagesToLoad(1));
            await instaApi.StoryProcessor.GetStoryFeedAsync();
        }

        public async Task LoginToEmbeddedBrowserAsync(string username, string password, string twoFactorCode, string verificationCode)
        {
            var account = await _instagramAccountRepository.GetAsync(username);

            if (account == null)
                throw new ServiceException(ErrorCodes.AccountDoesntExist, "Can't login to account that hasn't been created.");

            var accountProxy = await _proxyRepository.GetAccountsProxyAsync(account.Id);
            var proxyInfo = await _proxyRepository.GetAsync(accountProxy.ProxyId);

            if (accountProxy == null)
            {
                Console.WriteLine($"User {account.Username} doesn't have any working proxy, skipping...");
                await Task.Delay(5000);
                return;
            }
            else if (proxyInfo.ExpiryDate < DateTime.UtcNow)
            {
                Console.WriteLine($"Proxy {proxyInfo.Ip} for user {account.Username} is expired, skipping...");
                await Task.Delay(5000);
                return;
            }

            ChromeOptions chromeOptions = new ChromeOptions();
            var chromeProxy = new Proxy
            {
                Kind = ProxyKind.Manual,
                IsAutoDetect = false,
                SocksUserName = proxyInfo.Username,
                SocksPassword = proxyInfo.Password,
                HttpProxy = $"http://{proxyInfo.Ip}:{proxyInfo.Port}"
            };
            var cookiesPath = Path.Combine(Directory.GetCurrentDirectory(), "ebaccounts", account.Id.ToString()); 
            chromeOptions.Proxy = chromeProxy;
            chromeOptions.AddArgument("user-data-dir=" + cookiesPath);
            if(_settings.HeadlessBrowser)
                chromeOptions.AddArgument("--headless");
            chromeOptions.AddArgument("--lang=en");
            chromeOptions.AddArgument("ignore-certificate-errors");
            using (var chromeDriver = new ChromeDriver(".", chromeOptions))
            {
                string browserKey = $"{account.Id}-browser";
                var cacheCookies = (ReadOnlyCollection<OpenQA.Selenium.Cookie>)_cache.Get(browserKey);

                string loginUrl = "https://www.instagram.com";
                chromeDriver.Navigate().GoToUrl(loginUrl);

                if (cacheCookies != null)
                {
                    foreach (var cookie in cacheCookies)
                    {
                        chromeDriver.Manage().Cookies.AddCookie(cookie);
                    }
                    chromeDriver.Navigate().GoToUrl(loginUrl);
                }

                // Check if the explore button-svg is present on the page
                // to prevent throwing errors - if not, continue logging
                try
                {
                    var exploreSvg = chromeDriver.FindElementByCssSelector("svg");
                    _cache.Set(browserKey, chromeDriver.Manage().Cookies.AllCookies);
                    chromeDriver.Close();
                    return;
                }
                catch
                {
                    Console.WriteLine($"Logging user {username}...");
                }

                try
                {
                    var buttons = chromeDriver.FindElementsByCssSelector("button");
                    await Task.Delay(250);
                    if (buttons.Count >= 2)
                    {
                        if (buttons[2].Text == "Switch accounts")
                            buttons[2].Click();
                    }
                }
                catch
                {
                    Console.WriteLine($"Swtiching accounts not needed");
                }

                try
                {
                    var switchToLoginButton = chromeDriver.FindElementByCssSelector("a[href=\"/accounts/login/?source=auth_switcher\"]");
                    if (switchToLoginButton.Text == "Log in")
                        switchToLoginButton.Click();
                }
                catch
                {
                    Console.WriteLine($"Couldn't find login button");
                }
                

                await Task.Delay(250);

                var inputs = chromeDriver.FindElementsByCssSelector("input");
                inputs[0].SendKeys(username);
                inputs[1].SendKeys(password);

                await Task.Delay(500);

                var loginButton = chromeDriver.FindElementByCssSelector("button[type=\"submit\"]");
                loginButton.Click();

                _cache.Set($"{browserKey}-sessionid", chromeDriver.SessionId);
                _cache.Set(browserKey, chromeDriver.Manage().Cookies.AllCookies);
                await Task.Delay(3000);
                var codeInput = chromeDriver.FindElementByCssSelector("input");
                if (codeInput != null)
                {
                    if (verificationCode.Empty())
                    {
                        _cache.Set(browserKey, chromeDriver.Manage().Cookies.AllCookies);
                        chromeDriver.Close();
                        return;
                    }
                    else
                    {
                        try
                        {
                            codeInput.SendKeys(verificationCode);
                        }
                        catch
                        {
                            codeInput = chromeDriver.FindElementByCssSelector("input");
                            codeInput.SendKeys(verificationCode);
                        }
                        finally
                        {
                            await Task.Delay(500);
                            var verificationButton = chromeDriver.FindElementByCssSelector("button");
                            verificationButton.Click();
                            await Task.Delay(5000);
                            _cache.Set(browserKey, chromeDriver.Manage().Cookies.AllCookies);
                        }
                        _cache.Set(browserKey, chromeDriver.Manage().Cookies.AllCookies);
                        chromeDriver.Close();
                        return;
                    }

                }
            }
        }

        /// <summary>
        /// Sets account's authentication step and saves it to the repository
        /// </summary>
        /// <param name="step">Authentication step of the account to set</param>
        /// <param name="account">The account whose <see cref="AuthenticationStep"/> we want to set</param>
        /// <returns></returns>
        public async Task SetAuthenticationStepAndSaveAccount(AuthenticationStep step, InstagramAccount account)
        {
            // Set account's authentication step
            account.SetAuthenticationStep(step);

            // Update the account
            await _instagramAccountRepository.UpdateAsync(account);
        }

        /// <summary>
        /// Request to send verification code via SMS
        /// </summary>
        /// <param name="instaApi">The instance of <see cref="IInstaApi"/> to use</param>
        /// <param name="account">The Instagram account to authenticate</param>
        /// <param name="replayChallenge">Whether to resend the code</param>
        /// <returns></returns>
        public async Task RequestFromSMS(IInstaApi instaApi, InstagramAccount account, bool replayChallenge = false)
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

        /// <summary>
        /// Request to send verification code via Email
        /// </summary>
        /// <param name="instaApi">The instance of <see cref="IInstaApi"/> to use</param>
        /// <param name="account">The Instagram account to authenticate</param>
        /// <param name="replayChallenge">Whether to resend the code</param>
        /// <returns></returns>
        public async Task RequestFromEmail(IInstaApi instaApi, InstagramAccount account, bool replayChallenge = false)
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

        /// <summary>
        /// Attempt to login account via Two Factor Authentication
        /// </summary>
        /// <param name="instaApi">The instance of <see cref="IInstaApi"/> to use</param>
        /// <param name="account">The Instagram account to authenticate</param>
        /// <param name="twoFactorCode">The 2FA code received via SMS</param>
        /// <returns></returns>
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
                await SendMockupRequests(instaApi);
                return;
            }
            else
            {
                Console.WriteLine(twoFactorLogin.Info.Message);
                await SetAuthenticationStepAndSaveAccount(AuthenticationStep.TwoFactorRequired, account);
                return;
            }
        }

        /// <summary>
        /// Submit phone number for further verification if it hasn't been set yet
        /// </summary>
        /// <param name="instaApi">The instance of <see cref="IInstaApi"/> to use</param>
        /// <param name="account">The Instagram account to add the number to</param>
        /// <param name="phoneNumber">The phone number that we want to add to the account</param>
        /// <returns></returns>
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

        public async Task BuyComments(Guid accountId, double daysToAdd)
        {
            var account = await _instagramAccountRepository.GetAsync(accountId);

            if (account == null)
                throw new ServiceException(ErrorCodes.AccountDoesntExist, $"Cannot find account with given id: {accountId}.");

            if (daysToAdd <= 0)
                throw new ServiceException(ErrorCodes.DaysNotPositive, "Days to add must be a positive number");

            account.BuyComments(daysToAdd);

            await _instagramAccountRepository.UpdateAsync(account);
        }

        public async Task BuyPromotions(Guid accountId, double daysToAdd)
        {
            var account = await _instagramAccountRepository.GetAsync(accountId);

            if (account == null)
                throw new ServiceException(ErrorCodes.AccountDoesntExist, $"Cannot find account with given id: {accountId}.");

            if (daysToAdd <= 0)
                throw new ServiceException(ErrorCodes.DaysNotPositive, "Days to add must be a positive number.");

            account.BuyPromotions(daysToAdd);

            await _instagramAccountRepository.UpdateAsync(account);
        }
    }
}
