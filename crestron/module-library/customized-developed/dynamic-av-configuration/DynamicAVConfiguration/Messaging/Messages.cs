using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using DynamicAVConfiguration.Members;

namespace DynamicAVConfiguration.Messaging
{
    internal class ConfigurationResult : IMessage
    {
        /// <summary>
        /// whether or not the config was loaded properly
        /// </summary>
        public bool Loaded { get; private set; }
        /// <summary>
        /// a message to print for debugging
        /// </summary>
        public string DebugMessage { get; set; }

        public ConfigurationResult(bool success)
        {
            this.Loaded = success;
            this.DebugMessage = "av configuration load message published";
        }
    }

    internal class InputDetectedStatus : IMessage
    {
        /// <summary>
        /// the matrix id who published the event
        /// </summary>
        public ushort MatrixID { get; private set; }
        /// <summary>
        /// the input whose status changed
        /// </summary>
        public ushort Input { get; private set; }
        /// <summary>
        /// the subinput whose status changed
        /// </summary>
        public ushort SubInput { get; private set; }
        /// <summary>
        /// the new status
        /// </summary>
        public bool Detected { get; private set; }
        /// <summary>
        /// a message to print for debugging
        /// </summary>
        public string DebugMessage { get; set; }

        public InputDetectedStatus(ushort parent, ushort input, ushort subinput, bool status)
        {
            this.MatrixID = parent;
            this.Detected = status;
            this.Input = input;
            this.SubInput = subinput;
            this.DebugMessage = String.Format("Detection Status Changed -> Input: {0} // SubInput: {1} // Detected: {2}", this.Input, this.SubInput, this.Detected);
        }
    }

    internal class OutputOnlineStatus : IMessage
    {
        /// <summary>
        /// the matrix id who published the event
        /// </summary>
        public ushort MatrixID { get; private set; }
        /// <summary>
        /// the input whose status changed
        /// </summary>
        public ushort Output { get; private set; }
        /// <summary>
        /// the new status
        /// </summary>
        public bool Online { get; private set; }
        /// <summary>
        /// a message to print for debugging
        /// </summary>
        public string DebugMessage { get; set; }

        public OutputOnlineStatus(ushort parent, ushort output, bool status)
        {
            this.MatrixID = parent;
            this.Online = status;
            this.Output = output;
            this.DebugMessage = String.Format("Online Status Changed -> Output: {0} // Online: {2}", this.Output, this.Online);
        }
    }

    internal class InputOnlineStatus : IMessage
    {
        /// <summary>
        /// the matrix id who published the event
        /// </summary>
        public ushort MatrixID { get; private set; }
        /// <summary>
        /// the input whose status changed
        /// </summary>
        public ushort Input { get; private set; }
        /// <summary>
        /// the subinput whose status changed
        /// </summary>
        public ushort SubInput { get; private set; }
        /// <summary>
        /// the new status
        /// </summary>
        public bool Online { get; private set; }
        /// <summary>
        /// a message to print for debugging
        /// </summary>
        public string DebugMessage { get; set; }

        public InputOnlineStatus(ushort parent, ushort input, ushort subinput, bool status)
        {
            this.MatrixID = parent;
            this.Online = status;
            this.Input = input;
            this.SubInput = subinput;
            this.DebugMessage = String.Format("Online Status Changed -> Input: {0} // SubInput: {1} // Online: {2}", this.Input, this.SubInput, this.Online);
        }
    }

    internal class MatrixOnlineStatus : IMessage
    {
        /// <summary>
        /// the online status of the matrix
        /// </summary>
        public bool Online { get; private set; }
        /// <summary>
        /// the matrix id that provided the update
        /// </summary>
        public ushort MatrixID { get; private set; }
        /// <summary>
        /// a message to print for debugging
        /// </summary>
        public string DebugMessage { get; set; }

        public MatrixOnlineStatus(ushort matrixid, bool status)
        {
            this.Online = status;
            this.MatrixID = matrixid;
            this.DebugMessage = String.Format("Online Status Changed -> Matrix ID: {0} // Online: {1}", this.MatrixID, this.Online); 
        }
    }

    internal class MatrixRouteRequest : IMessage
    {
        public string DebugMessage { get; set; }

        public ushort Input { get; private set; }
        public ushort SubInput { get; private set; }
        public ushort Output { get; private set; }
        public ushort MatrixID { get; private set; }

        public MatrixRouteRequest(ushort matrixid, ushort input, ushort subinput, ushort output)
        {
            this.MatrixID = matrixid;
            this.Input = input;
            this.SubInput = subinput;
            this.Output = output;
            this.DebugMessage = String.Format("Matrix Tie Requested -> Matrix ID: {0} // Input: {1} | SubInput: {2} => Output: {3}", this.MatrixID, this.Input, this.SubInput, this.Output);
        }
    }

    internal class MatrixRouteUpdate : MatrixRouteRequest
    {
        public MatrixRouteUpdate(ushort matrixid, ushort input, ushort subinput, ushort output): base(matrixid, input, subinput, output)
        {
            this.DebugMessage = String.Format("Matrix Tie Updated -> Matrix ID: {0} // Input: {1} | SubInput: {2} => Output: {3}", this.MatrixID, this.Input, this.SubInput, this.Output);
        }
    }

    internal class SourceDetected : IMessage
    {
        public string DebugMessage { get; set; }
        
        public Source Source { get; private set; }
                
        public SourceDetected(Source src)
        {
            this.Source = src;
            this.DebugMessage = String.Format("Source Detect Changed -> Detected: {0} - Name: {1} // Matrix ID: {2} // Input: {3} // SubInput: {4}", this.Source.Detected, this.Source.Name, this.Source.MatrixID, this.Source.Input, this.Source.SubInput);
        }
    }

    internal class SourceOnline : IMessage
    {
        public string DebugMessage { get; set; }
        
        public Source Source { get; private set; }

        public SourceOnline(Source src)
        {
            this.Source = src;
            this.DebugMessage = String.Format("Source Online Changed -> Detected: {0} - Name: {1} // Matrix ID: {2} // Input: {3} // SubInput: {4}", this.Source.Online, this.Source.Name, this.Source.MatrixID, this.Source.Input, this.Source.SubInput);
        }
    }
}