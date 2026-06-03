using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace RandomizerKeypad
{
    public class StringEventArgs
    {
        public string Payload { get; private set; }

        public StringEventArgs()
        {
        }

        public StringEventArgs(string payload)
        {
            this.Payload = payload;
        }
    }

    public class StringArrayEventArgs
    {
        public string[] Payload { get; private set; }
        public ushort PayloadLength;

        public StringArrayEventArgs()
        {
        }

        public StringArrayEventArgs(List<string> list)
        {
            this.Payload = list.ToArray();
            this.PayloadLength = (ushort)list.Count;
        }
    }

    public class DigitalEventArgs
    {
        public ushort Payload { get; private set; }

        public DigitalEventArgs()
        {
        }

        public DigitalEventArgs(int payload)
        {
            this.Payload = (ushort)payload;
        }
    }
}