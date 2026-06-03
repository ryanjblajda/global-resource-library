using System;
using System.Text;
using Crestron.SimplSharp;      // For Basic SIMPL# Classes

namespace FormattedTimeDateString
{
    public delegate void StringPayloadEvent(object sender, StringPayloadEventArgs args);
    public delegate void DigitalAnalogEvent(object sender, DigitalAnalogPayloadEventArgs args);

    public class Formatter
    {
        private CTimer TimeKeeper;
        private const long DueTime = 1000;

        private string CurrentDateTimeFormatted;

        private string _format;
        public string Format
        {
            get { return this._format; }
            set
            {
                if ((value != this._format) && (value != String.Empty)) {
                    this._format = value;
                    //reset the timer
                    this.TimeKeeper.Reset(0, DueTime);
                }
            }
        }

        public event StringPayloadEvent UpdateFormattedTime;
        public event DigitalAnalogEvent Error;

        public Formatter()
        {
            this.TimeKeeper = new CTimer(this.OnTimeKeeperExpired, Timeout.Infinite, DueTime);
        }

        private void OnTimeKeeperExpired(object sender)
        {
            if (this.Format != String.Empty) { this.UpdateTime(); }
        }

        private void UpdateTime()
        {
            DateTime now = DateTime.Now;
            string formatted = String.Empty;
            bool failure = true;

            try
            {
                formatted = now.ToString(this.Format);
                failure = false;
            }
            catch (Exception e)
            {
                formatted = e.Message;
            }

            //check if the string is different, that way we arent updating simpl all the time
            if (CurrentDateTimeFormatted != formatted)
            {
                this.CurrentDateTimeFormatted = formatted;
                //update the formatted time
                this.UpdateFormattedTime(this, new StringPayloadEventArgs(formatted));
            }

            //update the error status based on success or failure
            this.Error(this, new DigitalAnalogPayloadEventArgs(Util.ConvertToUshort(failure)));
        }
    }
}