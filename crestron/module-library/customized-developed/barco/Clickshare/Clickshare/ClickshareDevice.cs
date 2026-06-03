using System;
using System.Text;
using Crestron.SimplSharp; // For Basic SIMPL# Classes
using Crestron.SimplSharp.Net.Https;
using Crestron.SimplSharp.CrestronXmlLinq;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Crestron.SimplSharp.CrestronXml.Serialization;
using Crestron.SimplSharp.CrestronSockets;
using System.Text.RegularExpressions;
using SIMPL;
using Crestron.SimplSharp.Net.Http;
using Clickshare.Constants;
using Clickshare.API;
using System.ComponentModel;

namespace Clickshare
{
    public class ClickshareDevice
    {
        private BindingList<Request> requestQueue = new BindingList<Request>();

        private CTimer PollTimer;
        
        private bool IsDebug;
        private bool IsDispatchInProgress;
        
        private HttpsClient Client = new HttpsClient();

        private SystemInformation SystemInfo = new SystemInformation();
        private LoginInformation LoginInfo = new LoginInformation();
        private List<DecodingStreamInformation> AvailableStreams = new List<DecodingStreamInformation>();

        private ushort requestedStreamID = 0;
       
        private string host = String.Empty;
        /// <summary>
        /// the host or ip address of the kiloview device
        /// </summary>
        public string Host 
        {
            get { return this.host; }
            set
            {
                if (value != null)
                {
                    this.host = value;
                    if (this.IsDebug) CrestronConsole.PrintLine("CLICKSHARE {0} @ {1} | Host: {2}", this.SystemInfo.VersionInformation.Product, this.Host, this.host);
                }
            }
        }
        private ushort pollInterval = 500;
        /// <summary>
        /// the polling interval in seconds to keep device details up to date
        /// </summary>
        public ushort PollInterval
        {
            get { return pollInterval; }
            set { this.pollInterval = (ushort)(value * 10); }
        }

        public string Username = String.Empty;
        public string Password = String.Empty;

        private bool isLoggedIn = false;
        public bool IsLoggedIn
        {
            get { return this.isLoggedIn; }
            set
            {
                bool login = this.isLoggedIn;
                this.isLoggedIn = value;
                if (this.IsDebug) CrestronConsole.PrintLine("CLICKSHARE {0} @ {1} | Logged In: {2}", this.SystemInfo.VersionInformation.Product, this.Host, this.isLoggedIn);
                //fire event
                if (login != value) { if (this.IsLoggedInChanged != null) { this.IsLoggedInChanged(this, new DigitalPayloadArgs(this.isLoggedIn)); } }
                //set polling enable based on login status
                this.Poll(Conversion.ConvertToSignal(this.isLoggedIn));
            }
        }

        private bool isError = false;
        public bool IsError
        {
            get { return this.isError; }
            set
            {
                bool error = this.isError;
                this.isError = value;
                if (this.IsDebug) CrestronConsole.PrintLine("CLICKSHARE {0} @ {1} | Error: {2}", this.SystemInfo.VersionInformation.Product, this.Host, this.isError);
                //fire event
                if (error != value) { if (this.IsErrorChanged != null) { this.IsErrorChanged(this, new DigitalPayloadArgs(this.isError)); } }
            }
        }

        private string errorMessage = String.Empty;
        public string ErrorMessage
        {
            get { return this.errorMessage; }
            set
            {
                string error = this.errorMessage;
                this.errorMessage = value;
                if (this.IsDebug) CrestronConsole.PrintLine("CLICKSHARE {0} @ {1} | Error Message: {2}", this.SystemInfo.VersionInformation.Product, this.Host, this.errorMessage);
                //fire event
                if (error != value) { if (this.ErrorMessageChanged != null) { this.ErrorMessageChanged(this, new StringPayloadArgs(this.errorMessage)); } }
            }
        }

        private ushort actualStreamID = 0;
        public ushort ActualStreamID 
        {
            get { return this.actualStreamID; }
            set 
            {
                ushort id = this.actualStreamID;
                this.actualStreamID = value;
                if (this.IsDebug) CrestronConsole.PrintLine("CLICKSHARE {0} @ {1} | Actual Stream Preset ID: {2}", this.SystemInfo.VersionInformation.Product, this.Host, this.actualStreamID);
                //fire event
                if (id != value) { if (this.ActualStreamIDChanged != null) { this.ActualStreamIDChanged(this, new AnalogPayloadArgs(this.actualStreamID)); } }
            }
        }

        private string actualStreamName = String.Empty;
        public string ActualStreamName
        {
            get { return this.actualStreamName; }
            set
            {
                string name = this.actualStreamName;
                this.actualStreamName = value;
                if (this.IsDebug) CrestronConsole.PrintLine("CLICKSHARE {0} @ {1} | Actual Stream Name: {2}", this.SystemInfo.VersionInformation.Product, this.Host, this.actualStreamName);
                //fire event
                if (name != value) { if (this.ActualStreamNameChanged != null) { this.ActualStreamNameChanged(this, new StringPayloadArgs(this.actualStreamName)); } }
            }
        }

        private string actualStreamIP = String.Empty;
        public string ActualStreamIP
        {
            get { return this.actualStreamIP; }
            set
            {
                string ip = this.actualStreamIP;
                this.actualStreamIP = value;
                if (this.IsDebug) CrestronConsole.PrintLine("CLICKSHARE {0} @ {1} | Actual Stream IP: {2}", this.SystemInfo.VersionInformation.Product, this.Host, this.actualStreamIP); 
                //fire event
                if (ip != value) { if (this.ActualStreamIPChanged != null) { this.ActualStreamIPChanged(this, new StringPayloadArgs(this.actualStreamIP)); } }
            }
        }

        private string actualStreamURL = String.Empty;
        public string ActualStreamURL
        {
            get { return this.actualStreamURL; }
            set
            {
                string url = this.actualStreamURL;
                this.actualStreamURL = value;
                if (this.IsDebug) CrestronConsole.PrintLine("CLICKSHARE {0} @ {1} | Actual Stream URL: {2}", this.SystemInfo.VersionInformation.Product, this.Host, this.actualStreamURL);
                //fire event
                if (url != value) { if (this.ActualStreamURLChanged != null) { this.ActualStreamURLChanged(this, new StringPayloadArgs(this.actualStreamURL)); } }
            }
        }

        public event EventHandler<StringPayloadArgs> ErrorMessageChanged;
        public event EventHandler<DigitalPayloadArgs> IsLoggedInChanged;
        public event EventHandler<DigitalPayloadArgs> IsErrorChanged;
        public event EventHandler<AnalogPayloadArgs> ActualStreamIDChanged;
        public event EventHandler<StringPayloadArgs> ActualStreamNameChanged;
        public event EventHandler<StringPayloadArgs> ActualStreamIPChanged;
        public event EventHandler<StringPayloadArgs> ActualStreamURLChanged;

        /// <summary>
        /// default constructor for SIMPL+
        /// </summary>
        public ClickshareDevice()
        {
            this.requestQueue.ListChanged += this.OnRequestQueueItemAdded;
            this.PollTimer = new CTimer(this.OnPollTimerExpired, Timeout.Infinite);
            this.Client.KeepAlive = false;
            this.Client.PeerVerification = false;
            this.Client.HostVerification = false;
        }

        private Request PopRequestFromQueue(int id)
        {
            Request request = null;

            if (id < this.requestQueue.Count)
            {
                //pull the request out
                request = this.requestQueue[id];
                //remove the request
                this.requestQueue.RemoveAt(id);
                if (this.IsDebug) CrestronConsole.PrintLine("CLICKSHARE {0} @ {1} | Poppped Item @ {2} From Queue -> {3}", this.SystemInfo.VersionInformation.Product, this.Host, id, request.Method);
            }
            else
            {
                if (this.IsDebug) { CrestronConsole.PrintLine("CLICKSHARE {0} @ {1} | Request Index Invalid -> {2} > {3}", this.SystemInfo.VersionInformation.Product, this.Host, id, this.requestQueue.Count); }
            }

            return request;
        }

        private void OnRequestQueueItemAdded(object sender, ListChangedEventArgs args)
        {
            //only dispatch the request if 
            if (args.ListChangedType == ListChangedType.ItemAdded)
            {
                try
                {
                    this.DispatchRequest();
                }
                catch (Exception e)
                {
                    if (this.IsDebug) { CrestronConsole.PrintLine("CLICKSHARE {0} @ {1} | Exception Encountered Popping Request From Queue: {2}", this.SystemInfo.VersionInformation.Product, this.Host, e.Message); }
                }
            }  
        }

        /// <summary>
        /// callback timer called when the poll timer expires to begin requesting current details from the device
        /// </summary>
        /// <param name="sender">the object the fired the callback</param>
        private void OnPollTimerExpired(object sender)
        {
            if (this.Host != String.Empty)
            {
                if (this.IsDebug) CrestronConsole.PrintLine("CLICKSHARE {0} @ {1} | Poll Timer Expired!", this.SystemInfo.VersionInformation.Product, this.Host);
                
                this.GetSystemInformation();
                this.GetCurrentDecodingStream();
                this.GetDecodingStreamPresetList();
            }
            else { if (this.IsDebug) CrestronConsole.PrintLine("CLICKSHARE {0} @ {1} | IP Address Invalid!", this.SystemInfo.VersionInformation.Product, this.Host); }
        }

        /// <summary>
        /// a callback fired when an HTTP response is received
        /// </summary>
        /// <param name="response"></param>
        /// <param name="err"></param>
        /// <param name="obj"></param>
        private void OnResponse(HttpsClientResponse response, HTTPS_CALLBACK_ERROR err, object obj)
        {
            if (err != HTTPS_CALLBACK_ERROR.COMPLETED)
            {
                if (this.IsDebug) CrestronConsole.PrintLine("CLICKSHARE {0} @ {1} | HTTPS_CALLBACK_ERROR: {2}", this.SystemInfo.VersionInformation.Product, this.Host, err.ToString());
                this.IsLoggedIn = false;
            }

            if (response != null)
            {
                if (response.Code != 200) 
                { 
                    CrestronConsole.PrintLine("CLICKSHARE {0} @ {1} | HTTP CODE: {2}", this.SystemInfo.VersionInformation.Product, this.Host, response.Code);
                    this.IsLoggedIn = false;
                }

                if (response.HasContentLength)
                {
                    string method = (string)obj;

                    if (this.IsDebug) CrestronConsole.PrintLine("CLICKSHARE {0} @ {1} Received -> {2}", this.SystemInfo.VersionInformation.Product, this.Host, response.ContentString);
                    
                    try { HandleResponse(response, method); }
                    catch (Exception e)
                    {
                        if (this.IsDebug) CrestronConsole.PrintLine("CLICKSHARE {0} @ {1} | Exception Encountered Handling Response -> {2}", this.SystemInfo.VersionInformation.Product, this.Host, method, e.Message);
                    }
                }
            }

            //dispatching should no longer be in progress once a response has been received
            lock (this.requestQueue) 
            {
                this.IsDispatchInProgress = false; 
                if (this.IsDebug) CrestronConsole.PrintLine("CLICKSHARE {0} @ {1} | Acquire Lock On Request Queue", this.SystemInfo.VersionInformation.Product, this.Host);
                if (this.IsDebug) CrestronConsole.PrintLine("CLICKSHARE {0} @ {1} | Dispatch In Progress -> {2}", this.SystemInfo.VersionInformation.Product, this.Host, this.IsDispatchInProgress);
            }

            if (this.IsDebug) CrestronConsole.PrintLine("CLICKSHARE {0} @ {1} | Release Lock On Request Queue", this.SystemInfo.VersionInformation.Product, this.Host);
            
            //dispatch requests if needed
            this.DispatchRequest();
        }

        private void HandleResponse(HttpsClientResponse response, string method)
        {
            Response incoming = null;

            try
            {
                 incoming = JsonConvert.DeserializeObject<Response>(response.ContentString);
            }
            catch (Exception e)
            {
                if (this.IsDebug) { CrestronConsole.PrintLine("CLICKSHARE {0} @ {1} | Exception Encountered Deserializing Response: {2}", this.SystemInfo.VersionInformation.Product, this.Host, method, e.Message); }
            }
            
            if (incoming != null)
            {
                this.ErrorMessage = incoming.Message;

                if (incoming.Result == Constants.Result.Ok)
                {
                    this.IsError = false;
                    HandleSuccessResponse(incoming, method);
                }
                else if (incoming.Result == Constants.Result.Error)
                {
                    this.IsError = true;
                }
                else if (incoming.Result == Constants.Result.VerificationError)
                {
                    this.IsError = true;
                }
            }
        }

        private void HandleSuccessResponse(Response response, string method)
        {
            switch (method)
            {
                case Methods.Login:
                    LoginResponse(response.Data);
                    break;
                case Methods.Session:
                    SessionResponse(response.Result);
                    break;
                case Methods.SetDecodingPreset:
                    SetDecodingPresetResponse(response.Result);
                    break;
                case Methods.GetSystemInformation:
                    SystemInformationResponse(response.Data);
                    break;
                case Methods.GetDecodingStreamPresetListInformation:
                    DecodingStreamPresetListInformationResponse(response.Data);
                    break;
                case Methods.GetDecodingStreamInformation:
                    DecodingStreamInformationResponse(response.Data);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// handles when a successful system info response is received
        /// </summary>
        /// <param name="data">the data provided by the device</param>
        private void SystemInformationResponse(JToken data)
        {
            try
            {
                SystemInformation info = data.ToObject<SystemInformation>();
                this.SystemInfo = info;
            }
            catch (Exception e)
            {
                CrestronConsole.PrintLine("CLICKSHARE {0} @ {1} | Exception Encountered Converting Data: {2}", this.SystemInfo.VersionInformation.Product, this.Host, e.Message);
            }
        }

        /// <summary>
        /// handles when a successful preset list info response is received
        /// </summary>
        /// <param name="data">the data provided by the device</param>
        private void DecodingStreamPresetListInformationResponse(JToken data)
        {
            try
            {
                List<DecodingStreamPresetInformation> list = data.ToObject<List<DecodingStreamPresetInformation>>();

                list.ForEach(stream =>
                {
                    if (stream.Name == this.ActualStreamName) 
                    { 
                        this.ActualStreamID = (ushort)stream.ID;
                        CrestronConsole.PrintLine("CLICKSHARE {0} @ {1} | Found Matching Name: {2} == {3} (Current Stream) -> Updating Actual Stream ID: {4}", this.SystemInfo.VersionInformation.Product, this.Host, stream.Name, this.ActualStreamName, this.ActualStreamID);
                    }
                });
            }
            catch (Exception e)
            {
                CrestronConsole.PrintLine("CLICKSHARE {0} @ {1} | Exception Encountered Converting Data: {2}", this.SystemInfo.VersionInformation.Product, this.Host, e.Message);
            }
        }

        /// <summary>
        /// handles when a successful decoding stream response is received
        /// </summary>
        /// <param name="data">the data provided by the device</param>
        private void DecodingStreamInformationResponse(JToken data)
        {
            try
            {
                DecodingStreamInformation stream = data.ToObject<DecodingStreamInformation>();
                this.ActualStreamName = stream.Name;
                this.ActualStreamIP = stream.IPAddress;
                this.ActualStreamURL = stream.URL;
            }
            catch (Exception e)
            {
                CrestronConsole.PrintLine("CLICKSHARE {0} @ {1} | Exception Encountered Converting Data: {2}", this.SystemInfo.VersionInformation.Product, this.Host, e.Message);
            }
        }

        /// <summary>
        /// handles when a successful decoding preset recall response is received
        /// </summary>
        /// <param name="result">the result of the operation</param>
        private void SetDecodingPresetResponse(string result)
        {
            if (result == Clickshare.Constants.Result.Ok) {

                if (this.IsDebug) CrestronConsole.PrintLine("CLICKSHARE {0} @ {1} | Set Decoding Preset Response: {2}", this.SystemInfo.VersionInformation.Product, this.Host, result);
                this.ActualStreamID = this.requestedStreamID; 
            }
        }
    
        /// <summary>
        /// handles a successful login response
        /// </summary>
        /// <param name="data">the data provided by the device</param>
        private void LoginResponse(JToken data)
        {
            try
            {
                LoginInformation details = data.ToObject<LoginInformation>();
                this.LoginInfo = details;
                if (this.IsDebug) { CrestronConsole.PrintLine("CLICKSHARE {0} @ {1} | Token: {2} // Alias: {3} // IsChange: {4}", this.SystemInfo.VersionInformation.Product, this.Host, details.Token, details.LoginAlias, details.IsLoginChanged); }
                //send a session request
                if (details.Token != String.Empty) { this.SendSessionRequest(this.Username, this.Password, details.LoginAlias, details.Token); }
            }
            catch (Exception e)
            {
                CrestronConsole.PrintLine("CLICKSHARE {0} @ {1} | Exception Encountered Converting Data: {2}", this.SystemInfo.VersionInformation.Product, this.Host, e.Message);
            }
        }

        /// <summary>
        /// handles a successful session response
        /// </summary>
        /// <param name="result">the result of the operation</param>
        private void SessionResponse(string result)
        {
            if (result == Constants.Result.Ok) { this.IsLoggedIn = true; }
        }

        /// <summary>
        /// sends an HTTPS request to the device
        /// </summary>
        /// <param name="service">the main uri to send the request to</param>
        /// <param name="method">the method for the uri</param>
        /// <param name="type">the request type</param>
        /// <param name="body">the request body json content</param>
        private void GenerateRequestJsonBody(string service, string method, Crestron.SimplSharp.Net.Https.RequestType type, List<HttpsHeader> headers, string body)
        {
            if (this.Host != String.Empty)
            {
                HttpsClientRequest request = new HttpsClientRequest();
                request.RequestType = type;

                //add the body if it exists
                if (body.Length != 0 && type != Crestron.SimplSharp.Net.Https.RequestType.Get) 
                { 
                    request.Header.ContentType = "application/json";
                    request.ContentString = body;
                }
                
                headers.ForEach(h => request.Header.AddHeader(h));

                request.Url = new UrlParser(String.Format("https://{0}{1}", this.Host, service));

                this.QueueRequest(request, method);
            }
            else { if (this.IsDebug) CrestronConsole.PrintLine("CLICKSHARE {0} @ {1} | IP ADDR INVALID", this.SystemInfo.VersionInformation.Product, this.Host); }
        }

        /// <summary>
        /// sends a request with URL parameters
        /// </summary>
        /// <param name="service">the main uri to send the request to</param>
        /// <param name="method">the method for uri</param>
        /// <param name="type">the request type</param>
        /// <param name="headers">the request headers to add</param>
        /// <param name="info">a dictionary with what parameters you wish to retrieve</param>
        private void GenerateRequestWithParams(string service, string method, Crestron.SimplSharp.Net.Https.RequestType type, List<HttpsHeader> headers, Dictionary<string, bool> info)
        {
            if (this.Host != String.Empty)
            {
                HttpsClientRequest request = new HttpsClientRequest();
                request.RequestType = type;

                headers.ForEach(h => request.Header.AddHeader(h));

                string param = String.Join("&", info.Select(kv => String.Format("{0}={1}", kv.Key, kv.Value)).ToArray());
                request.Url = new UrlParser(String.Format("https://{0}{1}?{2}", this.Host, service, param));

                this.QueueRequest(request, method);
            }
            else { if (this.IsDebug) CrestronConsole.PrintLine("CLICKSHARE {0} @ {1} | IP ADDR INVALID", this.SystemInfo.VersionInformation.Product, this.Host); }
        }

        private void QueueRequest(HttpsClientRequest request, string method)
        {
            if (this.IsDebug) CrestronConsole.PrintLine("CLICKSHARE {0} @ {1} | Add Request To Queue -> {2}", this.SystemInfo.VersionInformation.Product, this.Host, method);
            lock (this.requestQueue)
            {
                if (this.IsDebug) CrestronConsole.PrintLine("CLICKSHARE {0} @ {1} | Aqcuire Lock On Request Queue", this.SystemInfo.VersionInformation.Product, this.Host);
                this.requestQueue.Add(new Request(request, method));
            }
            if (this.IsDebug) CrestronConsole.PrintLine("CLICKSHARE {0} @ {1} | Release Lock On Request Queue", this.SystemInfo.VersionInformation.Product, this.Host);
        }

        /// <summary>
        /// dispatches the request to the device
        /// </summary>
        private void DispatchRequest()
        {
            lock (this.requestQueue)
            {
                if (this.IsDispatchInProgress == false)
                {
                    this.IsDispatchInProgress = true;
                    if (this.IsDebug) CrestronConsole.PrintLine("CLICKSHARE {0} @ {1} | Dispatch Request In Progress -> {2}", this.SystemInfo.VersionInformation.Product, this.Host, this.IsDispatchInProgress);

                    Request request = this.PopRequestFromQueue(0);

                    if (request != null)
                    {
                        try
                        {
                            if (this.IsDebug) CrestronConsole.PrintLine("CLICKSHARE {0} @ {1} | Dispatch Request -> {2}", this.SystemInfo.VersionInformation.Product, this.Host, request.WebRequest.Url);
                            this.Client.DispatchAsyncEx(request.WebRequest, this.OnResponse, request.Method);
                        }
                        catch (Exception e)
                        {
                            if (this.IsDebug) CrestronConsole.PrintLine("CLICKSHARE {0} @ {1} | Exception Encountered Dispatching Request -> {2} | {3}", this.SystemInfo.VersionInformation.Product, this.Host, request.WebRequest.Url, e.Message);

                            this.IsDispatchInProgress = false;
                            if (this.IsDebug) CrestronConsole.PrintLine("CLICKSHARE {0} @ {1} | Dispatch In Progress -> {2}", this.SystemInfo.VersionInformation.Product, this.Host, this.IsDispatchInProgress);
                        }
                    }
                    else 
                    {
                        if (this.IsDebug) CrestronConsole.PrintLine("CLICKSHARE {0} @ {1} | Unable To Obtain Request", this.SystemInfo.VersionInformation.Product, this.Host);
                        this.IsDispatchInProgress = false;
                        if (this.IsDebug) CrestronConsole.PrintLine("CLICKSHARE {0} @ {1} | Dispatch In Progress -> {2}", this.SystemInfo.VersionInformation.Product, this.Host, this.IsDispatchInProgress);
                    }
                }
            }
            if (this.IsDebug) CrestronConsole.PrintLine("CLICKSHARE {0} @ {1} | Release Lock On Request Queue", this.SystemInfo.VersionInformation.Product, this.Host);
        }

        /// <summary>
        /// enables debug printing
        /// </summary>
        /// <param name="state">whether debug is enabled or disabled</param>
        public void EnableDebug(ushort state)
        {
            this.IsDebug = Conversion.ConvertToBool(state);
        }

        /// <summary>
        /// sets the polling state
        /// </summary>
        /// <param name="state">enabled or disabled</param>
        public void Poll(ushort state)
        {
            if (state == 1)
            {
                this.PollTimer.Reset(0, this.PollInterval);
                return;
            }
            this.PollTimer.Stop();
        }

        /// <summary>
        /// sends a login request using set credentials
        /// typically called from SIMPL+
        /// </summary>
        public void SendLoginRequest()
        {
            this.SendLoginRequest(this.Username, this.Password);
        }

        /// <summary>
        /// sends a login request to the device
        /// </summary>
        /// <param name="user"></param>
        /// <param name="pass"></param>
        private void SendLoginRequest(string user, string pass)
        {
            string body = String.Empty;

            try 
            { 
                body = JsonConvert.SerializeObject(new LoginRequest(Constants.Language.English, user, pass));
                GenerateRequestJsonBody(Constants.URI.UserLogin, Constants.Methods.Login, Crestron.SimplSharp.Net.Https.RequestType.Post, new List<HttpsHeader>(), body);
            }
            catch (Exception e) 
            {
                CrestronConsole.PrintLine("CLICKSHARE {0} @ {1} | Exception Encountered | Serializing Login Request -> {2}", this.SystemInfo.VersionInformation.Product, this.Host, e.Message);
            }

        }

        /// <summary>
        /// sends a login session request to the device
        /// </summary>
        /// <param name="user">username to login as</param>
        /// <param name="pass">password for the account</param>
        /// <param name="alias">the user alias</param>
        /// <param name="token">the token provided by the device</param>
        private void SendSessionRequest(string user, string pass, string alias, string token)
        {
            string cookie = String.Empty;

            try 
            { 
                cookie = String.Format("language={0}; user={1}; alias={2}; token={3};", Constants.Language.English, user, alias, token);
                GenerateRequestJsonBody(Constants.URI.UserSession, Methods.Session, Crestron.SimplSharp.Net.Https.RequestType.Get, new List<HttpsHeader>() { new HttpsHeader("Cookie", cookie) }, "");
            }
            catch (Exception e) 
            {
                CrestronConsole.PrintLine("CLICKSHARE {0} @ {1} | Exception Encountered | Generating Cookie For Session Request -> {2}", this.SystemInfo.VersionInformation.Product, this.Host, e.Message);
            }
        }

        private void GetSystemInformation()
        {
            if (this.IsDebug) { CrestronConsole.PrintLine("CLICKSHARE {0} @ {1} | Sending Get System Information Request", this.SystemInfo.VersionInformation.Product, this.Host); }
            
            Dictionary<string, bool> items = new Dictionary<string, bool>() { 
                { SystemInformation.CPU, true }, 
                { SystemInformation.Disk, true }, 
                { SystemInformation.Memory, true }, 
                { SystemInformation.PersistentTime, true }, 
                { SystemInformation.Version, true } 
            };
            
            GenerateRequestWithParams(Constants.URI.SystemInformation, Methods.GetSystemInformation, Crestron.SimplSharp.Net.Https.RequestType.Get, new List<HttpsHeader>(), items);
        }

        private void GetCurrentDecodingStream()
        {
            if (this.IsDebug) { CrestronConsole.PrintLine("CLICKSHARE {0} @ {1} | Sending Get Decoding Stream Information Request", this.SystemInfo.VersionInformation.Product, this.Host); }
            GenerateRequestJsonBody(Constants.URI.CodecDecodingStreamInfo, Methods.GetDecodingStreamInformation, Crestron.SimplSharp.Net.Https.RequestType.Get, new List<HttpsHeader>(), "");
        }

        private void GetDecodingStreamPresetList()
        {
            if (this.IsDebug) { CrestronConsole.PrintLine("CLICKSHARE {0} @ {1} | Sending Get Decoding Stream Preset List Information Request", this.SystemInfo.VersionInformation.Product, this.Host); }
            GenerateRequestJsonBody(Constants.URI.CodecDecoderPresetListInfo, Methods.GetDecodingStreamPresetListInformation, Crestron.SimplSharp.Net.Https.RequestType.Get, new List<HttpsHeader>(), "");
        }

        public void RecallDecodingStreamPreset(ushort id)
        {
            this.requestedStreamID = id;

            try
            {
                string json = JsonConvert.SerializeObject(new DecodingPresetRequest(id));
                if (this.IsDebug) { CrestronConsole.PrintLine("CLICKSHARE {0} @ {1} | Sending Decoding Preset Request -> {2}", this.SystemInfo.VersionInformation.Product, this.Host, id); }
                GenerateRequestJsonBody(Constants.URI.CodecDecoderPresetSelect, Methods.SetDecodingPreset, Crestron.SimplSharp.Net.Https.RequestType.Post, new List<HttpsHeader>(), json);
            }
            catch (Exception e)
            {
                CrestronConsole.PrintLine("CLICKSHARE {0} @ {1} | Exception Encountered | Serializing Decoding Preset Request -> {2}", this.SystemInfo.VersionInformation.Product, this.Host, e.Message);
            }
        }
    }
}
