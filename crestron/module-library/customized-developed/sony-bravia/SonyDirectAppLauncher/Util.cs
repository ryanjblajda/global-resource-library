using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace SonyLiteControl
{
    internal static class SimpleCommands
    {
        internal const int TCPPort = 20060;
        internal const string SetPictureMute = "*SCPMUT000000000000000{0}\n";
        internal const string UpdatePictureMute = "*SCEMUT################\n";

        internal const string PictureMutePrefix = "PMUT";
    }

    internal enum VolumeAction
    {
        None,
        Up,
        Down
    }

    internal enum Signal : ushort
    {
        Low,
        High
    }

    internal enum Transport : ushort
    {
        None,
        Up,
        Down,
        Left,
        Right,
        Select,
        Home,
        Options,
        Back,
        Play,
        Pause
    }

    public static class Ports
    {
        public const string HDMI = "hdmi";
        public const string Component = "comp";
        public const string SCART = "scart";
    }
}