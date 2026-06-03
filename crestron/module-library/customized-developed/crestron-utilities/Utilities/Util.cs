using System;
using System.Text;
using Crestron.SimplSharp;                          				// For Basic SIMPL# Classes

namespace Util
{
    public enum DigitalSignal
    {
        Off = 0,
        On = 1
    }

    public enum Signal
    {
        Digital = 1,
        Analog = 2,
        Serial = 3
    }

    public static class Conversion
    {
        public static bool ConvertToBool(ushort value)
        {
            if ((DigitalSignal)value == DigitalSignal.On) return true;
            return false;
        }

        public static ushort ConvertToSignal(bool state)
        {
            if (state) return (ushort)DigitalSignal.On;
            return (ushort)DigitalSignal.Off;
        }
    }
}
