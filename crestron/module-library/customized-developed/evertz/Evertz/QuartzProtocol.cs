using System;
using System.Runtime.InteropServices;
using System.Text;
using Crestron.SimplSharp;                          				// For Basic SIMPL# Classes
using System.Collections.ObjectModel;
using System.Collections;
using System.ComponentModel;
using Util;

namespace Quartz
{
    public static class QuartzProtocol
    {
        /// <summary>
        /// when you create a quartz server object, it will register with this list, and then allow the rest of the devices to subscribe to messages from this device. 
        /// </summary>
        public static BindingList<QuartzServerDevice> RegisteredServers = new BindingList<QuartzServerDevice>();
        /// <summary>
        /// when you add a new router object [a miniature instance of a set of outputs to be controlled] they will register here.
        /// </summary>
        public static BindingList<QuartzServerRouter> RegisteredRouters = new BindingList<QuartzServerRouter>();
        /// <summary>
        /// enables printing debug messages to the console
        /// </summary>
        public static bool IsDebug;
        /// <summary>
        /// enables printing extra verbose debugging messages to the console
        /// </summary>
        public static bool IsVerbose;
        /// <summary>
        /// will also print using Console.WriteLine, only for use when debugging the library on a windows computer, not an appliance
        /// </summary>
        public static bool IsWindows;

        /// <summary>
        /// called by SIMPL when the library is instantiated
        /// </summary>
        static QuartzProtocol()
        {
            RegisteredRouters.ListChanged += OnListChanged;
            RegisteredServers.ListChanged += OnListChanged;
        }

        /// <summary>
        /// event handler called when any of the binding lists changes
        /// </summary>
        /// <param name="sender">the list that changed</param>
        /// <param name="e">the args object denoting what happened</param>
        static void OnListChanged(object sender, ListChangedEventArgs e)
        {
            VerboseMessage("Registered Device Event");

            if (sender == RegisteredServers)
            {
                DebugMessage("Registered Servers Changed!");
                RegisteredServersChanged(sender, e);
            }
            else if (sender == RegisteredRouters)
            {
                DebugMessage("Registered Routers Changed!");
                RegisteredRoutersChanged(sender, e);
            }
        }

        /// <summary>
        /// called when the registered server list changes 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void RegisteredServersChanged(object sender, ListChangedEventArgs e)
        {
        }

        /// <summary>
        /// called when the registered router list changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void RegisteredRoutersChanged(object sender, ListChangedEventArgs e)
        {
        }

        internal static void Message(string msg, params object[] args)
        {
            try
            {
                //generate the message based on args
                string debugMsg = String.Format(msg, args);
                //print the message
                CrestronConsole.PrintLine(String.Format("Evertz | Quartz Protocol | {0}", debugMsg));
                if (QuartzProtocol.IsWindows) { Console.WriteLine(String.Format("Evertz | Quartz Protocol | {0}", debugMsg)); }
            }
            catch (Exception e) { 
                CrestronConsole.PrintLine(String.Format("Evertz | Quartz Protocol | Error Creating Message {0}\r\r{1}", e.InnerException, e.Message));
                if (QuartzProtocol.IsWindows) { Console.WriteLine(String.Format("Evertz | Quartz Protocol | Error Creating Message {0}\r\r{1}", e.InnerException, e.Message)); }
            }
        }

        internal static void VerboseMessage(string msg, params object[] args)
        {
            if (QuartzProtocol.IsVerbose)
            {
                DebugMessage(msg, args);
            }
        }

        internal static void DebugMessage(string msg, params object[] args)
        {
            if (QuartzProtocol.IsDebug)
            {
                Message(msg, args);
            }
        }

        public static void Debug(ushort enable)
        {
            if (Util.Conversion.ConvertToBool(enable)) QuartzProtocol.IsDebug = true;
            else QuartzProtocol.IsDebug = false;
        }
    }
}
