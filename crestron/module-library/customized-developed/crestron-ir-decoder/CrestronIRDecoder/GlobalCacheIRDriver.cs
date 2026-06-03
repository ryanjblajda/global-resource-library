using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CrestronIRDecoder.Crestron;

namespace CrestronIRDecoder
{
    class GlobalCacheIRDriver
    {
        public ushort Parent
        {
            get;
            set;
        }

        public ushort ParentIRPort
        {
            get;
            set;
        }

        public string IRFilePath
        {
            get;
            set;
        }

        //retrieve all the commands that the user defined that should be called from the driver
        public delegate string GetCommandNames(int parameter);
        public GetCommandNames GetCommandNamesFromParameters { get; set; }
        
        private List<string> CommandNames;
        private Crestron.CrestronIRDriver Driver;

        public GlobalCacheIRDriver()
        {
            this.CommandNames = new List<string>();
            this.Driver = new CrestronIRDecoder.Crestron.CrestronIRDriver();
            for (int i = 1; i <= 32; i++) { this.CommandNames.Add(GetCommandNamesFromParameters(i)); }
        }

        public void SendCommand(ushort cmd)
        {
            //list indexs are zero based, SIMPL+ is not, so we decrement to correct for it.
            cmd--;
            if (cmd < CommandNames.Count)
            {
                CrestronIRCommand command = this.Driver.FindCommandByName(CommandNames[cmd]);
                //generate the global cache on/off pattern
                string commandCodePattern = String.Join(",", command.CodePattern.Select(patternItem => command.BurstCodes[patternItem].ToString()).ToArray());
                string globalCacheCommand = String.Format("{0},{1},{2},{3}", command.FrequencyHertz, command.MinimumRepeats, 1, commandCodePattern);

                //send command to parent module
            }
        }
    }
}
