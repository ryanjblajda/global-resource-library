using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using System.ComponentModel;

namespace Quartz
{
    public class QuartzServerRouter : INotifyPropertyChanged
    {
        /// <summary>
        /// the ID of the server device that we want to subscribe to
        /// </summary>
        private ushort serverdeviceid;
        public ushort ServerDeviceID
        {
            get { return this.serverdeviceid; }
            set
            {
                //only assign if it changes
                if (value != this.serverdeviceid)
                {
                    this.serverdeviceid = value;
                    this.DebugMessage("ID Set: {0}", this.serverdeviceid);
                    this.NotifyPropertyChanged(PropertyChangedDetails.ID);
                }
            }
        }

        /// <summary>
        /// called when a property changes so that subscribers can do stuff with this data
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// called when a property changes
        /// </summary>
        /// <param name="p"></param>
        /// <param name="p_2"></param>
        private void NotifyPropertyChanged(string propertyName)
        {
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// called by SIMPL by default
        /// </summary>
        public QuartzServerRouter()
        {
            //add ourselves to the list of registered devices
            QuartzProtocol.RegisteredRouters.Add(this);
        }

        void DebugMessage(string msg, params object[] args)
        {
            string debugMsg = string.Format(msg, args);
            QuartzProtocol.DebugMessage("QuartzRouter // ID: {0} | {1}", this.ServerDeviceID, debugMsg);
        }
    }
}