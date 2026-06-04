using System;
using System.Text;
using System.Linq;
using Crestron.SimplSharp;                          				// For Basic SIMPL# Classes
using Newtonsoft.Json;
using System.Collections.Generic;
using FileOperations;

namespace BasicConfigReader
{
    public class ConfigReader
    {
        public delegate void OnDeserialize(ushort success);
        public OnDeserialize Complete { get; set; }

        public delegate void UpdateStringItemsDelegate(ushort idx, SimplSharpString value);
        public UpdateStringItemsDelegate UpdateStringItem { get; set; }

        public delegate void UpdateDigitalItemsDelegate(ushort idx, short value);
        public UpdateDigitalItemsDelegate UpdateDigitalItem { get; set; }

        public delegate void UpdateAnalogItemsDelegate(ushort idx, short value);
        public UpdateAnalogItemsDelegate UpdateAnalogItem { get; set; }

        private bool IsDebug;

        private FileReaderWriter FileManager;
        
        private string _path;
        public string FilePath 
        {
            get { return _path; }
            set
            {
                _path = value;
                if (_path != null && FileManager != null) this.Refresh(this._path);
            }
        }

        private CTimer PeriodicUpdateTimer;

        private Dictionary<string, Item> ConfigurationItems;

        /// <summary>
        /// default constructor
        /// </summary>
        public ConfigReader()
        {
            this.FileManager = new FileReaderWriter();
            this.ConfigurationItems = new Dictionary<string, Item>();
            this.PeriodicUpdateTimer = new CTimer(this.OnPeriodicUpdateTimerExpired, Timeout.Infinite);
        }

        /// <summary>
        /// sets whether debug printing should occur
        /// </summary>
        /// <param name="enable">the debug state</param>
        public void SetDebug(ushort enable)
        {
            this.IsDebug = Util.Conversion.ConvertToBool(enable);
        }

        /// <summary>
        /// enables a periodic refresh of the configuration file
        /// </summary>
        /// <param name="time"></param>
        public void EnablePeriodicRefresh(ushort enable, int time)
        {
            if (Util.Conversion.ConvertToBool(enable)) { this.PeriodicUpdateTimer.Reset(0, time * 1000); }
            else { this.PeriodicUpdateTimer.Stop(); }
        }

        /// <summary>
        /// load the details from a specified path
        /// </summary>
        /// <param name="path">the specified path to load json data from</param>
        public void Refresh(string path)
        {
            string contents = FileManager.Refresh(path);
            
            if (contents != null)
            {
                bool success = false;
                try
                {
                    Configuration results = JsonConvert.DeserializeObject<Configuration>(contents);
                    success = true;

                    this.OnConfigRefresh(results);
                }
                catch (Exception e)
                {
                    if (this.IsDebug) CrestronConsole.PrintLine("Error Deserializing Contents of File {0} | {1}", this.FilePath, e.Message);
                    success = false;
                }
                finally
                {
                    if (this.Complete != null) this.Complete(Util.Conversion.ConvertToSignal(success));
                }
            }
        }

        /// <summary>
        /// //called by the SIMPL+ end of things to add a field that the module has subscribed to
        /// </summary>
        /// <param name="index">the signal index the parameter is located at, so we know where to aim updates</param>
        /// <param name="name">the name that should be looked for in the config file</param>
        /// <param name="type">the signal type, serial, analog, digital</param>
        public void AddItem(ushort index, string name, ushort type)
        {
            if (!this.ConfigurationItems.Keys.ToList().Contains(name))
            {
                if (this.IsDebug) CrestronConsole.PrintLine("Adding Field: {1} @ Index {0} | Type == {2}", index, name, (Util.Signal)type);
                this.ConfigurationItems.Add(name, new Item((Util.Signal)type, name, index));
            }
            else { if (this.IsDebug) CrestronConsole.PrintLine("Matching Field: {1} @ Index {0} | Type == {2} // Already Exists!! Not Adding!", index, name, (Util.Signal)type); } 
        }

        private void OnPeriodicUpdateTimerExpired(object sender)
        {
            if (this.IsDebug) CrestronConsole.PrintLine("Periodic Update Timer Expired");
            this.Refresh(this.FilePath);
        }

        /// <summary>
        /// //called when config file is refreshed and then pushes values to SIMPL+
        /// </summary>
        private void OnConfigRefresh(Configuration results)
        {
            try
            {
                this.ConfigurationItems.Keys.ToList().ForEach(key =>
                {
                    Item found = null;

                    switch (this.ConfigurationItems[key].Type)
                    {
                        case Util.Signal.Analog:
                            found = results.Analogs.Find(i => i.Name == key);
                            if (found != null) { this.UpdateAnalogItem(this.ConfigurationItems[key].Location, found.GetValueShort()); }
                            break;
                        case Util.Signal.Digital:
                            found = results.Digitals.Find(i => i.Name == key);
                            if (found != null) { this.UpdateDigitalItem(this.ConfigurationItems[key].Location, found.GetValueShort()); }
                            break;
                        case Util.Signal.Serial:
                            found = results.Strings.Find(i => i.Name == key);
                            if (found != null) { this.UpdateStringItem(this.ConfigurationItems[key].Location, found.GetValueString()); }
                            break;
                    }

                    if (this.IsDebug) { if (found != null) { CrestronConsole.PrintLine("Updating {0} Configuration Item {1} @ {2} -> {3}", found.Type, found.Name, found.Location, found.GenericValue); } }
                });
            }
            catch (Exception e)
            {
                if (this.IsDebug) CrestronConsole.PrintLine("Error Updating SIMPL+ Array | {0}", e.Message);
            }
        }
    }
}
