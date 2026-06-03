using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Newtonsoft.Json;
using Crestron.SimplSharp.Net.Https;

namespace Clickshare.API
{
    internal class Request
    {
        public HttpsClientRequest WebRequest { get; private set; }
        public string Method { get; private set; }

        public Request(HttpsClientRequest request, string method)
        {
            this.WebRequest = request;
            this.Method = method;
        }
    }

    internal class LoginRequest
    {
        [JsonProperty("app")]
        public ApplicationLanguageInformation ApplicationLanguage { get; private set; }

        [JsonProperty("username")]
        public string Username { get; private set; }

        [JsonProperty("password")]
        public string Password { get; private set; }

        /// <summary>
        /// constructor for login request
        /// </summary>
        /// <param name="language">the language to use</param>
        /// <param name="user">the username for the device</param>
        /// <param name="pass">the password for the device</param>
        public LoginRequest(string language, string user, string pass)
        {
            this.Username = user;
            this.Password = pass;
            this.ApplicationLanguage = new ApplicationLanguageInformation(language);
        }
    }

    internal class DecodingPresetRequest
    {
        [JsonProperty("id")]
        public int Preset { get; private set; }

        public DecodingPresetRequest(int preset)
        {
            this.Preset = preset;
        }
    }
}