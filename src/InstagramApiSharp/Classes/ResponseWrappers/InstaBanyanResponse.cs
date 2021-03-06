﻿/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharpMine
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharpMine
 * 
 * IRANIAN DEVELOPERS
 */

using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace InstagramApiSharp.Classes.ResponseWrappers
{
    public class InstaBanyanContainerResponse
    {
        [JsonProperty("entities")]
        public InstaBanyanSuggestionsResponse Entities { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }
    }

    public class InstaBanyanSuggestionsResponse
    {
        [JsonProperty("user")]
        public List<InstaUserShortResponse> Users { get; set; } = new List<InstaUserShortResponse>();
        [JsonProperty("thread")]
        public List<InstaDirectInboxThreadResponse> Threads { get; set; } = new List<InstaDirectInboxThreadResponse>();
    }
}
