using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace SonyLiteControl
{
    public class AnalogDigitalPayloadArgs : EventArgs
    {
        public ushort Payload { get; private set; }

        public AnalogDigitalPayloadArgs()
        {
        }

        public AnalogDigitalPayloadArgs(ushort payload)
        {
            this.Payload = payload;
        }
    }
}