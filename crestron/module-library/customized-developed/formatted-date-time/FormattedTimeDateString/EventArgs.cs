using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace FormattedTimeDateString
{
    public class StringPayloadEventArgs
    {
        public string Payload { get; private set; }

        public StringPayloadEventArgs()
        {
        }

        public StringPayloadEventArgs(string payload)
        {
            this.Payload = payload;
        }
    }

    public class DigitalAnalogPayloadEventArgs
    {
        public ushort Payload { get; private set; }
        
        public DigitalAnalogPayloadEventArgs()
        {
        }

        public DigitalAnalogPayloadEventArgs(ushort payload)
        {
            this.Payload = payload;
        }
    }
}