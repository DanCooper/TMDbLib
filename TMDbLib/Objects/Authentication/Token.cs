﻿using System;
using Newtonsoft.Json;
using TMDbLib.Converters;

namespace TMDbLib.Objects.Authentication
{
    /// <summary>
    /// A request token is required in order to request a user authenticated session id.
    /// Request tokens will expire after 60 minutes. 
    /// As soon as a valid session id has been created the token will be useless.
    /// </summary>
    public class Token
    {
        /// <summary>
        /// The date / time before which the token must be used, else it will expire. Time is expressed as local time.
        /// </summary>
        [JsonProperty("expires_at")]
        [JsonConverter(typeof(DateTimeConverterYearMonthDayHourMinuteSecondUtc))]
        public DateTime ExpiresAt { get; set; }
        [JsonProperty("request_token")]
        public string RequestToken { get; set; }
        [JsonProperty("success")]
        public bool Success { get; set; }
        [JsonProperty("authentication_callback")]
        public string AuthenticationCallback { get; set; }
    }
}
