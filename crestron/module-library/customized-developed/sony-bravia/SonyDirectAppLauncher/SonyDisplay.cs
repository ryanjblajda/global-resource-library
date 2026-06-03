using System;
using System.Text;
using Crestron.SimplSharp; // For Basic SIMPL# Classes
using Crestron.SimplSharp.Net.Http;
using Crestron.SimplSharp.CrestronXmlLinq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Crestron.SimplSharp.CrestronXml.Serialization;
using Crestron.SimplSharp.CrestronSockets;
using System.Text.RegularExpressions;

namespace SonyLiteControl
{
    public class SonyDisplay
    {
        private string ipaddr;
        public string IPAddress 
        {
            get { return this.ipaddr; }
            set
            {
                if (value != null)
                {
                    this.ipaddr = value;
                    if (this.Comms != null) this.Comms.AddressClientConnectedTo = this.ipaddr;
                }
            }
        }

        public string PSK { get; set; }

        private ushort pollintveral;
        public ushort PollInterval 
        {
            get { return pollintveral; }
            set { this.pollintveral = (ushort)(value * 10); }
        }

        public ushort VolumeIncrementInterval { get; set; }

        private CTimer PollTimer;
        private CTimer VolumeNudgeTimer;
        private VolumeAction volaction;
        private Regex InputURI;
        private Regex SimpleResponse;

        public delegate void AnalogDigitalPayloadEvent(object sender, AnalogDigitalPayloadArgs args);
        public event AnalogDigitalPayloadEvent PowerState;
        public event AnalogDigitalPayloadEvent MuteState;
        public event AnalogDigitalPayloadEvent VolumeState;
        public event AnalogDigitalPayloadEvent InputState;
        public event AnalogDigitalPayloadEvent PictureMuteState;
        
        private bool pwr;
        private bool Power 
        { 
            get { return this.pwr; }
            set
            {
                this.pwr = value;
                if (this.PowerState != null) this.PowerState(this, new AnalogDigitalPayloadArgs((this.Power == true) ? (ushort)Signal.High : (ushort)Signal.Low));
            }
        }

        private bool mute;
        private bool Muted 
        {
            get { return this.mute; }
            set 
            {
                this.mute = value;
                if (this.MuteState != null) this.MuteState(this, new AnalogDigitalPayloadArgs((this.Muted == true) ? (ushort)Signal.High : (ushort)Signal.Low));
            }
        }
        private int vol;
        private int Volume
        {
            get { return vol; }
            set 
            { 
                this.vol = value * 65535 / 100;
                if (this.VolumeState != null) this.VolumeState(this, new AnalogDigitalPayloadArgs((ushort)this.Volume));
            } 
        }

        private int input;
        private int Input 
        {
            get { return input; }
            set
            {
                this.input = value;
                if (this.InputState != null) this.InputState(this, new AnalogDigitalPayloadArgs((ushort)this.Input));
            }
        }

        private bool picmute;
        private bool PictureMuted
        {
            get { return this.picmute; }
            set
            {
                this.picmute = value;
                if (this.PictureMuteState != null) this.PictureMuteState(this, new AnalogDigitalPayloadArgs((this.PictureMuted == true) ? (ushort)Signal.High : (ushort)Signal.Low));
            }
        }

        private bool Debug { get; set; }

        private TCPClient Comms;

        public SonyDisplay()
        {
            this.PollTimer = new CTimer(this.OnPollTimerExpired, Timeout.Infinite);
            this.VolumeNudgeTimer = new CTimer(this.OnVolumeNudgeTimerExpired, Timeout.Infinite);
            this.VolumeIncrementInterval = 500;
            this.InputURI = new Regex("\\w+:(\\w+)\\?\\w+=(\\d)");
            this.SimpleResponse = new Regex("\\*S([ENAC])([\\D]+)([\\d]+)\n");
            
            this.Comms = new TCPClient();
            this.Comms.AddressClientConnectedTo = this.IPAddress;
            this.Comms.PortNumber = SimpleCommands.TCPPort;
        }

        private void OnPollTimerExpired(object sender)
        {
            if (this.IPAddress != String.Empty)
            {
                if (this.Debug) CrestronConsole.PrintLine("SONY DISPLAY @ {0} | POLL TIMER EXPIRED", this.IPAddress);
                this.GetAppList();
                this.GetPowerStatus();

                if (this.Power)
                {
                    this.GetVolumeInformation();
                    this.GetPlayingContentInfo();
                    this.GetPictureMuteStatus();
                }
                //this.GetCurrentExternalInputStatus();
                //this.GetWebAppStatus();
            }
            else { if (this.Debug) CrestronConsole.PrintLine("SONY DISPLAY @ {0} | IP ADDR INVALID", this.IPAddress); }
        }

        private void OnVolumeNudgeTimerExpired(object sender)
        {
            if (this.volaction == VolumeAction.Up)
            {
                this.VolumeNudge(VolumeAction.Up);
            }
            else if (this.volaction == VolumeAction.Down)
            {
                this.VolumeNudge(VolumeAction.Down);
            }
        }

        private void OnResponse(HttpClientResponse response, HTTP_CALLBACK_ERROR err, object obj)
        {
            if(err != HTTP_CALLBACK_ERROR.COMPLETED) 
                if(this.Debug) CrestronConsole.PrintLine("SONY DISPLAY @ {0} | HTTP_CALLBACK_ERROR: {1}", this.IPAddress, err.ToString());

            if (response != null)
            {
                if (response.HasContentLength)
                {
                    string method = (string)obj;

                    if (this.Debug) CrestronConsole.PrintLine("SONY DISPLAY @ {0} | {1}", this.IPAddress, response.ContentString);
                    try
                    {
                        switch (method)
                        {
                            case Methods.GetApplicationList:
                                break;
                            case Methods.GetCurrentExternalInputsStatus:
                                break;
                            case Methods.GetPlayingContentInfo:
                                GenericResponse InputResponse = JsonConvert.DeserializeObject<SonyLiteControl.GenericResponse>(response.ContentString);
                                if (InputResponse.GenericResults[0].Uri != null)
                                {
                                    if (this.Debug) CrestronConsole.PrintLine("SONY DISPLAY @ {0} | {1} {2}", this.IPAddress, method, InputResponse.GenericResults[0].Uri);
                                    this.Input = this.ParseInputUri(InputResponse.GenericResults[0].Uri);
                                }
                                break;
                            case Methods.GetPowerStatus:
                                GenericResponse PowerResponse = JsonConvert.DeserializeObject<SonyLiteControl.GenericResponse>(response.ContentString);
                                if(PowerResponse.GenericResults[0].Status != null) this.Power = (PowerResponse.GenericResults[0].Status == "standby") ? false : true;
                                break;
                            case Methods.GetVolumeInformation:
                                AudioResponse VolumeResponse = JsonConvert.DeserializeObject<SonyLiteControl.AudioResponse>(response.ContentString);
                                if(VolumeResponse.AudioResults[0][0].Volume != null) this.Volume = (int)VolumeResponse.AudioResults[0][0].Volume;
                                if(VolumeResponse.AudioResults[0][0].Mute != null) this.Muted = (bool)VolumeResponse.AudioResults[0][0].Mute;
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        if (this.Debug) CrestronConsole.PrintLine("SONY DISPLAY @ {0} | {1} {2}", this.IPAddress, method, e.Message);
                    }
                }
            }
        }

        private void OnXmlResponse(HttpClientResponse response, HTTP_CALLBACK_ERROR err)
        {
            if (this.Debug)
            {
                if (response.HasContentLength) CrestronConsole.PrintLine(response.ContentString);
                else CrestronConsole.PrintLine(err.ToString());
            }
        }

        private void SendCommandJSON(string service, string method, Dictionary<object, object> body)
        {
            if (this.PSK != null && this.IPAddress != null)
            {
                if (this.IPAddress != String.Empty)
                {
                    HttpClient thisClient = new HttpClient();
                    thisClient.HostName = this.IPAddress;
                    HttpClientRequest thisRequest = new HttpClientRequest();
                    thisRequest.Header.AddHeader(new HttpHeader("X-Auth-PSK", this.PSK));
                    thisRequest.RequestType = RequestType.Post;
                    thisRequest.Header.ContentType = "application/json";

                    string ipaddr = this.IPAddress.Replace("http://", "");
                    ipaddr = ipaddr.Replace("https://", "");

                    thisRequest.Url = new UrlParser(string.Format("http://{0}/sony/{1}", ipaddr, service));
                    thisRequest.ContentString = JsonConvert.SerializeObject(new Dictionary<object, object>() { { "method", method }, { "id", 1 }, { "params", (body == null) ? new List<Dictionary<object, object>>() : new List<Dictionary<object, object>>() { body } }, { "version", "1.0" } }, Formatting.Indented);

                    thisClient.DispatchAsyncEx(thisRequest, this.OnResponse, method);
                }
                else { if (this.Debug) CrestronConsole.PrintLine("SONY DISPLAY @ {0} | IP ADDR INVALID", this.IPAddress); }
            }
        }

        private void SendCommandSOAP(string service, string cmd)
        {
            if (this.PSK != null && this.IPAddress != null)
            {
                HttpClient thisClient = new HttpClient();
                thisClient.HostName = this.IPAddress;
                HttpClientRequest thisRequest = new HttpClientRequest();
                thisRequest.Header.AddHeader(new HttpHeader("SOAPACTION", "\"urn:schemas-sony-com:service:IRCC:1#X_SendIRCC\""));
                thisRequest.Header.AddHeader(new HttpHeader("X-Auth-PSK", this.PSK));
                thisRequest.RequestType = RequestType.Post;
                thisRequest.Header.ContentType = "text/xml; charset=UTF-8";

                string ipaddr = this.IPAddress.Replace("http://", "");
                ipaddr = ipaddr.Replace("https://", "");

                thisRequest.Url = new UrlParser(string.Format("http://{0}/sony/{1}", ipaddr, service));
                thisRequest.ContentString = String.Format("<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\" s:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\"> <s:Body><u:X_SendIRCC xmlns:u=\"urn:schemas-sony-com:service:IRCC:1\"><IRCCCode>{0}</IRCCCode></u:X_SendIRCC></s:Body></s:Envelope>", cmd);

                thisClient.DispatchAsync(thisRequest, this.OnXmlResponse);
            }
        }

        private void SendCommandSimple(string cmd)
        {
            if (this.Comms.ClientStatus == SocketStatus.SOCKET_STATUS_CONNECTED)
            {
                SocketErrorCodes err = this.Comms.SendDataAsync(Encoding.UTF8.GetBytes(cmd), Encoding.UTF8.GetByteCount(cmd), this.OnTCPClientSend);
                CrestronConsole.PrintLine("SONY DISPLAY @ {0} | SOCKET STATUS {1}", this.IPAddress, err);
            }
            else
            {
                SocketErrorCodes err = this.Comms.ConnectToServerAsync(this.OnTCPClientConnected, cmd);
                CrestronConsole.PrintLine("SONY DISPLAY @ {0} | SOCKET STATUS {1}", this.IPAddress, err);
            }

            this.Comms.ReceiveDataAsync(this.OnTCPClientReceive);
        }

        private void OnTCPClientConnected(TCPClient client, object usrobj)
        {
            string cmd = (string)usrobj;
            if (client.ClientStatus == SocketStatus.SOCKET_STATUS_CONNECTED)
            {
                SocketErrorCodes err = client.SendDataAsync(Encoding.UTF8.GetBytes(cmd), Encoding.UTF8.GetByteCount(cmd), this.OnTCPClientSend);
                CrestronConsole.PrintLine("SONY DISPLAY @ {0} | SOCKET STATUS {1}", this.IPAddress, err);
            }
        }

        private void OnTCPClientReceive(TCPClient client, int received)
        {
            CrestronConsole.PrintLine("SONY DISPLAY @ {0} | RECEIVED {1} BYTES", this.IPAddress, received);
            string dataRx = Encoding.UTF8.GetString(client.IncomingDataBuffer, 0, received);
            CrestronConsole.PrintLine("SONY DISPLAY @ {0} | SAID {1}", this.IPAddress, dataRx);
            this.Comms.ReceiveDataAsync(this.OnTCPClientReceive);
            if (dataRx.Length > 0)
            {
                if(this.SimpleResponse.IsMatch(dataRx))
                {
                    Match simpleCommandResponse = this.SimpleResponse.Match(dataRx);
                    string cmd = simpleCommandResponse.Groups[2].ToString();
                    CrestronConsole.PrintLine("SONY DISPLAY @ {0} | {1}", this.IPAddress, cmd);
                    switch (cmd)
                    {
                        case SimpleCommands.PictureMutePrefix:
                            //parse response
                            try
                            {
                                string state = simpleCommandResponse.Groups[3].ToString();
                                int realstate = Int32.Parse(state[state.Length - 1].ToString());
                                CrestronConsole.PrintLine("SONY DISPLAY @ {0} | {1} // {2}", this.IPAddress, realstate, state);
                                //set current state so SIMPL+ updates
                                this.PictureMuted = (realstate == 1) ? true : false;
                            }
                            catch (Exception e)
                            {
                                CrestronConsole.PrintLine("SONY DISPLAY @ {0} | {1}", this.IPAddress, e.Message);
                            }
                            break;
                    }
                }
            }
        }

        private void OnTCPClientSend(TCPClient client, int sent)
        {
            CrestronConsole.PrintLine("SONY DISPLAY @ {0} | SENT {1} BYTES", this.IPAddress, sent);
            //SocketErrorCodes err = this.Comms.DisconnectFromServer();
            //CrestronConsole.PrintLine("SONY DISPLAY @ {0} | SOCKET STATUS {1}", this.IPAddress, err);
        }

        private int ParseInputUri(string uri)
        {
            int input = 0;
            if (this.InputURI.IsMatch(uri))
            {
                Match uriMatch = this.InputURI.Match(uri);
                string port = uriMatch.Groups[1].ToString();

                switch (port)
                {
                    case Ports.HDMI:
                        input = Int32.Parse(uriMatch.Groups[2].ToString());
                        break;
                    case Ports.Component:
                        input = Int32.Parse(uriMatch.Groups[2].ToString()) + 4;
                        break;
                    case Ports.SCART:
                        input = Int32.Parse(uriMatch.Groups[2].ToString()) + 8;
                        break;
                }
            }

            return input;
        }

        private void GetVolumeInformation()
        {
            this.SendCommandJSON(Service.Audio, Methods.GetVolumeInformation, null);
        }

        private void GetPowerStatus()
        {
            this.SendCommandJSON(Service.System, Methods.GetPowerStatus, null);
        }

        private void GetPlayingContentInfo()
        {
            this.SendCommandJSON(Service.AVContent, Methods.GetPlayingContentInfo, null);
        }

        private void GetCurrentExternalInputStatus()
        {
            this.SendCommandJSON(Service.AVContent, Methods.GetCurrentExternalInputsStatus, null);
        }

        private void GetWebAppStatus()
        {
            this.SendCommandJSON(Service.AppControl, Methods.GetWebAppStatus, null);
        }

        private void GetAppList()
        {
            this.SendCommandJSON(Service.AppControl, Methods.GetApplicationList, null);
        }

        public void EnableDebug(ushort state)
        {
            this.Debug = ((Signal)state == Signal.High) ? true : false;
        }

        public void Poll(ushort state)
        {
            if (state == 1)
            {
                this.PollTimer.Reset(0, this.PollInterval);
                return;
            }
            this.PollTimer.Stop();
        }

        public void SetActiveApp(string uri)
        {
            this.SendCommandJSON(Service.AppControl, Methods.SetActiveApp, new Dictionary<object, object>() { { "uri", uri }, { "data", "" } });
        }

        public void SetPowerState(ushort state)
        {
            this.SendCommandJSON(Service.System, Methods.SetPowerStatus, new Dictionary<object, object>() { { "status", ((Signal)state == Signal.High) ? true : false } });
            this.GetPowerStatus();
        }

        public void TogglePower()
        {
            this.SendCommandJSON(Service.System, Methods.SetPowerStatus, new Dictionary<object, object>() { { "status", !this.Power } });
            this.GetPowerStatus();
        }

        public void SetVolume(ushort volume)
        {
            //scale 0-65535 to 0-100
            int vol = volume * 100 / 65535;
            this.SendCommandJSON(Service.Audio, Methods.SetAudioVolume, new Dictionary<object, object>() { { "target", "speaker" }, { "volume", vol.ToString() } });
            this.GetVolumeInformation();
        }

        internal void VolumeNudge(VolumeAction action)
        {
            string nudge = "";
            this.volaction = action;
            switch (action)
            {
                case VolumeAction.Up:
                    nudge = "+1";
                    break;
                case VolumeAction.Down:
                    nudge = "-1";
                    break;
            }

            this.SendCommandJSON(Service.Audio, Methods.SetAudioVolume, new Dictionary<object, object>() { {"target", "speaker"}, { "volume", nudge } });
            this.VolumeNudgeTimer.Reset(this.VolumeIncrementInterval, this.VolumeIncrementInterval);
            this.GetVolumeInformation();
        }

        public void VolumeNudge(ushort action)
        {
            string nudge = "";
            this.volaction = (VolumeAction)action;
            switch ((VolumeAction)action)
            {
                case VolumeAction.Up:
                    nudge = "+1";
                    break;
                case VolumeAction.Down:
                    nudge = "-1";
                    break;
            }

            this.SendCommandJSON(Service.Audio, Methods.SetAudioVolume, new Dictionary<object, object>() { { "target", "speaker" }, { "volume", nudge } });
            this.VolumeNudgeTimer.Reset(this.VolumeIncrementInterval, this.VolumeIncrementInterval);
            this.GetVolumeInformation();
        }

        public void StopVolumeNudge()
        {
            this.VolumeNudgeTimer.Stop();
        }

        public void SetMute(ushort state)
        {
            this.SendCommandJSON(Service.Audio, Methods.SetAudioMute, new Dictionary<object, object>() { { "status", ((Signal)state == Signal.High) ? true : false } });
            this.GetVolumeInformation();
        }

        public void ToggleMute()
        {
            this.SendCommandJSON(Service.Audio, Methods.SetAudioMute, new Dictionary<object, object>() { { "status", !this.Muted } });
            this.GetVolumeInformation();
        }
        //
        /*
         * inputs
         * 1 - hdmi 1, 2 - hdmi 2, 3 - hdmi 3, 4 - hdmi 4, 5 - comp 1, 6 - comp 2, 7 - comp 3, 8 - comp 4, 9 - scart 1, 10 - scart 2, 11 - scart 3, 12 - scart 4, 
        */
        //
        public void SetInput(ushort input)
        {
            string kind = String.Empty;
            string port = String.Empty;

            //generate strings for port type based on agnostic inputs from SIMPL+
            if (input <= 4)
            {
                kind = Ports.HDMI;
                port = input.ToString();
            }
            else if (input > 4 && input <= 8)
            {
                kind = Ports.Component;
                port = (input - 4).ToString();
            }
            else if (input > 8 && input <= 12)
            {
                kind = Ports.SCART;
                port = (input - 8).ToString();
            }
            
            string uri = string.Format("extInput:{0}?port={1}", kind, port);
            this.SendCommandJSON(Service.AVContent, Methods.SetPlayContent, new Dictionary<object, object>() { { "uri", uri } });
            this.GetCurrentExternalInputStatus();
            this.GetPlayingContentInfo();
        }

        public void SetAllAppsTerminate()
        {
            this.SendCommandJSON(Service.AppControl, Methods.TerminateApps, null);
        }

        public void SendTransportCommand(ushort cmd)
        {
            string irCode = String.Empty;
            switch ((Transport)cmd)
            {
                case Transport.Up:
                    irCode = "AAAAAQAAAAEAAAB0Aw==";
                    break;
                case Transport.Down:
                    irCode = "AAAAAQAAAAEAAAB1Aw==";
                    break;
                case Transport.Left:
                    irCode = "AAAAAQAAAAEAAAA0Aw==";
                    break;
                case Transport.Right:
                    irCode = "AAAAAQAAAAEAAAAzAw==";
                    break;
                case Transport.Select:
                    irCode = "AAAAAQAAAAEAAABlAw==";
                    break;
                case Transport.Home:
                    irCode = "AAAAAQAAAAEAAABgAw==";
                    break;
                case Transport.Options:
                    irCode = "AAAAAgAAAJcAAAA2Aw==";
                    break;
                case Transport.Back:
                    irCode = "AAAAAgAAAJcAAAAjAw==";
                    break;
                case Transport.Play:
                    irCode = "AAAAAgAAAJcAAAAaAw==";
                    break;
                case Transport.Pause:
                    irCode = "AAAAAgAAAJcAAAAZAw==";
                    break;
            }

            if(irCode != String.Empty) this.SendCommandSOAP(Service.IRCC, irCode);
        }

        public void SetPictureMute(ushort mute)
        {
            //send a command via the SIMPLE control format, inserting the command value into the appropriate place
            this.SendCommandSimple(String.Format(SimpleCommands.SetPictureMute, mute));
        }

        private void GetPictureMuteStatus()
        {
            this.SendCommandSimple(SimpleCommands.UpdatePictureMute);
        }
    }
}
