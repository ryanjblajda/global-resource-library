using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Newtonsoft.Json;

namespace BasicConfigReader
{
    public class Item
    {
        [JsonProperty("name")]
        public string Name { get; private set; }

        [JsonProperty("value")]
        public object GenericValue { get; internal set; }

        public ushort Location { get; private set; }

        public Util.Signal Type { get; private set; }

        public Item(Util.Signal type)
        {
            this.Type = type;
        }

        public Item(Util.Signal type, ushort location)
        {
            this.Type = type;
            this.Location = location;
        }

        public Item(Util.Signal type, string name, ushort location)
        {
            this.Type = type;
            this.Name = name;
            this.Location = location;
        }

        [JsonConstructor()]

        public Item(string name, object value)
        {
            this.Name = name;
            this.GenericValue = value;
        }

        public string GetValueString() { return Convert.ToString(this.GenericValue); }

        public short GetValueShort() { return Convert.ToInt16(this.GenericValue); } 
    }

    public class Configuration
    {
        [JsonProperty("strings")]
        public List<Item> Strings { get; set; }
        [JsonProperty("digitals")]
        public List<Item> Digitals { get; set; }
        [JsonProperty("analogs")]
        public List<Item> Analogs { get; set; }
    }
}