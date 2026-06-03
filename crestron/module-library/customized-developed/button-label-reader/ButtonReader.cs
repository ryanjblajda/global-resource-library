using System;
using System.Text;
using Crestron.SimplSharp; // For Basic SIMPL# Classes
using System.Collections.Generic;
using Crestron.SimplSharp.CrestronIO;
using Newtonsoft.Json;
using FileOperations;
using Util;

namespace ButtonLabelReader
{
    public class Root
    {
        [JsonProperty("buttons")]
        public List<string> Buttons { get; set; }
    }

    public class ButtonLabelArgs : EventArgs
    {
        public string[] Labels { get; private set; }
        public ushort NumLabels { get; private set; }
        
        public ButtonLabelArgs()
        {
        }

        public ButtonLabelArgs(List<string> labels)
        {
            this.Labels = labels.ToArray();
            this.NumLabels = (ushort)labels.Count;
        }
    }

    public class ButtonReader
    {
        public delegate void ButtonsUpdated(object sender, ButtonLabelArgs args);
        public event ButtonsUpdated OnButtonsUpdated;

        public delegate void Saved(ushort status);
        public Saved SaveOperationResult { get; set; }

        private FileReaderWriter JsonFile = new FileReaderWriter();

        public ButtonReader()
        {
            this.Buttons = new List<string>();
        }

        private List<string> _buttons;
        public List<string> Buttons 
        {
            get { return this._buttons; }
            private set
            {
                if (value != null)
                {
                    this._buttons = value;
                    if(this.OnButtonsUpdated != null) this.OnButtonsUpdated(this, new ButtonLabelArgs(this.Buttons));
                }
            }
        }

        private string _path;
        public string FilePath 
        {
            get { return this._path; }
            set 
            {
                if (value != null)
                {
                    this._path = value;
                }
            } 
        }

        public void Refresh(string path)
        {
            string contents = JsonFile.Refresh(path);
            if (contents != null)
            {
                Root fileContents = JsonConvert.DeserializeObject<Root>(contents);
                this.Buttons = fileContents.Buttons;
            }
        }

        private void Save(string path)
        {
            Root toSave = new Root();
            toSave.Buttons = this.Buttons;
            bool result = JsonFile.Save(path, JsonConvert.SerializeObject(toSave));
            if (result == true) if (this.SaveOperationResult != null) this.SaveOperationResult(Util.Conversion.ConvertToSignal(result));
        }

        public void UpdateLabel(ushort btnIndex, string label)
        {
            if (btnIndex < this.Buttons.Count)
            {
                this.Buttons[btnIndex] = label;
            }
            else
            {
                this.Buttons.Add(label);
            }

            this.Save(this.FilePath);
            this.Refresh(this.FilePath);
        }
    }
}
