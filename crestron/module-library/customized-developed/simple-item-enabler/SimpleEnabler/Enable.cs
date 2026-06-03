using System;
using System.Text;
using Crestron.SimplSharp;   // For Basic SIMPL# Classes
using FileOperations;
using System.Collections.Generic;
using Newtonsoft.Json;
using Util;

namespace SimpleEnable
{
    public class Item
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("enable")]
        public bool Enabled { get; set; }
    }

    internal class Root
    {
        [JsonProperty("items")]
        public List<Item> Items { get; set; }
    }

    public class UpdateItemsArgs : EventArgs
    {
        public ushort[] EnableStatus;
        public ushort EnableStatusCount;

        public UpdateItemsArgs()
        {
        }

        public UpdateItemsArgs(ushort[] items)
        {
            EnableStatus = items;
            EnableStatusCount = (ushort)items.Length;         
        }
    }

    public class Enable
    {

        public delegate void UpdateItemsDelegate(object sender, UpdateItemsArgs args);
        public event UpdateItemsDelegate UpdateItems;

        public ushort DefinedOutputs;

        private List<Item> _items;
        private List<Item> Items
        {
            get { return this._items; }
            set { this._items = value; this.PushNewItems(); }
        }

        private Dictionary<string, ushort> ItemLocations;

        private FileReaderWriter JsonFile = new FileReaderWriter();

        private string _path;
        public string FilePath
        {
            get { return this._path; }
            set
            {
                this._path = value;
                this.JsonFile.Refresh(this.FilePath);
            }
        }

        public Enable()
        {
            this.Items = new List<Item>();
            this.ItemLocations = new Dictionary<string, ushort>();
        }

        public void Refresh(string path)
        {
            string contents = JsonFile.Refresh(path);
            if (contents != null)
            {
                Root fileContents = JsonConvert.DeserializeObject<Root>(contents);
                this.Items = fileContents.Items;
            }
        }

        public void AddItem(ushort index, string name)
        {
            if (!this.ItemLocations.ContainsKey(name)) this.ItemLocations.Add(name, index);
        }

        private void PushNewItems()
        {
            if(this.ItemLocations != null && this.Items.Count > 0)
            {
                ushort[] StateContainer = new ushort[this.DefinedOutputs];

                this.Items.ForEach(delegate(Item i)
                {
                    if (this.ItemLocations.ContainsKey(i.Name))
                    {
                        try
                        {
                            StateContainer[this.ItemLocations[i.Name]] = Conversion.ConvertToSignal(i.Enabled);
                        }
                        catch (IndexOutOfRangeException)
                        {
                            CrestronConsole.PrintLine("Enable Module Encountered Index Out Of Range { IDX: {0} | COUNT: {1} }", this.ItemLocations[i.Name], this.ItemLocations.Count);
                        }
                    }
                });

                if (this.UpdateItems != null) this.UpdateItems(this, new UpdateItemsArgs(StateContainer));
            }
        }
    }
}
