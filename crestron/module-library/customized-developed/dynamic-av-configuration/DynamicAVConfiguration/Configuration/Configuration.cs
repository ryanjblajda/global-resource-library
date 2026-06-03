using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharp.CrestronIO;
using Newtonsoft.Json;
using DynamicAVConfiguration.Members;
using DynamicAVConfiguration.Messaging;

namespace DynamicAVConfiguration.Configuration
{
    public static class Configuration
    {
        public static AVSystem AVSystem = new AVSystem();
        internal static MessageBus CommunicationBus = new MessageBus();
        internal static Boolean IsDebug;

        public static void EnableDebug(ushort enable)
        {
            Configuration.IsDebug = SIMPL.Conversion.ConvertToBool(enable);
            CommunicationBus.IsDebug = SIMPL.Conversion.ConvertToBool(enable);
        }

        public static void RefreshConfiguration(string path)
        {
            bool result = Configuration.Refresh(path);
            //publish a message with the results
            if (Configuration.IsDebug) { CrestronConsole.PrintLine("System Configuation Loaded: {0}", result); }
            Configuration.CommunicationBus.Publish(new ConfigurationResult(result));
        }

        private static bool Refresh(string path)
        {
            //assume we fail by default, if we succeed, we will change this to reflect that.
            bool success = false;

            try
            {
                try
                {
                }
                catch (Exception e)
                {
                    CrestronConsole.PrintLine("System Initialization Failure {0} | {1} | Error Clearing Room Subscriptions", e.Message, e.InnerException);
                }

                FileStream configFile = File.Open(path, FileMode.Open);
                StreamReader configReader = new StreamReader(configFile);
                if (configFile.CanRead)
                {
                    try
                    {
                        string configFileString = configReader.ReadToEnd();
                        configFile.Close();
                        AVSystem cfg = JsonConvert.DeserializeObject<AVSystem>(configFileString, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore});
                        Configuration.AVSystem = cfg;
                        success = true;
                    }
                    catch (Exception e)
                    {
                        CrestronConsole.PrintLine(String.Format("System Initialization Failure {0} | {1} | Error Reading JSON Config File", e.Message, e.InnerException));
                    }
                }
            }
            catch (Exception e)
            {
                CrestronConsole.PrintLine(String.Format("System Initialzation Failure {0} | {1} | Error Opening JSON Config File", e.Message, e.InnerException));
            }

            return (success);
        }
    }
}