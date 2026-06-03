using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Newtonsoft.Json;

namespace DynamicAVConfiguration.Members
{
    public class AVSystem
    {
        [JsonProperty("sources", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public List<Source> Sources { get; private set; }

        [JsonProperty("displays", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public List<Destination> Destinations { get; private set; }

        [JsonProperty("rooms", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public List<Room> Rooms { get; private set; }

        [JsonProperty("linking", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public Linking LinkFeatures { get; private set; }

        [JsonProperty("walls", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public List<Wall> Walls { get; private set; }

        //will register themselves as they get defined in the simpl program
        public List<VideoMatrix> Matrices { get; private set; }

        public AVSystem()
        {
            this.Sources = new List<Source>();
            this.Destinations = new List<Destination>();
            this.Rooms = new List<Room>();
            this.Walls = new List<Wall>();
            this.Matrices = new List<VideoMatrix>();
            this.LinkFeatures = new Linking();
        }
    }
}