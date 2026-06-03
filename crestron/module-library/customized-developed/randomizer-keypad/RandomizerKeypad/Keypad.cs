using System;
using System.Text;
using System.Collections.Generic;
using Crestron.SimplSharp;
using System.Linq;                          				// For Basic SIMPL# Classes

namespace RandomizerKeypad
{
    public class Keypad
    {
        public string Passcode { get; set; }

        private string userpasscode;
        private string UserPasscodeEntry
        {
            get { return userpasscode; }
            set 
            { 
                if (value != null)
                {
                    this.userpasscode = value;
                    if (this.PasscodeMaskChanged != null) this.PasscodeMaskChanged(this, new StringEventArgs(new string('*', this.userpasscode.Length)));
                    if (this.PasscodeRealChanged != null) this.PasscodeRealChanged(this, new StringEventArgs(this.userpasscode));
                }
            }
        }

        private List<int> currentkeypad;
        public  List<int> CurrentKeypad 
        {
            get { return this.currentkeypad; }
            set 
            {
                //check if value is null
                if (value != null) this.currentkeypad = value;
                
                //generate a new empty string list, then assign our list of ints to it.
                List<string> KeypadTexts = new List<string>();
                this.currentkeypad.ForEach(item => KeypadTexts.Add(item.ToString()));

                //fire off an event to update the UI
                if (this.RandomizeComplete != null) this.RandomizeComplete(this, new StringArrayEventArgs(KeypadTexts));
            }
        }

        public delegate void OnStringPayloadEvent(object sender, StringEventArgs args);
        public event OnStringPayloadEvent PasscodeMaskChanged;
        public event OnStringPayloadEvent PasscodeRealChanged;

        public delegate void OnStringArrayPayloadEvent(object sender, StringArrayEventArgs args);
        public event OnStringArrayPayloadEvent RandomizeComplete;

        public delegate void OnDigitalPayloadEvent(object sender, DigitalEventArgs args);
        public event OnDigitalPayloadEvent PasscodeEntrySuccess;
        public event OnDigitalPayloadEvent PasscodeEntryFailure;

        public Keypad()
        {
        }

        public void Initialize()
        {
            this.UserPasscodeEntry = String.Empty;
            this.Randomize();
        }

        public void Delete()
        {
            //subtract 1 character from the current passcode string
            if (UserPasscodeEntry != null) if (UserPasscodeEntry.Length != 0) UserPasscodeEntry = UserPasscodeEntry.Substring(0, UserPasscodeEntry.Length - 1);
        }

        public void Clear()
        {
            if (UserPasscodeEntry != null) UserPasscodeEntry = "";
        }

        public void KeyPress(ushort key)
        {
            //check if the current keypad is null, if so generate one
            if (this.CurrentKeypad == null) this.Randomize();
            
            //append key entry
            this.UserPasscodeEntry += this.CurrentKeypad[key - 1].ToString();
        }

        public void SubmitPasscode()
        {
            if (this.UserPasscodeEntry != null && this.Passcode != null)
            {
                if (this.UserPasscodeEntry == this.Passcode) this.Success();
                else this.Failure();

                //clear user passcode entry
                this.UserPasscodeEntry = String.Empty;
            }
        }

        private void Success()
        {
            //fire off success event
            if (this.PasscodeEntrySuccess != null) this.PasscodeEntrySuccess(this, new DigitalEventArgs(1));
            //randomize keypad
            this.Randomize();
            //fire timer to hide text later
            CTimer Hider = new CTimer(this.OnTimerExpired, "success", 2000);
        }

        private void Failure()
        {
            //fire off failure event
            if (this.PasscodeEntryFailure != null) this.PasscodeEntryFailure(this, new DigitalEventArgs(1));
            //fire timer to hide text later
            CTimer Hider = new CTimer(this.OnTimerExpired, "failure", 2000);
        }

        private void OnTimerExpired(object callback)
        {
            try
            {
                string result = (string)callback;
                //CrestronConsole.PrintLine(result);

                if (result == "success")
                {
                    if (this.PasscodeEntrySuccess != null)
                    {
                        this.PasscodeEntrySuccess(this, new DigitalEventArgs(0));
                        //CrestronConsole.PrintLine("Firing Hide Success");
                    }
                }
                else if (result == "failure")
                {
                    if (this.PasscodeEntryFailure != null)
                    {
                        this.PasscodeEntryFailure(this, new DigitalEventArgs(0));
                        //CrestronConsole.PrintLine("Firing Hide Failure");
                    }
                }
                else
                {
                    CrestronConsole.PrintLine("{0} not captured in if statement", result);
                }
            }
            catch (Exception e)
            {
                CrestronConsole.PrintLine(e.Message);
            }
        }

        private void Randomize()
        {
            var array = Enumerable.Range(0, 10).ToArray();
            var _rng = new Random();

            for (int n = array.Count(); n > 1;)
            {
                int k = _rng.Next(n);
                --n;
                var temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }

            this.CurrentKeypad = array.ToList();
        }
    }
}
