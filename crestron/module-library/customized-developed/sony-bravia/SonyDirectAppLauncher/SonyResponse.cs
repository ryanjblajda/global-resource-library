using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Newtonsoft.Json;

namespace SonyLiteControl
{
    internal class GenericResponse
    {
        [JsonProperty("result", NullValueHandling = NullValueHandling.Ignore)]
        public List<GenericDetails> GenericResults { get; set; }

        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public long? ID { get; set; }
    }

    internal class AudioResponse
    {
        [JsonProperty("result", NullValueHandling = NullValueHandling.Ignore)]
        public List<List<AudioDetails>> AudioResults { get; set; }

        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public long? ID { get; set; }
    }

    //GetVolumeInformation
    internal class AudioDetails
    {
        [JsonProperty("volume", NullValueHandling = NullValueHandling.Ignore)]
        public long? Volume { get; set; }

        [JsonProperty("minVolume", NullValueHandling = NullValueHandling.Ignore)]
        public long? MinVolume { get; set; }

        [JsonProperty("mute", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Mute { get; set; }

        [JsonProperty("maxVolume", NullValueHandling = NullValueHandling.Ignore)]
        public long? MaxVolume { get; set; }

        [JsonProperty("target", NullValueHandling = NullValueHandling.Ignore)]
        public string Target { get; set; }
    }

    //GetPowerStatus
    internal class GenericDetails
    {
        [JsonProperty("status", NullValueHandling = NullValueHandling.Ignore)]
        public string Status { get; set; }

        [JsonProperty("source", NullValueHandling = NullValueHandling.Ignore)]
        public string Source { get; set; }

        [JsonProperty("title", NullValueHandling = NullValueHandling.Ignore)]
        public string Title { get; set; }

        [JsonProperty("uri", NullValueHandling = NullValueHandling.Ignore)]
        public string Uri { get; set; }
    }
}