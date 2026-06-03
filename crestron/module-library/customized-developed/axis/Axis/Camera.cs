using System;
using System.Text;
using Crestron.SimplSharp;                          				// For Basic SIMPL# Classes
using Crestron.SimplSharp.Net.Http;

namespace Axis
{
    public class Camera
    {
        private string _ip;
        public string IPAddress
        {
            get { return _ip; }
            set 
            {
                if (value != null)
                {
                    //remove any http nonsense from the string
                    string parsed = value.Replace("https://", "");
                    parsed = parsed.Replace("http://", "");

                    //assign value to ip field
                    this._ip = parsed;
                }
                else { this._ip = "Not Set"; }
            }
        }

        public ushort PanTiltSpeed { get; set; }
        public ushort ZoomSpeed { get; set; }

        public delegate void DigitalPayloadEvent(object sender, DigitalPayloadEventArgs args);
        public event DigitalPayloadEvent Success;
        public event DigitalPayloadEvent Failure;

        public delegate void StringPayloadEvent(object sender, StringPayloadEventArgs args);
        public event StringPayloadEvent FailureMessage;

        /// <summary>
        /// constructor for the axis camera object
        /// </summary>
        public Camera()
        {
            this.PanTiltSpeed = 5;
            this.ZoomSpeed = 5;
            this.IPAddress = "Not Set";
        }

        internal void OnHttpResponse(HttpClientResponse response, HTTP_CALLBACK_ERROR err, object obj)
        {
            if (response != null)
            {

                try 
                {
                    Utilities.CameraAction action = (Utilities.CameraAction)obj;
                    CrestronConsole.PrintLine("Axis Camera @ {0} | HTTP_CALLBACK_ERR: {1} | PTZAction: {2} | HTTP Status: {3}", this.IPAddress, err, action, response.Code);
                }
                catch (Exception e) { CrestronConsole.PrintLine("Axis Camera @ {0} | Exception: {1}", this.IPAddress, e.StackTrace); }
                

                switch (response.Code)
                {
                    case 200: //error
                        CrestronConsole.PrintLine("Axis Camera @ {0} | Error Message: {1}", this.IPAddress, response.ContentString);
                        if (this.Failure != null)
                        {
                            this.Failure(this, new DigitalPayloadEventArgs(1));
                            this.Failure(this, new DigitalPayloadEventArgs(0));
                        }

                        if (this.FailureMessage != null)
                        {
                            this.FailureMessage(this, new StringPayloadEventArgs(response.ContentString));
                        }
                        break;
                    case 204: //success
                        if (this.Success != null)
                        {
                            this.Success(this, new DigitalPayloadEventArgs(1));
                            this.Success(this, new DigitalPayloadEventArgs(0));
                        }
                        break;
                    default:
                        CrestronConsole.PrintLine("Axis Camera @ {0} | Error Message: {1}", this.IPAddress, response.ContentString);
                        if (this.Failure != null)
                        {
                            this.Failure(this, new DigitalPayloadEventArgs(1));
                            this.Failure(this, new DigitalPayloadEventArgs(0));
                        }
                        if (this.FailureMessage != null)
                        {
                            this.FailureMessage(this, new StringPayloadEventArgs(response.ContentString));
                        }
                        break;
                }
            }
        }

        internal int GenerateActualPTZSpeed(ushort speed, Utilities.CameraAction action)
        {
            int actualSpeed = 1;

            switch (action)
            {
                case Utilities.CameraAction.Up:
                    actualSpeed = speed;
                    break;
                case Utilities.CameraAction.Left:
                    actualSpeed = -Math.Abs(speed);
                    break;
                case Utilities.CameraAction.Right:
                    actualSpeed = speed;
                    break;
                case Utilities.CameraAction.Down:
                    actualSpeed = -Math.Abs(speed);
                    break;
                case Utilities.CameraAction.In:
                    actualSpeed = speed;
                    break;
                case Utilities.CameraAction.Out:
                    actualSpeed = -Math.Abs(speed);
                    break;
            }

            return actualSpeed;
        }

        public void Home(ushort homeAction)
        {
            HttpClient client = new HttpClient();
            HttpClientRequest request = new HttpClientRequest();
            client.HostName = this.IPAddress;
            request.RequestType = RequestType.Get;
            request.Header.AddHeader(new HttpHeader("X-Requested-Auth", "Digest"));
            string commandURL = "";
            Utilities.CameraAction action = Utilities.CameraAction.Stop;

            if (homeAction == 0) 
            { 
                commandURL = Utilities.BaseURL + Utilities.PanTiltZoomService + Utilities.SaveHomeAction;
                action = Utilities.CameraAction.HomeSave;

                try { request.Url = new UrlParser(String.Format(commandURL, this.IPAddress)); }
                catch (Exception e) { CrestronConsole.PrintLine("Axis Camera @ {0} | Home Save: {2} | Exception: {1}", this.IPAddress, e.StackTrace, action); } 
            }
            else if (homeAction == 1) 
            { 
                commandURL = Utilities.BaseURL + Utilities.PanTiltZoomService + Utilities.MoveAction;
                action = Utilities.CameraAction.HomeGo;

                try { request.Url = new UrlParser(String.Format(commandURL, this.IPAddress, Utilities.Home)); }
                catch (Exception e) { CrestronConsole.PrintLine("Axis Camera @ {0} | Home Go: {2} | Exception: {1}", this.IPAddress, e.StackTrace, action); } 
            }

            try 
            {
                HttpClient.DISPATCHASYNC_ERROR err = client.DispatchAsyncEx(request, this.OnHttpResponse, action);
                CrestronConsole.PrintLine("Axis Camera @ {0} | DISPATCHASYNC_ERR: {1} | Move Action: {2}", this.IPAddress, err, Utilities.Home);
            }
            catch (Exception e) { CrestronConsole.PrintLine("Axis Camera @ {0} | Home Action: {2} | Exception: {1}", this.IPAddress, e.StackTrace, action); } 
        }

        public void RecallPreset(ushort preset)
        {
            HttpClient client = new HttpClient();
            HttpClientRequest request = new HttpClientRequest();
            request.RequestType = RequestType.Get;
            request.Header.AddHeader(new HttpHeader("X-Requested-Auth", "Digest"));
            string commandURL = Utilities.BaseURL + Utilities.PanTiltZoomService + Utilities.RecallDevicePresetAction;
            Utilities.CameraAction action = Utilities.CameraAction.PresetRecall;

            try
            {
                request.Url = new UrlParser(String.Format(commandURL, this.IPAddress, preset));
                HttpClient.DISPATCHASYNC_ERROR err = client.DispatchAsyncEx(request, this.OnHttpResponse, action);
                CrestronConsole.PrintLine("Axis Camera @ {0} | DISPATCHASYNC_ERR: {1} | PTZ Action: {2}", this.IPAddress, err, action);
            }
            catch (Exception e) { CrestronConsole.PrintLine("Axis Camera @ {0} | Preset Recall | Exception: {1}", this.IPAddress, e.StackTrace); }
        }

        public void SavePreset(ushort preset)
        {
            HttpClient client = new HttpClient();
            HttpClientRequest request = new HttpClientRequest();
            request.RequestType = RequestType.Get;
            request.Header.AddHeader(new HttpHeader("X-Requested-Auth", "Digest"));
            string commandURL = Utilities.BaseURL + Utilities.PanTiltZoomService + Utilities.SaveDevicePresetAction;
            
            Utilities.CameraAction action = Utilities.CameraAction.PresetSave;

            try
            {
                request.Url = new UrlParser(String.Format(commandURL, this.IPAddress, preset));
                HttpClient.DISPATCHASYNC_ERROR err = client.DispatchAsyncEx(request, this.OnHttpResponse, action);
                CrestronConsole.PrintLine("Axis Camera @ {0} | DISPATCHASYNC_ERR: {1} | PTZ Action: {2}", this.IPAddress, err, action);
            }
            catch (Exception e) { CrestronConsole.PrintLine("Axis Camera @ {0} | Preset Save | Exception: {1}", this.IPAddress, e.StackTrace); }
        }

        public void PanTiltZoom(ushort action)
        {
            HttpClient client = new HttpClient();
            HttpClientRequest request = new HttpClientRequest();
            request.RequestType = RequestType.Get;
            request.Header.AddHeader(new HttpHeader("X-Requested-Auth", "Digest"));
            string commandURL = Utilities.BaseURL + Utilities.PanTiltZoomService + Utilities.ContinuousPanTiltMoveAction;
            HttpClient.DISPATCHASYNC_ERROR err;
            Utilities.CameraAction act = (Utilities.CameraAction)action;

            string zoomURL = Utilities.BaseURL + Utilities.PanTiltZoomService + Utilities.ContinuousZoomMoveAction;

            switch (act)
            {
                case Utilities.CameraAction.Stop:
                    //set the ptz speed to 0,0 to stop.
                    try { request.Url = new UrlParser(String.Format(commandURL, this.IPAddress, 0, 0)); }
                    catch (Exception e) { CrestronConsole.PrintLine("Axis Camera @ {0} | PTZ Stop | Exception: {1}", this.IPAddress, e.StackTrace); }

                    //create a new request specifically to also stop any zoom actions just in case
                    HttpClientRequest zoomStopRequest = new HttpClientRequest();
                    zoomStopRequest.RequestType = RequestType.Get;
                    zoomStopRequest.Header.AddHeader(new HttpHeader("X-Requested-Auth", "Digest"));
                    
                    //set the zoom speed to 0 to stop.
                    try 
                    {
                        HttpClient stopClient = new HttpClient();
                        zoomStopRequest.Url = new UrlParser(String.Format(zoomURL, this.IPAddress, 0));
                        HttpClient.DISPATCHASYNC_ERROR zoomStopErr = stopClient.DispatchAsyncEx(zoomStopRequest, this.OnHttpResponse, act);
                        CrestronConsole.PrintLine("Axis Camera @ {0} | DISPATCHASYNC_ERR: {1} | Zoom Stop Action", this.IPAddress, zoomStopErr);
                    }
                    catch (Exception e) { CrestronConsole.PrintLine("Axis Camera @ {0} | Zoom Stop | Exception: {1}", this.IPAddress, e.StackTrace); }

                    break;
                case Utilities.CameraAction.Down:
                    try { request.Url = new UrlParser(String.Format(commandURL, this.IPAddress, 0, GenerateActualPTZSpeed(this.PanTiltSpeed, act))); }
                    catch (Exception e) { CrestronConsole.PrintLine("Axis Camera @ {0} | Action: {2} | Exception: {1}", this.IPAddress, e.StackTrace, act); } 
                    break;
                case Utilities.CameraAction.Up:
                    try { request.Url = new UrlParser(String.Format(commandURL, this.IPAddress, 0, GenerateActualPTZSpeed(this.PanTiltSpeed, act))); }
                    catch (Exception e) { CrestronConsole.PrintLine("Axis Camera @ {0} | Action: {2} | Exception: {1}", this.IPAddress, e.StackTrace, act); } 
                    break;
                case Utilities.CameraAction.Left:
                    try { request.Url = new UrlParser(String.Format(commandURL, this.IPAddress, GenerateActualPTZSpeed(this.PanTiltSpeed, act), 0)); }
                    catch (Exception e) { CrestronConsole.PrintLine("Axis Camera @ {0} | Action: {2} | Exception: {1}", this.IPAddress, e.StackTrace, act); } 
                    break;
                case Utilities.CameraAction.Right:
                    try { request.Url = new UrlParser(String.Format(commandURL, this.IPAddress, GenerateActualPTZSpeed(this.PanTiltSpeed, act), 0)); }
                    catch (Exception e) { CrestronConsole.PrintLine("Axis Camera @ {0} | Action: {2} | Exception: {1}", this.IPAddress, e.StackTrace, act); } 
                    break;
                case Utilities.CameraAction.In:
                    try { request.Url = new UrlParser(String.Format(zoomURL, this.IPAddress, GenerateActualPTZSpeed(this.ZoomSpeed, act))); }
                    catch (Exception e) { CrestronConsole.PrintLine("Axis Camera @ {0} | Action: {2} | Exception: {1}", this.IPAddress, e.StackTrace, act); } 
                    break;
                case Utilities.CameraAction.Out:
                    try { request.Url = new UrlParser(String.Format(zoomURL, this.IPAddress, GenerateActualPTZSpeed(this.ZoomSpeed, act))); }
                    catch (Exception e) { CrestronConsole.PrintLine("Axis Camera @ {0} | Action: {2} | Exception: {1}", this.IPAddress, e.StackTrace, act); } 
                    break;
            }

            try
            {
                err = client.DispatchAsyncEx(request, this.OnHttpResponse, action);
                CrestronConsole.PrintLine("Axis Camera @ {0} | DISPATCHASYNC_ERR: {1} | PTZ Action: {2}", this.IPAddress, err, act);
            }
            catch (Exception e) { CrestronConsole.PrintLine("Axis Camera @ {0} | Action: {2} | Exception: {1}", this.IPAddress, e.StackTrace, act); } 
        }
    }
}
