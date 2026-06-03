using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Newtonsoft.Json;

namespace DynamicAVConfiguration.Members
{
    public class Route
    {
        [JsonProperty("matrix_id", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public ushort MatrixID { get; private set; }

        [JsonProperty("input", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public ushort Input { get; private set; }

        [JsonProperty("sub_input", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public ushort SubInput { get; private set; }
        
        [JsonProperty("output", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public ushort Output { get; private set; }

        [JsonProperty("detection_host", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public bool DetectionHost { get; private set; }

        public Route()
        {
        }

        public Route(ushort id, ushort input, ushort subinput, bool host)
        {

        }
    }
}