using FollowUP.Core.Domain;
using FollowUP.Core.Repositories;
using InstagramApiSharp.API;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Android.DeviceInfo;
using InstagramApiSharp.Classes.SessionHandlers;
using InstagramApiSharp.Enums;
using InstagramApiSharp.Helpers;
using InstagramApiSharp.Logger;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace FollowUP.Infrastructure.Services.InstagramApiService
{
    public class InstagramApiService : IInstagramApiService
    {
        private readonly IProxyRepository _proxyRepository;
        private readonly IInstagramAccountRepository _accountRepository;
        private readonly IMemoryCache _cache;

        public InstagramApiService(IProxyRepository proxyRepository, IMemoryCache cache,
                                   IInstagramAccountRepository accountRepository)
        {
            _proxyRepository = proxyRepository;
            _accountRepository = accountRepository;
            _cache = cache;
        }

        public async Task<IInstaApi> GetInstaApi(InstagramAccount account)
        {
            // Divide proxy to proper proxy parts
            var accountProxy = await _proxyRepository.GetAccountsProxyAsync(account.Id);

            if (accountProxy == null)
            {
                Console.WriteLine($"[{DateTime.Now}][{account.Username}] Can't find any working proxy, skipping...");
                await Task.Delay(5000);
                return null;
            }

            var proxyInfo = await _proxyRepository.GetAsync(accountProxy.ProxyId);

            if (proxyInfo.ExpiryDate < DateTime.UtcNow)
            {
                Console.WriteLine($"[{DateTime.Now}][{account.Username}] Proxy {proxyInfo.Ip} is expired, skipping...");
                await Task.Delay(TimeSpan.FromSeconds(5));
                return null;
            }

            var proxy = new InstaProxy("127.0.0.1", "8888");//proxyInfo.Ip, proxyInfo.Port);
            //{
            //    Credentials = new NetworkCredential(proxyInfo.Username, proxyInfo.Password)
            //};

            // Now create a client handler which uses that proxy
            var httpClientHandler = new HttpClientHandler()
            {
                Proxy = proxy,
            };

            // Set the user's credentials
            var userSession = new UserSessionData
            {
                UserName = account.Username,
                Password = account.Password
            };

            // Create new instance of InstaApi with given credentials, setting request delay and session handler for user
            var instaApi = InstaApiBuilder.CreateBuilder()
                                        .SetUser(userSession)
                                        .UseLogger(new DebugLogger(InstagramApiSharp.Logger.LogLevel.Exceptions))
                                        .SetSessionHandler(new FileSessionHandler() { FilePath = account.FilePath })
                                        .UseHttpClientHandler(httpClientHandler)
                                        .Build();

            // Check if there is an instaApi instance bound to the account...
            var instaApiCache = (IInstaApi)_cache.Get(account.Id);

            if (instaApiCache != null)
            {
                // ...if true, use it
                instaApi = instaApiCache;
            }

            // TODO: Device service
            instaApi.SetApiVersion(InstaApiVersionType.Version117);
            instaApi.SetDevice(AndroidDeviceGenerator.GetByName(account.AndroidDevice));

            // Try logging in from session
            try
            {
                instaApi?.SessionHandler?.Load();
                instaApi?.SessionHandler?.Save();
            }
            catch (Exception e)
            {
                Console.WriteLine($"[{DateTime.Now}][{account.Username}] Could not load state from file, error info: {e.Message}");
                await Task.Delay(TimeSpan.FromSeconds(10));
                return null;
            }

            if (!instaApi.IsUserAuthenticated)
            {
                Console.WriteLine($"[{DateTime.Now}][{account.Username}] User not logged in, please authenticate first.");
                await Task.Delay(TimeSpan.FromSeconds(1));
                return null;
            }

            return instaApi;
        }

        public async Task<InstagramAccount> GetRandomSlaveAccount()
        {
            return await _accountRepository.GetAsync("kontotestowefollowup1");
        }
    }
}
