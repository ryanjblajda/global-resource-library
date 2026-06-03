using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace DynamicAVConfiguration
{
    public static class Utilities
    {
        public static void DebugMessage(string msg, params object[] args)
        {
            if (Configuration.Configuration.IsDebug) {
                try {
                    CrestronConsole.PrintLine(msg, args);
                    Console.WriteLine(msg, args);
                }
                catch(Exception e) {
                    CrestronConsole.PrintLine("Exception Encountered Attempting To Print DebugMessage: {0}" , e.Message);
                    Console.WriteLine("Exception Encountered Attempting To Print DebugMessage: {0}", e.Message);
                }
            }
        }
    }
}