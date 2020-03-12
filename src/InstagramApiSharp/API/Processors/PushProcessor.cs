/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Android.DeviceInfo;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Enums;
using InstagramApiSharp.Helpers;
using InstagramApiSharp.Logger;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace InstagramApiSharp.API.Processors
{
    internal class PushProcessor : IPushProcessor
    {
        private readonly AndroidDevice _deviceInfo;
        private readonly HttpHelper _httpHelper;
        private readonly IHttpRequestProcessor _httpRequestProcessor;
        private readonly InstaApi _instaApi;
        private readonly IInstaLogger _logger;
        private readonly UserSessionData _user;
        private readonly UserAuthValidate _userAuthValidate;
        public PushProcessor(AndroidDevice deviceInfo, UserSessionData user,
            IHttpRequestProcessor httpRequestProcessor, IInstaLogger logger,
            UserAuthValidate userAuthValidate, InstaApi instaApi, HttpHelper httpHelper)
        {
            _deviceInfo = deviceInfo;
            _user = user;
            _httpRequestProcessor = httpRequestProcessor;
            _logger = logger;
            _userAuthValidate = userAuthValidate;
            _instaApi = instaApi;
            _httpHelper = httpHelper;
        }

        public async Task<IResult<bool>> RegisterPushAsync(InstaPushChannelType pushChannelType = InstaPushChannelType.Mqtt)
        {
            UserAuthValidator.Validate(_userAuthValidate);
            try
            {
                var instaUri = UriCreator.GetPushRegisterUri();

                var data = new Dictionary<string, string>();

                if(pushChannelType == InstaPushChannelType.Mqtt)
                {
                    var kObj = new JObject
                    {
                        {"pn", InstaApiConstants.INSTAGRAM_PACKAGE_NAME},
                        {"di", _deviceInfo.PushDeviceGuid.ToString()},
                        {"ai", InstaApiConstants.FACEBOOK_ANALYTICS_APP_ID},
                        {"ck", InstaApiConstants.FACEBOOK_ORCA_APP_ID},
                    };
                    var kTokText = kObj.ToString(Formatting.None);
                    var base64KToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(kTokText));
                    var token = new JObject
                    {
                        {"k", base64KToken},
                        {"v", 0},
                        {"t", "fbns-b64"}
                    };

                    data = new Dictionary<string, string>
                    {
                        {"device_type", pushChannelType.GetChannelDeviceType()},
                        {"is_main_push_channel", (pushChannelType == InstaPushChannelType.Mqtt).ToString().ToLower()},
                        {"device_sub_type", "2"},
                        {"device_token", WebUtility.UrlEncode(token.ToString(Formatting.None))},
                        {"_csrftoken", _user.CsrfToken},
                        {"guid", _deviceInfo.DeviceGuid.ToString()},
                        {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                        {"users", _user.LoggedInUser.Pk.ToString()},
                        {"family_device_id", _deviceInfo.FamilyDeviceGuid.ToString()},
                    };
                }
                else if (pushChannelType == InstaPushChannelType.Fcm)
                {
                    data = new Dictionary<string, string>
                    {
                        {"device_type", pushChannelType.GetChannelDeviceType()},
                        {"is_main_push_channel", "false"},
                        {"device_sub_type", "0"},
                        {"device_token", "ftGLbT_8oOM:APA91bGMxUAKFmDWn_iYNld0-MUH6F1YkPHJuXevzZAAaO5ULD5QbkACR_7A7-b2WsTilw3f2iiScn80R5qao2kcQmefiOk9sW02My-ba8ZfO2u3FYxPuoNqIqQbwsEDrZXjFS6z3Qr4"},
                        {"_csrftoken", _user.CsrfToken},
                        {"guid", _deviceInfo.DeviceGuid.ToString()},
                        {"_uuid", _deviceInfo.DeviceGuid.ToString()},
                        {"users", _user.LoggedInUser.Pk.ToString()},
                        {"family_device_id", _deviceInfo.FamilyDeviceGuid.ToString()},
                    };
                }
                
                var request = _httpHelper.GetDefaultRequest(HttpMethod.Post, instaUri, _deviceInfo, data);
                var response = await _httpRequestProcessor.SendAsync(request, true);
                var json = await response.Content.ReadAsStringAsync();
                if (response.StatusCode != HttpStatusCode.OK)
                    return Result.UnExpectedResponse<bool>(response, json);
                var obj = JsonConvert.DeserializeObject<InstaDefault>(json);
                return obj.Status.ToLower() == "ok" ? Result.Success(true) : Result.UnExpectedResponse<bool>(response, json);
            }
            catch (HttpRequestException httpException)
            {
                _logger?.LogException(httpException);
                return Result.Fail(httpException, default(bool), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                _logger?.LogException(exception);
                return Result.Fail<bool>(exception);
            }
        }
    }
}
