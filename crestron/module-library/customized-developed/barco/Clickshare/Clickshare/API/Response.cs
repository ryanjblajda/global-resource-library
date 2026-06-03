using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Clickshare.Constants;

namespace Clickshare.API
{
    internal class Response
    {
        [JsonProperty("result")]
        public string Result { get; private set; }

        [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
        public JToken Data { get; private set; }

        [JsonProperty("msg", NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; private set; }

        public Response()
        {
            this.Result = Constants.Result.Error;
            this.Message = String.Empty;
        }

        [JsonConstructor]
        public Response(string result, JToken data, string message)
        {
            this.Result = result;
            this.Data = data;
            if (message != null) { this.Message = message; }
            else { this.Message = String.Empty; }
        }
    }
}