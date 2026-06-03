using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrestronIRDecoder.Crestron
{
    public class CrestronIRCommand
    {
        public string Name;
        public byte[] Function;
        public int ID;

        private int _crestronFreq;
        public int CrestronFrequency
        {
            get { return this._crestronFreq; }
            set { this._crestronFreq = value; this.CreateFrequencyValues(); }
        }

        public int FrequencyHertz;
        public int ProntoFrequency;

        public int OneTimeBurstCount;
        public int RepeatBurstCount;

        internal List<short> BurstCodes;
        internal List<byte> CodePattern;

        public int MinimumRepeats;

        private const double CrestronFrequencyDivisor = .9628;
        private const double CrestronFrequencyOffset = .5;
        private const int HertzFrequencyDivisor = 1000000;
        private const double HertzFrequencyMultiplier = .241246;

        public CrestronIRCommand()
        {
            this.Name = "Not Set";
            this.ID = 0;
            this.BurstCodes = new List<short>();
            this.CodePattern = new List<byte>();
        }

        public CrestronIRCommand(string name, int id)
        {
            this.Name = name;
            this.ID = id;
            this.BurstCodes = new List<short>();
            this.CodePattern = new List<byte>();
        }

        private void CreateFrequencyValues()
        {
            this.ProntoFrequency = (int)Math.Round(this.CrestronFrequency / CrestronFrequencyDivisor + CrestronFrequencyOffset);
            this.FrequencyHertz = (int)Math.Round(HertzFrequencyDivisor / (this.ProntoFrequency * HertzFrequencyMultiplier));
        }
    }
}
