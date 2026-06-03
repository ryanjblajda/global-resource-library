using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Newtonsoft.Json;
using Crestron.SimplSharp.Net.Http;
using Crestron.SimplSharp.Net;

namespace MersiveSolstice
{
    public class MDisplayInformation
    {
        public string m_hostName { get; set; }
        public string m_ipv4 { get; set; }
        public string m_displayName { get; set; }
        public ushort m_port { get; set; }
    }

    public class MGeneralCuration
    {
        public string language { get; set; }
        public bool showSplashScreen { get; set; }
        public bool localConfigEnabled { get; set; }
        public bool browserConfigEnabled { get; set; }
        public bool autoConnectOnClientLaunch { get; set; }
        public bool autoSDSOnClientLaunch { get; set; }
        public string minimumClientVersionOverride { get; set; }
    }

    public class MAuthenticationCuration
    {
        public ushort screenKeyIsEnabled { get; set; }
        public ushort moderatorApprovalIsDisabled { get; set; }

        public bool screenKeyEnabled
        {
            set
            {
                if (value == true)
                {
                    screenKeyIsEnabled = 1;
                }
                else
                {
                    screenKeyIsEnabled = 0;
                }
            }
        }

        public bool moderatorApprovalDisabled 
        {
            set
            {
                if (value == true)
                {
                    moderatorApprovalIsDisabled = 1;
                }
                else
                {
                    moderatorApprovalIsDisabled = 0;
                }
            }
        }

        public string sessionKey { get; set; }
    }

    public class WifiConfig
    {
        public string ssid { get; set; }
        public int security { get; set; }
        public string identity { get; set; }
        public int eap { get; set; }
        public int phase2 { get; set; }
        public string password { get; set; }
        public ushort DHCP_Enabled { get; set; }
        public bool dhcp 
        {
            set
            {
                if (value == true)
                {
                    DHCP_Enabled = 1;
                }
                else
                {
                    DHCP_Enabled = 0;
                }
            }
        }
        public string staticIP { get; set; }
        public string gateway { get; set; }
        public int prefixLength { get; set; }
        public string dns1 { get; set; }
        public string dns2 { get; set; }
    }

    public class ApConfig
    {
        public string SSID { get; set; }
        public int SecurityMode { get; set; }
        public string PSK { get; set; }
        public bool hidden { get; set; }
        public int channel { get; set; }
    }

    public class Ethernet
    {
        public ushort DHCP_Enabled { get; set; }
        public bool dhcp 
        {
            set
            {
                if (value == true)
                {
                    DHCP_Enabled = 1;
                }
                else
                {
                    DHCP_Enabled = 0;
                }
            }
        }

        public string staticIP { get; set; }
        public string gateway { get; set; }
        public int prefixLength { get; set; }
        public string dns1 { get; set; }
        public string dns2 { get; set; }
    }

    public class HttpProxyServerSettings
    {
        public bool enabled { get; set; }
        public string ip { get; set; }
        public int port { get; set; }
        public string username { get; set; }
        public string password { get; set; }
    }

    public class HttpsProxyServerSettings
    {
        public bool enabled { get; set; }
        public string ip { get; set; }
        public int port { get; set; }
        public string username { get; set; }
        public string password { get; set; }
    }

    public class MRssFeedList
    {
        public bool enabled { get; set; }
        public string name { get; set; }
        public int length { get; set; }
        public string uri { get; set; }
    }

    public class MNetworkCuration
    {
        public long connectionShowFlags { get; set; }
        public bool discoveryBroadcastEnabled { get; set; }
        public int maximumConnections { get; set; }
        public long maximumLicensedConnections { get; set; }
        public int maximumImageSize { get; set; }
        public int maximumPublished { get; set; }
        public int maximumAirPlayUsers { get; set; }
        public bool publishToNameServer { get; set; }
        public string sdsHostName { get; set; }
        public string sdsHostName2 { get; set; }
        public int remoteViewMode { get; set; }
        public int firewallMode { get; set; }
        public bool postTypeDesktopSupported { get; set; }
        public bool postTypeApplicationWindowSupported { get; set; }
        public bool postTypeMediaFilesSupported { get; set; }
        public bool postTypeAirPlaySupported { get; set; }
        public bool postTypeAndroidMirroringSupported { get; set; }
        public bool bonjourProxyEnabled { get; set; }
        public int wifiMode { get; set; }
        public WifiConfig wifiConfig { get; set; }
        public ApConfig apConfig { get; set; }
        public bool ethernetEnabled { get; set; }
        public bool ethernetGatewayCheckEnabled { get; set; }
        public Ethernet ethernet { get; set; }
        public HttpProxyServerSettings httpProxyServerSettings { get; set; }
        public HttpsProxyServerSettings httpsProxyServerSettings { get; set; }
        public bool bulletinEnabled { get; set; }
        public string bulletinText { get; set; }
        public ushort emergencyIsEnabled { get; set; }
        public bool emergencyEnabled 
        {
            set
            {
                if (value == true)
                {
                    emergencyIsEnabled = 1;
                }
                else
                {
                    emergencyIsEnabled = 0;
                }
            }
        }
        public string emergencyText { get; set; }
        public IList<MRssFeedList> m_rssFeedList { get; set; }
        public int splashScreenMode { get; set; }
    }

    public class MLicenseCuration
    {
        public int licenseStatus { get; set; }
        public int trustFlags { get; set; }
        public string fulfillmentType { get; set; }
        public bool enabled { get; set; }
        public string fulfillmentId { get; set; }
        public string entitlementId { get; set; }
        public string productId { get; set; }
        public string suiteId { get; set; }
        public string expirationDate { get; set; }
        public string featureLine { get; set; }
        public int numDaysToExpiration { get; set; }
        public string maxUsers { get; set; }
        public int licensing_maxPosts { get; set; }
        public bool licensing_maxPostsIsConfigurable { get; set; }
        public bool licensing_atMaxPostsReplace { get; set; }
        public int licensing_maxUsers { get; set; }
        public bool licensing_maxUsersIsConfigurable { get; set; }
        public bool licensing_remoteViewEnabled { get; set; }
        public bool licensing_remoteViewIsConfigurable { get; set; }
        public bool licensing_runtimeAccessControls { get; set; }
    }

    public class MUserGroupCuration
    {
        public string adminPassword { get; set; }
        public int presenterPasswordLength { get; set; }
        public string presenterPassword { get; set; }
        public bool passwordValidationEnabled { get; set; }
    }

    public class TimeZone
    {
        public string id { get; set; }
        public string name { get; set; }
        public int offset { get; set; }
    }

    public class MSystemCuration
    {
        public ushort automaticDateTime { get; set; }
        public bool autoDateTime 
        {
            set
            {
                if (value == true)
                {
                    automaticDateTime = 1;
                }
                else
                {
                    automaticDateTime = 0;
                }
            }
        }
        public string ntpServer { get; set; }
        public long dateTime { get; set; }
        public string timeZone { get; set; }
        public IList<TimeZone> timeZones { get; set; }
        public bool l24HourTime { get; set; }
        public string resolution { get; set; }
    }

    public class Solstice
    {
        public string m_displayId { get; set; }
        public string m_serverVersion { get; set; }
        public string m_productName { get; set; }
        public string m_productVariant { get; set; }
        public int m_productHardwareVersion { get; set; }
        public MDisplayInformation m_displayInformation { get; set; }
        public MGeneralCuration m_generalCuration { get; set; }
        public MAuthenticationCuration m_authenticationCuration { get; set; }
        public MNetworkCuration m_networkCuration { get; set; }
        public MLicenseCuration m_licenseCuration { get; set; }
        public MUserGroupCuration m_userGroupCuration { get; set; }
        public MSystemCuration m_systemCuration { get; set; }

        internal string apiConfig = "/api/config";
        internal string httpPrefix = "http://";
        public string IPAddress;
        public string Password;
        private int Interval;
        private CTimer PollTimer;

        public event EventHandler<CustomEventArgs> onUpdate;
        public event EventHandler<CustomExceptionArgs> onError;

        CrestronQueue<string> commandQueue = new CrestronQueue<string>();

        public Solstice()
        {
            this.PollTimer = new CTimer(this.OnPollTimerExpired, Timeout.Infinite);
        }

        public void Initialize(string _ip, string _password)
        {
            this.IPAddress = _ip;
            this.Password = _password;
        }

        public void OnPollTimerExpired(object sender)
        {
            this.SendRequest(this.apiConfig, "");
        }

        public void EnablePolling(ushort interval)
        {
            //turn interval into seconds
            this.Interval = interval * 1000;
            //reset the timer immediately, then repeat after interval time
            this.PollTimer.Reset(0, this.Interval);
        }

        public void DisablePolling()
        {
            this.PollTimer.Stop();
        }

        public void AddItemToQueue(string _command)
        {
            try
            {
                commandQueue.TryToEnqueue(_command);
                CrestronConsole.PrintLine("Item Added To Queue");
            }
            catch (Exception)
            {
                CrestronConsole.PrintLine("Queue Full");
            }
        }

        private void onAddItem()
        {
            while (commandQueue.Count > 0)
            {
                try
                {
                    string item = commandQueue.TryToDequeue();
                    CrestronConsole.PrintLine("Item Removed From Queue");
                    SendRequest(apiConfig, item);
                }
                catch
                {
                    CrestronConsole.PrintLine("Error Removing Object From Queue");
                }
            }
        }

        public void SendRequest(string apiString, string _commandString)
        {
            HttpClient device = new HttpClient();
            HttpClientRequest request = new HttpClientRequest();

            try
            {
                string requestURL = httpPrefix + this.IPAddress + apiString;

                //if there is a password, add it to the request string
                if (this.Password != null)
                {
                    requestURL = requestURL + "?" + this.Password;
                }

                //if there is a command, add it...and make the request type POST
                if (_commandString != "")
                {
                    request.RequestType = RequestType.Post;

                    request.Header.ContentType = "application/json";

                    request.ContentSource = ContentSource.ContentString;
                    request.ContentString = _commandString;

                    CrestronConsole.PrintLine(_commandString);
                }
                else
                {
                    request.RequestType = RequestType.Get;
                }

                request.Url = new UrlParser(requestURL);
            }
            catch (UrlParserException)
            {
                onError(new UrlParserException(), new CustomExceptionArgs());
            }

            if (request.RequestType == RequestType.Get)
            {
                //CrestronConsole.PrintLine("Request is Get");

                HTTPClientResponseCallback whenComplete = new HTTPClientResponseCallback(onGetResponse);
                request.OnTransferEnd += this.onPostResponse;
                device.DispatchAsync(request, whenComplete);
            }
            else if (request.RequestType == RequestType.Post)
            {
                //CrestronConsole.PrintLine("Request is Post");

                HTTPClientResponseCallback whenComplete = new HTTPClientResponseCallback(onGetResponse);
                request.OnTransferEnd += this.onPostResponse;

                device.DispatchAsync(request, whenComplete);
            }
            else
            {
            }
            _commandString = "";
        }

        public void onPostResponse(object _sender, TransferEndEventArgs _args)
        {
            CrestronConsole.PrintLine("Getting New Status After Sending Command");
            SendRequest(apiConfig, "");
        }

        public void onGetResponse(HttpClientResponse _object, HTTP_CALLBACK_ERROR _errors)
        {
            try
            {
                Solstice response = JsonConvert.DeserializeObject<Solstice>(_object.ContentString);

                this.m_displayId = response.m_displayId;
                this.m_serverVersion = response.m_serverVersion;
                this.m_productName = response.m_productName;
                this.m_productVariant = response.m_productVariant;
                this.m_productHardwareVersion = response.m_productHardwareVersion;
                this.m_displayInformation = response.m_displayInformation;
                this.m_generalCuration = response.m_generalCuration;
                this.m_authenticationCuration = response.m_authenticationCuration;
                this.m_networkCuration = response.m_networkCuration;
                this.m_licenseCuration = response.m_licenseCuration;
                this.m_userGroupCuration = response.m_userGroupCuration;
                this.m_systemCuration = response.m_systemCuration;

                if (response != null)
                {
                    onUpdate(response, new CustomEventArgs());
                }
                else
                {
                    CrestronConsole.PrintLine("Response Is Null");
                }
            }
            catch (Exception)
            {
                //CrestronConsole.PrintLine("Error Deserializing Response");
            }
        }
    }
}