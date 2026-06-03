using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using DynamicAVConfiguration.Messaging;
using SIMPL;

namespace DynamicAVConfiguration.Members
{
    public class MatrixRouteEventArgs
    {
        public ushort Output { get; private set; }
        public ushort Input { get; private set; }
        public ushort SubInput { get; private set; }

        public MatrixRouteEventArgs()
        {
        }

        public MatrixRouteEventArgs(ushort output, ushort input, ushort subinput)
        {
            this.Output = output;
            this.Input = input;
            this.SubInput = subinput;
        }
    }

    public class VideoMatrix
    {
        /// <summary>
        /// a global reference to the message bus that publishes messages throughout the system
        /// </summary>
        private Messaging.MessageBus MessageBus;
        /// <summary>
        /// the symbol location in simpl windows used for debugging only
        /// </summary>
        public string SymbolName { get; set; }
        /// <summary>
        /// boolean representing the online status of the matrix
        /// </summary>
        public bool Online { get; private set; }
        /// <summary>
        /// the matrix id, an integer identifier to uniquely identify this matrix
        /// </summary>
        public ushort MatrixID { get; set; }
        /// <summary>
        /// a delegate function to retrieve the detection or online status of an input/output
        /// </summary>
        /// <param name="index">the input or output to refresh the status of</param>
        /// <returns>a ushort representing the signal detection or online status of an input or output</returns>
        public delegate ushort GetUshortStatus(ushort index);
        /// <summary>
        /// get the input signal detection status of an input index array on the simpl+ symbol we defined as a video matrix
        /// </summary>
        public GetUshortStatus GetInputSignalDetectionStatus { get; set; }
        /// <summary>
        /// get the input online detection status of an input index array on the simpl+ symbol we defined as a video matrix
        /// </summary>
        public GetUshortStatus GetInputSignalOnlineStatus { get; set; }
        /// <summary>
        /// get the output online detection status of an output index array on the simpl+ symbol we defined as a video matrix
        /// </summary>
        public GetUshortStatus GetOutputSignalOnlineStatus { get; set; }

        public delegate void MatrixRouteDelegate(object sender, MatrixRouteEventArgs args);
        public event MatrixRouteDelegate MatrixRouteRequest;

        public VideoMatrix()
        {
            this.Online = false;
            this.MatrixID = 0;
            this.SymbolName = "Symbol Location Not Set";
            this.MessageBus = Configuration.Configuration.CommunicationBus;
            this.MessageBus.Subscribe<ConfigurationResult>(this.OnConfigurationUpdated);
            this.MessageBus.Subscribe<MatrixRouteRequest>(this.OnMatrixRouteRequest);
        }

        public VideoMatrix(ushort id) : this()
        {
            this.MatrixID = id;
        }

        /// <summary>
        /// only prints when debugging
        /// </summary>
        private void DebugPrint(string message, params object[] args)
        {
            if (Configuration.Configuration.IsDebug) { CrestronConsole.PrintLine(String.Format("Video Matrix {1} @ {0} // ", this.SymbolName, this.MatrixID) + message, args); } 
        }
        /// <summary>
        /// called when the message bus publishes a config update message
        /// </summary>
        internal void OnConfigurationUpdated(ConfigurationResult message)
        {
            this.DebugPrint("Received Configuration Update, Configuration Load {0}", message.Loaded ? "Successful" : "Failure");
            if (message.Loaded)
            {
                //get input online status
                //get input detect status
                this.UpdateValidSourcesStatus();
                //get output online status
            }
        }

        /// <summary>
        /// called when the message bus publishes a matrix route request message
        /// </summary>
        /// <param name="message"></param>
        internal void OnMatrixRouteRequest(MatrixRouteRequest message)
        {
            this.DebugPrint(message.DebugMessage);
            //we only want to do something if the message was intended for us.
            if (message.MatrixID == this.MatrixID)
            {
                //make sure that someone has subscribed to our event to prevent null reference errors
                if (this.MatrixRouteRequest != null) { this.MatrixRouteRequest(this, new MatrixRouteEventArgs(message.Output, message.Input, message.SubInput)); }
            }
        }

        /// <summary>
        /// this is called internally by the matrix when the system configuration is loaded to make sure that if a user changes the config, the status is updated and propogated to all devices
        /// after that occurs
        /// </summary>
        private void UpdateValidSourcesStatus()
        {
            //create a copy of the list
            List<Source> sources = new List<Source>();
            //briefly lock the list to prevent errors since multiple simpl threads may access the object at once.
            lock (Configuration.Configuration.AVSystem.Sources) { sources = Configuration.Configuration.AVSystem.Sources.ToList(); }
            //release the lock and then loop through our copy of the list
            List<Source> validSources = sources.Where(src => src.Routes.Where(route => route.MatrixID == this.MatrixID).Count() > 0).ToList();
            validSources.ForEach(delegate(Source src)
            {
                Route match = src.Routes.First(route => route.MatrixID == this.MatrixID);
                //get the input detection status if it is not null
                if (this.GetInputSignalDetectionStatus != null)
                {
                    this.DebugPrint("Refreshing Input Detection Status -> Source: {0} // Input: {1} // SubInput: {2}", src.Name, match.Input, match.SubInput);
                    ushort result = this.GetInputSignalDetectionStatus(match.Input);
                    bool detected = SIMPL.Conversion.ConvertToBool(result);
                    this.DebugPrint("Current Input Detection Status ->  Source: {0} // Input: {1} // SubInput: {2} -> {3}", src.Name, match.Input, match.SubInput, detected);
                }
                //get the input online status if it is not null
                if (this.GetInputSignalOnlineStatus != null)
                {
                    this.DebugPrint("Refreshing Input Online Status -> Source: {0} // Input: {1} // SubInput: {2}", src.Name, match.Input, match.SubInput);
                    ushort result = this.GetInputSignalOnlineStatus(match.Input);
                    bool online = SIMPL.Conversion.ConvertToBool(result);
                    this.DebugPrint("Current Input Online Status ->  Source: {0} // Input: {1} // SubInput: {2} -> {3}", src.Name, match.Input, match.SubInput, online);
                }
            });
        }
        /// <summary>
        /// called from simpl+ when an inputs detection status changes
        /// </summary>
        /// <param name="index">the input on the matrix that caused the update</param>
        /// <param name="status">the status of the input</param>
        public void UpdateInputDetectionStatus(ushort index, ushort status)
        {
            bool detected = SIMPL.Conversion.ConvertToBool(status);
            InputDetectedStatus message = new InputDetectedStatus(this.MatrixID, index, 0, detected);
            this.DebugPrint(message.DebugMessage);
            this.MessageBus.Publish(message);
        }
        /// <summary>
        /// called from simpl+ when an inputs online status changes
        /// </summary>
        /// <param name="index">the input on the matrix that caused the update</param>
        /// <param name="status">the status of the input</param>
        public void UpdateInputOnlineStatus(ushort index, ushort status)
        {
            bool online = SIMPL.Conversion.ConvertToBool(status);
            InputOnlineStatus message = new InputOnlineStatus(this.MatrixID, index, 0, online);
            this.DebugPrint(message.DebugMessage);
            this.MessageBus.Publish(message);
        }
        /// <summary>
        /// called from simpl+ when an outputs online status changes
        /// </summary>
        /// <param name="index">the input on the matrix that caused the update</param>
        /// <param name="status">the status of the input</param>
        public void UpdateOutputOnlineStatus(ushort index, ushort status)
        {
            bool online = SIMPL.Conversion.ConvertToBool(status);
            OutputOnlineStatus message = new OutputOnlineStatus(this.MatrixID, index, online);
            this.DebugPrint(message.DebugMessage);
            this.MessageBus.Publish(message);
        }
        /// <summary>
        /// called from simpl+ when a matrix comes online
        /// </summary>
        /// <param name="status">the online status of the matrix</param>
        public void UpdateMatrixOnlineStatus(ushort status)
        {
            bool online = SIMPL.Conversion.ConvertToBool(status);
            this.DebugPrint("Online: {0}", online);
            MatrixOnlineStatus message = new MatrixOnlineStatus(this.MatrixID, online);
            this.DebugPrint(message.DebugMessage);
            this.MessageBus.Publish(message);
        }

        /// <summary>
        /// called from simpl+ when a matrix fb signal updates its status 
        /// </summary>
        /// <param name="output">the output whose feedback changed</param>
        /// <param name="input">potentially the item that caused the change, but sent to make sure all receivers are up to date</param>
        /// <param name="subinput">potentially the item that caused the chang, but sent to make sure all receivers are up to datee</param>
        public void UpdateMatrixRouteStatus(ushort output, ushort input, ushort subinput)
        {
            MatrixRouteUpdate message = new MatrixRouteUpdate(this.MatrixID, input, subinput, output);
            this.DebugPrint(message.DebugMessage);
            this.MessageBus.Publish<MatrixRouteUpdate>(message);
        }
    }
}