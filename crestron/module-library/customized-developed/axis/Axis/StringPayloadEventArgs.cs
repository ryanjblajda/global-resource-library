using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace Axis
{
    public class StringPayloadEventArgs : EventArgs
    {
        public string Payload { get; private set; }

        public StringPayloadEventArgs()
        {
            this.Payload = "";
        }

        public StringPayloadEventArgs(string payload)
        {
            this.Payload = payload;
        }
    }
}