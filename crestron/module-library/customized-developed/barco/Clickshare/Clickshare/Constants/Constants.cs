using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace Clickshare.Constants
{
    public class Result
    {
        public const string Ok = "ok";
        public const string Error = "error";
        public const string VerificationError = "verficationError";
    }

    public class Language
    {
        public const string English = "en";
        public const string Chinese = "zh";
    }

    public class URI
    {
        //api
        private const string API = "/configuration";

        //system control
        public const string SystemInformation = "";
    }

    public static class Service
    {
        internal const string Audio = "/audio";
        internal const string Buttons = "/buttons";
        internal const string Https = "/https";
        internal const string Features = "/features";
        internal const string Configurator = "/configurator";
        internal const string System = "/system";
    }

    public static class ServiceEndpoints
    {
        //https
        public const string CustomCertificates = "/custom-certificates";
        public const string CustomSigningRequests = "/custom-signing-requests";
            //custom certs
            public const string LastUpload = "/last-upload";
            //status
            public const string Status = "/status";
        
        //input cards
        public const string InputCards = "/input-cards";
        
        //peripherals
        public const string Peripherals = "/peripherals";
            //updates
            public const string Updates = "/updates";
        
        //personalization
        public const string Personalization = "/personalization";
       
        //security
        public const string Security = "/security";

        //system 
        public const string DeviceIdentity = "/device-identity";
        public const string Logging = "/logging";
        public const string Network = "/network";
            //wired
            public const string Wired = "/wired";
            //wireless
            public const string Wireless = "/wireless";
                //wireless client
                public const string WirelessClient = "/wireless-client";
        public const string PowerManagement = "/power-management";
        public const string Software = "/software";
            //software
            public const string Available = "/available";
                //latest
                public const string Latest = "/latest";
        public const string Status = "/status";
        public const string Time = "/time";
        public const string UpdateSettings = "
        
        //buttons
        public const string ConnectionMethod = "/connection-method";
        public const string NetworkIntegration = "/network-integration";
            //ident methods
            public const string IdentificationMethod = "/identification-method";
        
        //features
        public const string Airplay = "/airplay";
        public const string Blackboard = "/blackboard";
        public const string ClickshareApp = "/clickshare-app";
        public const string DigitalSignage = "/digital-signage";
        public const string FlexibleRoomSystems = "/flexible-room-systems";
        public const string GoogleCast = "/google-cast";
        public const string LocalView = "/local-view";
        public const string Miracast = "/miracast";
        public const string RemotePairing = "/remote-pairing";
        public const string SmartFocus = "/smart-focus";
        public const string SNMP = "/snmp";
        public const string Ultrasound = "/ultrasound";
        public const string WiredRoomDock = "/wired-roomdock";
    }

    public static class Methods
    {
        public const string Authentication = "/authentication";
        public const string Authentication802dot1x = "/802dot1xAuthentication";
        public const string SCEP = "/scep";
        public const string Enrollments = "/entrollments";
    }
}