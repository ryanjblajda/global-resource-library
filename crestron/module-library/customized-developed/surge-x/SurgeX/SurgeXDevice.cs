using System;
using System.Text;
using Crestron.SimplSharp;                          				// For Basic SIMPL# Classes

namespace SurgeX
{
    public class SurgeXDevice
    {
        public string ModelName { get; private set; }
        public string Serial { get; private set; }
        public string ActiveState { get; private set; }
        public string MAC { get; private set; }
        public ushort ActiveUsers { get; private set; }
        public ushort ShutdownRequests { get; private set; }
        public string FirmwareVersion { get; private set; }
        public DateTime CurrentSystemTime { get; private set; }
        public ushort AutoLogoutTime { get; private set; }
        public string TemperatureUnits { get; private set; }
        
        /// <summary>
        /// default constructor
        /// </summary>
        public SurgeXDevice()
        {
        }
    }
}
