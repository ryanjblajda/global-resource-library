using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Newtonsoft.Json;

namespace Clickshare.API
{
    internal class ApplicationLanguageInformation
    {
        [JsonProperty("language")]
        public string Language { get; private set; }

        public ApplicationLanguageInformation(string lang)
        {
            this.Language = lang;
        }
    }

    internal class SystemInformation
    {
        public const string Version = "version";
        public const string CPU = "cpu";
        public const string Disk = "disk";
        public const string PersistentTime = "persisTime";
        public const string Memory = "memory";

        [JsonProperty(Version, NullValueHandling = NullValueHandling.Ignore)]
        public VersionInformation VersionInformation { get; private set; }

        [JsonProperty(CPU, NullValueHandling = NullValueHandling.Ignore)]
        public CPUInformation CPUInformation { get; private set; }

        [JsonProperty("mem", NullValueHandling =NullValueHandling.Ignore)]
        public MemoryInformation MemoryInformation { get; private set; }

        [JsonProperty(Disk, NullValueHandling = NullValueHandling.Ignore)]
        public DiskInformation DiskInformation { get; private set; }

        [JsonProperty(PersistentTime, NullValueHandling = NullValueHandling.Ignore)]
        public PersistenceInformation PersistenceInformation { get; private set; }

        public SystemInformation()
        {
            this.VersionInformation = new VersionInformation();
            this.CPUInformation = new CPUInformation();
            this.MemoryInformation = new MemoryInformation();
            this.DiskInformation = new DiskInformation();
            this.PersistenceInformation = new PersistenceInformation();
        }
    }

    internal class NetworkInformation
    {
        [JsonProperty("status")]
        public string Status { get; private set; }

        [JsonProperty("address")]
        public string IPAddress { get; private set; }

        [JsonProperty("netmask")]
        public string SubnetMask { get; private set; }

        [JsonProperty("gw")]
        public string DefaultGateway { get; private set; }

        [JsonProperty("dns")]
        public string DNSServers { get; private set; }

        [JsonProperty("device")]
        public string InterfaceName { get; private set; }

        [JsonProperty("mac")]
        public string MACAddress { get; private set; }

        [JsonProperty("method")]
        public string Method { get; private set; }

        [JsonProperty("enable")]
        public bool Enabled { get; private set; }
    }

    internal class HostnameInformation
    {
        [JsonProperty("hostname")]
        public string Hostname { get; private set; }

        public HostnameInformation()
        {
            this.Hostname = String.Empty;
        }
    }

    internal class LoginInformation
    {
        [JsonProperty("token")]
        public string Token { get; private set; }

        [JsonProperty("alias")]
        public string LoginAlias { get; private set; }

        [JsonProperty("changed")]
        public bool IsLoginChanged { get; private set; }
    }

    internal class VersionInformation
    {
        [JsonProperty("ndiVersion")]
        public string VersionNDI { get; private set; }

        [JsonProperty("hardwareVersion")]
        public string VersionHardware { get; private set; }

        [JsonProperty("softwareVersion")]
        public string VersionSoftware { get; private set; }

        [JsonProperty("serialNumber")]
        public string SerialNumber { get; private set; }

        [JsonProperty("product")]
        public string Product { get; private set; }

        [JsonProperty("oem")]
        public string OEM { get; private set; }

        public VersionInformation()
        {
            this.VersionNDI = "?";
            this.VersionHardware = "?";
            this.VersionSoftware = "?";
            this.SerialNumber = "?";
            this.Product = "?";
            this.OEM = "?";
        }
    }

    internal class CPUInformation
    {
        [JsonProperty("percent")]
        public float Usage { get; private set; }

        public CPUInformation()
        {
            this.Usage = 0;
        }
    }

    internal class MemoryInformation
    {
        [JsonProperty("percent")]
        public float Usage { get; private set; }

        [JsonProperty("total")]
        public long Total { get; private set; }

        [JsonProperty("used")]
        public long Used { get; private set; }

        public MemoryInformation()
        {
            this.Total = 0;
            this.Usage = 0;
            this.Used = 0;
        }
    }

    internal class DiskInformation
    {
        [JsonProperty("percent")]
        public float Usage { get; private set; }

        [JsonProperty("total")]
        public long Total { get; private set; }

        [JsonProperty("used")]
        public long Used { get; private set; }
        
        [JsonProperty("free")]
        public long Free { get; private set; }

        public DiskInformation()
        {
            this.Total = 0;
            this.Usage = 0;
            this.Used = 0;
            this.Free = 0;
        }
    }

    internal class PersistenceInformation
    {
        [JsonProperty("uptime")]
        public float Uptime { get; private set; }

        public PersistenceInformation()
        {
            this.Uptime = 0;
        }
    }

    internal class DecodingStreamPresetInformation
    {
        [JsonProperty("name")]
        public string Name { get; private set; }

        [JsonProperty("id")]
        public int ID { get; private set; }

        [JsonProperty("url")]
        public string URL { get; private set; }

        [JsonProperty("group")]
        public string Group { get; private set; }

        [JsonProperty("online")]
        public string Online { get; private set; }

        [JsonProperty("warning")]
        public string Warning { get; private set; }

        [JsonProperty("device_name")]
        public string DeviceName { get; private set; }

        [JsonProperty("channel_name")]
        public string ChannelName { get; private set; }

        [JsonProperty("ip")]
        public string IPAddress { get; private set; }

        [JsonProperty("port")]
        public int Port { get; private set; }

        [JsonProperty("enable")]
        public int Enable { get; private set; }
    }

    internal class DecodingStreamInformation
    {
        [JsonProperty("name")]
        public string Name { get; private set; }
        
        [JsonProperty("ip")]
        public string IPAddress { get; private set; }
        
        [JsonProperty("online")]
        public bool Online { get; private set; }
        
        [JsonProperty("resolution")]
        public string Resolution { get; private set; }
        
        [JsonProperty("codec")]
        public string Codec { get; private set; }
        
        [JsonProperty("bitrate")]
        public long Bitrate { get; private set; }
        
        [JsonProperty("deInterlace")]
        public bool DeInterlace { get; private set; }
        
        [JsonProperty("interlaced")]
        public int Interlaced { get; private set; }
        
        [JsonProperty("frame_rate")]
        public int FrameRate { get; private set; }
        
        [JsonProperty("audio")]
        public string Audio { get; private set; }
        
        [JsonProperty("url")]
        public string URL { get; private set; }
        
        [JsonProperty("xRes")]
        public int Width { get; private set; }
        
        [JsonProperty("yRes")]
        public int Height { get; private set; }
        
        [JsonProperty("audio_sampling")]
        public int AudioSampling { get; private set; }
        
        [JsonProperty("audio_channels")]
        public int AudioChannels { get; private set; }

        [JsonProperty("hdcp")]
        public int HDCP { get; private set; }
        
        [JsonProperty("out_colorspace")]
        public int ColorSpace { get; private set; }
        
        [JsonProperty("supported")]
        public bool Supported { get; private set; }
        
        [JsonProperty("output_resolution_choose")]
        public string OutputResolutionChosen { get; private set; }
        
        [JsonProperty("output_resolution")]
        public string OutputResolution { get; private set; }
        
        [JsonProperty("hdmi_channels")]
        public int HDMIChannels { get; private set; }
        
        [JsonProperty("hdmi_channels_mapping")]
        public List<ChannelMappingInformation> HDMIChannelsMapping { get; private set; }

        [JsonProperty("line_out_channels")]
        public int LineOutChannels { get; private set; }

        [JsonProperty("line_out_channels_mapping")]
        public List<ChannelMappingInformation> LineOutChannelsMapping { get; private set; }

        [JsonProperty("ndi_connection")]
        public string Connection { get; private set; }

    }

    internal class ChannelMappingInformation
    {
        [JsonProperty("output_channel")]
        public int OutputChannel { get; private set; }
        
        [JsonProperty("source_channel")]
        public int SourceChannel { get; private set; }
    }
}