using FollowUP.Core.Domain;
using FollowUP.Infrastructure.Exceptions;
using FollowUP.Infrastructure.Extensions;
using InstagramApiSharp;
using InstagramApiSharp.API;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Helpers;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Services.InstagramAuthenticationService
{
    public class InstagramAuthenticationService : IInstagramAuthenticationService
    {
        private readonly IMemoryCache _cache;
        private readonly IInstagramApiService _instagramApiService;

        public InstagramAuthenticationService(IMemoryCache cache,
                IInstagramApiService instagramApiService)
        {
            _cache = cache;
            _instagramApiService = instagramApiService;
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
        public async Task LoginAsync(InstagramAccount account, string phoneNumber,
            string twoFactorCode, string verificationCode, bool preferSMSVerification,
            bool replayChallenge)
        {
            var instaApi = await _instagramApiService.GetInstaApiAsync(account, true);

            // If user is authenticated
            if (instaApi.IsUserAuthenticated)
            {
                await SendMockupRequestsAfterLoginAsync(instaApi);
                return;
            }

            // Send requests before login to initialize login session
            if (verificationCode.Empty() && twoFactorCode.Empty())
            {
                await instaApi.SendRequestsBeforeLoginAsync();
            }

            IResult<InstaLoginResult> logInResult;

            // Log in the user (only, if there is no two factor nor verification code)
            if (!twoFactorCode.Empty())
            {
                logInResult = new Result<InstaLoginResult>(false, InstaLoginResult.TwoFactorRequired);
            }
            else if (!verificationCode.Empty())
            {
                logInResult = new Result<InstaLoginResult>(false, InstaLoginResult.ChallengeRequired);
            }
            else
            {
                logInResult = await instaApi.LoginAsync();
            }

            // If the login succeeded
            if (logInResult.Succeeded)
            {
                instaApi.SaveSession();

                // Set the authentiaction step to authenticated
                await SendMockupRequestsAfterLoginAsync(instaApi);
                return;
            }

            if (logInResult.Value == InstaLoginResult.BadPassword)
            {
                throw new ServiceException(ErrorCodes.InvalidCredentials,
                    $"Given password is invalid.");
            }

            // If the challenge is not required
            if (logInResult.Value != InstaLoginResult.ChallengeRequired)
            {
                // If the user has two factor authentication enabled
                if (logInResult.Value == InstaLoginResult.TwoFactorRequired)
                {
                    // Authenticate with two factor code
                    await LoginWithTwoFactorAsync(instaApi, account, twoFactorCode);
                }
                // Every other error during logging in
                else
                {
                    Console.WriteLine($"Unable to login: {logInResult.Info.Message}");
                    throw new ServiceException(ErrorCodes.UnknownError,
                        $"Received unexpected response from Instagram while logging in" +
                        $"user: {account.Username}.");
                }
            }

            // If we are here, that means the Instagram's challenge is required

            // Start the challenge response
            var challenge = await instaApi.GetChallengeRequireVerifyMethodAsync();

            // If it didn't succeed
            if (!challenge.Succeeded)
            {
                throw new ServiceException(ErrorCodes.UnknownError,
                    "Instagram challenge was required however it failed for unknown reason.");
            }

            // If the phone number submit is required
            if (challenge.Value.SubmitPhoneRequired)
            {
                // Try submitting the phone number
                try
                {
                    await SubmitPhoneAsync(instaApi, account, phoneNumber);
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw new ServiceException(ErrorCodes.PhoneRequired,
                        "Submitting phone number failed critically.");
                }
            }

            // If we have verification code
            if (!verificationCode.Empty())
            {
                // Verify if the code is correct
                var verifyCodeLogin = await instaApi.VerifyCodeForChallengeRequireAsync(verificationCode);

                // If the login succeeded...
                if (verifyCodeLogin.Succeeded)
                {
                    instaApi.SaveSession();
                    await SendMockupRequestsAfterLoginAsync(instaApi);
                    return;
                }
                else if (verifyCodeLogin.Value == InstaLoginResult.TwoFactorRequired)
                {
                    throw new ServiceException(ErrorCodes.TwoFactorRequired,
                        "Two factor code is required for further authentication.");
                }
                // TODO: Check this reponse
                else if (verifyCodeLogin.Value == InstaLoginResult.ChallengeRequired)
                {
                    throw new ServiceException(ErrorCodes.VerificationCodeRequired,
                        "Given verification code was invalid.");
                }
            }

            // If we are here, it means it is before entering the codes

            // If we have valid email or phone number
            if (challenge.Value.StepData != null && verificationCode.Empty())
            {
                // If the user prefers verification via SMS
                if (preferSMSVerification)
                {
                    if (!challenge.Value.StepData.PhoneNumber.Empty())
                    {
                        await RequestFromSMSAsync(instaApi, account, replayChallenge);
                    }
                    else if (!challenge.Value.StepData.Email.Empty())
                    {
                        await RequestFromEmailAsync(instaApi, account, replayChallenge);
                    }

                    throw new ServiceException(ErrorCodes.ChallengeRequired,
                        "None of the verification options worked.");
                }

                if (!challenge.Value.StepData.Email.Empty())
                {
                    await RequestFromEmailAsync(instaApi, account, replayChallenge);
                }
                else if (!challenge.Value.StepData.PhoneNumber.Empty())
                {
                    await RequestFromSMSAsync(instaApi, account, replayChallenge);
                }

                throw new ServiceException(ErrorCodes.ChallengeRequired,
                        "None of the verification options worked.");
            }
        }

        private async Task SendMockupRequestsAfterLoginAsync(IInstaApi instaApi)
        {
            instaApi.GetCurrentDevice().PigeonSessionId = Guid.NewGuid();
            await instaApi.SendRequestsAfterLoginAsync();
            await instaApi.GetBanyanSuggestionsAsync();
            await instaApi.StoryProcessor.GetStoryFeedWithPostMethodAsync(/* preloaded reel ids */);
            await instaApi.FeedProcessor.GetUserTimelineFeedAsync(PaginationParameters.MaxPagesToLoad(1));
            await instaApi.SendRequestsAfterFeedFetchAsync();
            await instaApi.BusinessProcessor.GetBusinessAccountInformationAsync();
            await instaApi.MessagingProcessor.GetUsersPresenceAsync();
            await instaApi.GetViewableStatusesAsync();
            await instaApi.MessagingProcessor.GetDirectInboxAsync(PaginationParameters.MaxPagesToLoad(1), 1);
            await instaApi.MessagingProcessor.GetDirectInboxAsync(PaginationParameters.MaxPagesToLoad(1), 2);
            await instaApi.FeedProcessor.GetTopicalExploreFeedAfterLogin();
            await instaApi.GetNotificationBadge();
            instaApi.SetDevice(instaApi.GetCurrentDevice().RandomizeBandwithConnection());
            await Task.Delay(0);
        }

        /// <summary>
        /// Request to send verification code via SMS
        /// </summary>
        /// <param name="instaApi">The instance of <see cref="IInstaApi"/> to use</param>
        /// <param name="account">The Instagram account to authenticate</param>
        /// <param name="replayChallenge">Whether to resend the code</param>
        /// <returns></returns>
        public async Task RequestFromSMSAsync(IInstaApi instaApi, InstagramAccount account, bool replayChallenge = false)
        {
            var requestPhoneVerify = await instaApi.RequestVerifyCodeToSMSForChallengeRequireAsync(replayChallenge);
            if (requestPhoneVerify.Succeeded)
            {
                _cache.Set(account.Username, instaApi);
                throw new ServiceException(ErrorCodes.VerificationCodeRequired,
                    "Verification code has been sent to your phone.");
            }

            throw new ServiceException(ErrorCodes.ChallengeRequired,
                "Getting verification code for SMS failed.");
        }

        /// <summary>
        /// Request to send verification code via Email
        /// </summary>
        /// <param name="instaApi">The instance of <see cref="IInstaApi"/> to use</param>
        /// <param name="account">The Instagram account to authenticate</param>
        /// <param name="replayChallenge">Whether to resend the code</param>
        /// <returns></returns>
        public async Task RequestFromEmailAsync(IInstaApi instaApi, InstagramAccount account, bool replayChallenge = false)
        {
            var requestEmailVerify = await instaApi.RequestVerifyCodeToEmailForChallengeRequireAsync(replayChallenge);
            if (requestEmailVerify.Succeeded)
            {
                _cache.Set(account.Username, instaApi);
                throw new ServiceException(ErrorCodes.VerificationCodeRequired,
                    "Verification code has been sent to your email.");
            }

            throw new ServiceException(ErrorCodes.ChallengeRequired,
                "Getting verification code for email failed.");
        }

        /// <summary>
        /// Attempt to login account via Two Factor Authentication
        /// </summary>
        /// <param name="instaApi">The instance of <see cref="IInstaApi"/> to use</param>
        /// <param name="account">The Instagram account to authenticate</param>
        /// <param name="twoFactorCode">The 2FA code received via SMS or e-mail</param>
        /// <returns></returns>
        public async Task LoginWithTwoFactorAsync(IInstaApi instaApi, InstagramAccount account, string twoFactorCode)
        {
            // If this is the first time and we don't have the two factor code
            if (twoFactorCode.Empty())
            {
                // Remember the instaApi instance
                _cache.Set(account.Username, instaApi);
                throw new ServiceException(ErrorCodes.TwoFactorRequired,
                    "Two factor authentication required - no code has been given yet.");
            }

            // Two factor authentication
            var twoFactorLogin = await instaApi.TwoFactorLoginAsync(twoFactorCode);

            if (twoFactorLogin.Succeeded)
            {
                instaApi.SaveSession();

                await SendMockupRequestsAfterLoginAsync(instaApi);
                return;
            }
            else
            {
                Console.WriteLine(twoFactorLogin.Info.Message);
                throw new ServiceException(ErrorCodes.TwoFactorFailed,
                    "Two factor authentication failed.");
            }
        }

        /// <summary>
        /// Submit phone number for further verification if it hasn't been set yet
        /// </summary>
        /// <param name="instaApi">The instance of <see cref="IInstaApi"/> to use</param>
        /// <param name="account">The Instagram account to add the number to</param>
        /// <param name="phoneNumber">The phone number that we want to add to the account</param>
        /// <returns></returns>
        public async Task SubmitPhoneAsync(IInstaApi instaApi, InstagramAccount account, string phoneNumber)
        {
            // Return phone required if it's empty
            if (phoneNumber.Empty())
            {
                throw new ServiceException(ErrorCodes.PhoneRequired,
                    "Phone number is required for further authentication.");
            }

            // Add the + if necessary
            if (!phoneNumber.StartsWith("+"))
            {
                phoneNumber = $"+{phoneNumber}";
            }

            var submitPhone = await instaApi.SubmitPhoneNumberForChallengeRequireAsync(phoneNumber);
            if (submitPhone.Succeeded)
            {
                var verifyPhoneAfterAdded = await instaApi.RequestVerifyCodeToSMSForChallengeRequireAsync();

                if (verifyPhoneAfterAdded.Succeeded)
                {
                    throw new ServiceException(ErrorCodes.VerificationCodeRequired,
                        "Please now enter the verification code.");
                }
                else
                {
                    throw new ServiceException(ErrorCodes.PhoneRequired,
                        "Phone number verification failed.");
                }
            }
            else
            {
                Console.WriteLine(submitPhone.Info.Message);
                throw new ServiceException(ErrorCodes.PhoneRequired,
                    "Phone number verification failed.");
            }
        }
    }
}
