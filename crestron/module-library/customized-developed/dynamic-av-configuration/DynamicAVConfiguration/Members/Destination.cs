using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Newtonsoft.Json;

namespace DynamicAVConfiguration.Members
{
    public class Destination
    {
        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; private set; }

        [JsonProperty("output", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public ushort Output { get; private set; }

        [JsonProperty("matrix_id", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public ushort MatrixID { get; private set; }
    }
}