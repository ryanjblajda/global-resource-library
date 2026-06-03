using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Newtonsoft.Json;

namespace DynamicAVConfiguration.Members
{
    public class Source
    {
        [JsonProperty("name", DefaultValueHandling=DefaultValueHandling.Populate, NullValueHandling=NullValueHandling.Ignore)]
        public string Name { get; private set; }
        
        [JsonProperty("help", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public string HelpText { get; private set; }
        
        [JsonProperty("error", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public string ErrorText { get; private set; }
        
        [JsonProperty("icon_text", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public string IconString { get; private set; }
        
        [JsonProperty("icon_value", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public ushort IconAnalog { get; private set; }

        [JsonProperty("matrix_id", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public ushort MatrixID { get; private set; }

        [JsonProperty("input", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public ushort Input { get; private set; }

        [JsonProperty("sub_input", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        public ushort SubInput { get; private set; }

        [JsonProperty("routes", DefaultValueHandling = DefaultValueHandling.Populate, NullValueHandling = NullValueHandling.Ignore)]
        internal List<Route> Routes { get; private set; }

        private bool online;
        public bool Online 
        {
            get { return this.online; }
            private set
            {
                if (this.online != value)
                {
                    this.online = value;
                    Utilities.DebugMessage("Source {0} / Online: {1}", this.Name, this.online);
                    this.MessageBus.Publish<Messaging.SourceOnline>(new Messaging.SourceOnline(this));
                }
            }
        }

        private bool detected;
        public bool Detected 
        {
            get { return this.detected; }
            private set
            {
                if (this.detected != value)
                {
                    this.detected = value;
                    Utilities.DebugMessage("Source {0} / Detected: {1}", this.Name, this.detected);
                    this.MessageBus.Publish<Messaging.SourceDetected>(new Messaging.SourceDetected(this));
                }
            }
        }

        private Messaging.MessageBus MessageBus;

        public Source()
        {
            this.MessageBus = Configuration.Configuration.CommunicationBus;
            this.Name = "Generic Source";
            this.HelpText = "Please load a configuration to view a valid help message";
            this.ErrorText = "Please load a configuration to view a valid error message";
            this.IconAnalog = 0;
            this.IconString = "";
            this.Routes = new List<Route>();
            this.Online = false;
            this.Detected = false;
            this.MessageBus.Subscribe<Messaging.InputDetectedStatus>(this.OnInputChanged);
            this.MessageBus.Subscribe<Messaging.InputOnlineStatus>(this.OnInputChanged);
        }

        public Source(ushort id, string name, List<Route> routes) : this()
        {
            this.MatrixID = id;
            this.Routes = routes;
            this.Name = name;
        }

        public Source(ushort id, string name, ushort input, ushort subinput, List<Route> routes): this(id, name, routes)
        {
            this.Input = input;
            this.SubInput = subinput;
        }

        private void CheckInputOnlineUpdateValid(Messaging.InputOnlineStatus message)
        {
            if (this.MatrixID == message.MatrixID)
            {
                if (message.Input == this.Input || this.SubInput == message.SubInput) { this.Online = message.Online; }
            }
        }

        private void CheckInputDetectedUpdateValid(Messaging.InputDetectedStatus message)
        {
            if (this.MatrixID == message.MatrixID)
            {
                if (message.Input == this.Input || this.SubInput == message.SubInput) { this.Detected = message.Detected; }
            }
        }
        
        private void OnInputChanged(Messaging.IMessage message)
        {
            Utilities.DebugMessage("Source: {1} | Received Input Changed Message => {0}", message.DebugMessage, this.Name);

            if (typeof(Messaging.InputOnlineStatus) == message.GetType())
            {
                Messaging.InputOnlineStatus status = (Messaging.InputOnlineStatus)message;
                this.CheckInputOnlineUpdateValid(status);
            }
            else if (typeof(Messaging.InputDetectedStatus) == message.GetType())
            {
                Messaging.InputDetectedStatus status = (Messaging.InputDetectedStatus)message;
                this.CheckInputDetectedUpdateValid(status);
            }
        }
    }
}