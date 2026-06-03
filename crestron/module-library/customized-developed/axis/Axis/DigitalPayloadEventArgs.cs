using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace Axis
{
    public class DigitalPayloadEventArgs : EventArgs
    {
        public ushort Payload { get; internal set; }
        
        public DigitalPayloadEventArgs()
        {
        }

        public DigitalPayloadEventArgs(ushort payload)
        {
            this.Payload = payload;
        }
    }
}