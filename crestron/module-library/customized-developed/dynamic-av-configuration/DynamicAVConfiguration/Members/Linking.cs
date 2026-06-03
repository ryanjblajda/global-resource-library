using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Newtonsoft.Json;

namespace DynamicAVConfiguration.Members
{
    public class Linking
    {
        [JsonProperty("enabled", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public bool Enabled { get; private set; }
        
        [JsonProperty("broadcast", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public bool Broadcast { get; private set; }

        //should this be part of the dynamic config? this can be handled based on how the user interface is programmed
        [JsonProperty("detected_only", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public bool UseDetectedSourcesOnly { get; private set; }

        [JsonProperty("combine_sources", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public bool CombineSources { get; private set; }

        //named zones is removed and will be refactored to always happen, because any other thing is stupid
    }
}