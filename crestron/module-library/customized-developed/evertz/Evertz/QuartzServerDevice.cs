using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using System.Collections.ObjectModel;
using System.ComponentModel;
//using Crestron.SimplSharp.CrestronSockets;

namespace Quartz
{
    public class QuartzServerDevice : INotifyPropertyChanged
    {
        /// <summary>
        /// a unique identifier to allow servers & routers to associate themselves with one another
        /// </summary>
        private ushort id;
        public ushort ID { get { return this.id; }
            set
            {
                if (this.id != value)
                {
                    this.id = value;
                    this.DebugMessage("ID Set: {0}", this.id);
                }
            }
        }
        /// <summary>
        /// the IP address of the server running the Quartz Protocol that we wish to communicate with
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// the custom port we should be using to connect to the server
        /// </summary>
        public ushort Port { get; set; }

        /// <summary>
        /// holds the QuartzRouter objects with matching id's so that we can notify them as needed
        /// </summary>
        private List<QuartzServerRouter> Subscribers;

        //private TCPClient QuartzConnection;

        /// <summary>
        /// called when a property changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// called by SIMPL by default
        /// </summary>
        public QuartzServerDevice()
        {
            //add ourselves to the list of registered devices
            QuartzProtocol.RegisteredServers.Add(this);
            //create a list to hold subscribers;
            this.Subscribers = new List<QuartzServerRouter>();
            //this.QuartzConnection = new TCPClient();
            // this.QuartzConnection.SocketStatusChange += this.OnSocketStatusChanged;
            //subscribe to when the registered routers list changes, so that we can make sure to update routers as they register.
            QuartzProtocol.RegisteredRouters.ListChanged += OnRegisteredRoutersListChanged;
            //need to loop through the list as it stands, so we can associate any existing devices that were created before we were
        }

        /// <summary>
        /// called when the registered router list changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnRegisteredRoutersListChanged(object sender, ListChangedEventArgs e)
        {
            this.DebugMessage("Received RegisteredRoutersListChanged Event");
            if (e.ListChangedType == ListChangedType.ItemChanged) {
                BindingList<QuartzServerRouter> list = (BindingList<QuartzServerRouter>)sender;
                if (list[e.NewIndex].ServerDeviceID == this.ID)
                {
                    this.DebugMessage("QuartzRouter ID Match!");

                    if (!this.Subscribers.Contains(list[e.NewIndex])) {
                        this.DebugMessage("Adding QuartzRouter to Subscribers");
                        this.Subscribers.Add(list[e.NewIndex]);
                    }
                }
                else
                {
                    if (this.Subscribers.Contains(list[e.NewIndex]))
                    {
                        this.DebugMessage("Removing QuartzRouter from Subscribers");
                        this.Subscribers.Remove(list[e.NewIndex]);
                    }
                }
            }
        }

        /// <summary>
        /// when the socket status changes
        /// </summary>
        /// <param name="client"></param>
        /// <param name="socketStatus"></param>
        /*void OnSocketStatusChanged(TCPClient client, SocketStatus socketStatus)
        {
            this.DebugMessage("Socket Status: {0}", socketStatus);
        }

        /// <summary>
        /// callback for tcp client connection
        /// </summary>
        void OnSocketConnected(TCPClient client)
        {
            this.DebugMessage("Socket Connected");
        }

        /// <summary>
        /// connect to the server
        /// </summary>
        public void Connect()
        {
            this.DebugMessage("Attempting Connection");
            if (this.IPAddress != "" && this.Port != 0)
            {
                this.QuartzConnection.PortNumber = this.Port;
                this.QuartzConnection.AddressClientConnectedTo = this.IPAddress;
                SocketErrorCodes result = this.QuartzConnection.ConnectToServerAsync(this.OnSocketConnected);
                this.DebugMessage("Connection Attempt Result: {0}", result);
            }
            else
            {
                this.DebugMessage("Either IPAddress or Port Invalid!");
            }
        }*/

        void DebugMessage(string msg, params object[] args)
        {
            string debugMsg = string.Format(msg, args);
            QuartzProtocol.DebugMessage("QuartzServer // ID: {0} | {1}", this.ID, debugMsg);
        }

    }
}