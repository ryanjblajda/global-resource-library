using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Newtonsoft.Json;

namespace DynamicAVConfiguration.Members
{
    public class Room
    {
        [JsonProperty("name", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; private set; }
        
        [JsonProperty("sources", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public ushort[] MaximumAvailableSources { get; private set; }

        [JsonProperty("displays", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public ushort[] MaximumAvailableDestinations { get; private set; }

        public ushort ID { get; internal set; }
    }
}