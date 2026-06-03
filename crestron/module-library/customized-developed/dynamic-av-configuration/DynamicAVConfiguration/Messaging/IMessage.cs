using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace DynamicAVConfiguration.Messaging
{
    public interface IMessage
    {
        string DebugMessage { get; set; }
    }
}