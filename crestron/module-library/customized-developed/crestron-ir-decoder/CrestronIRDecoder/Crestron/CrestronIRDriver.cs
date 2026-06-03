using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CrestronIRDecoder.Crestron
{
    public enum Fields : byte
    {
        FileType = 240,
        Comment = 241, 
        CreationDate = 242,
        Manufacturer = 243,
        DeviceModel = 244,
        RemoteModel = 245,
        MinRepeatFunctionDelay = 246,
        DeviceType = 247,
        F8 = 248,
        F9 = 249,
        FA = 250,
        FB = 251,
        FC = 252,
        ButtonLabelHeader = 253,
        FE = 254,
        FF = 255
    }

    public class CrestronIRDriver
    {
        //always present
        public string       FileType;     //f0
        public string       Comment;      //f1
        public DateTime     CreationDate; //f2
        public string       Manufacturer; //f3
        public string       DeviceModel;  //f4
        public string       RemoteModel;  //f5
        public int          MinRepeats;   //f6
        public int          FunctionDelay;//f6
        public string       DeviceType;   //f7

        //potentially present
        public List<string> F8;
        public List<string> F9;
        public string       FA;
        public string       FB;
        public string       FC;
        public string       FD;
        public string       FE;

        //ff starts the actual IR command data

        //\xff\xLEN\xIDX\xFREQ\xONETIME\xREPEAT\xBURST\xBP1~\xBPN
        
        //\x00 end of file

        public List<CrestronIRCommand> Commands;

        //flag that lets us know weve seen FF at some point in the byte buffer of the IR driver.
        private bool IRFunctionFlag;
        private const int burstCodeStartPosition = 4;

        public CrestronIRDriver()
        {
            this.FileType = "IR";
            this.Comment = String.Empty;
            this.CreationDate = DateTime.Now.Date;
            this.Manufacturer = String.Empty;
            this.DeviceModel = String.Empty;
            this.RemoteModel = String.Empty;
            this.MinRepeats = 0;
            this.FunctionDelay = 0;
            this.DeviceType = String.Empty;
            this.F8 = new List<string>();
            this.F9 = new List<string>();
            this.FA = String.Empty;
            this.FB = String.Empty;
            this.FC = String.Empty;
            this.FD = String.Empty;
            this.FE = String.Empty;
            this.Commands = new List<CrestronIRCommand>();
        }

        public CrestronIRDriver(string path)
        {
            this.FileType = "IR";
            this.Comment = String.Empty;
            this.CreationDate = DateTime.Now.Date;
            this.Manufacturer = String.Empty;
            this.DeviceModel = String.Empty;
            this.RemoteModel = String.Empty;
            this.MinRepeats = 0;
            this.FunctionDelay = 0;
            this.DeviceType = String.Empty;
            this.F8 = new List<string>();
            this.F9 = new List<string>();
            this.FA = String.Empty;
            this.FB = String.Empty;
            this.FC = String.Empty;
            this.FD = String.Empty;
            this.FE = String.Empty;
            this.Commands = new List<CrestronIRCommand>();

            this.ProcessIRFile(path);
        }

        private byte[] TrimByteArray(byte[] array, int start)
        {
            byte[] result = new byte[array.Length - start];

            for(int i = start; i < array.Length; i++)
            {
                result[i - start] = array[i];
            }

            return result;
        }

        private byte[] GetByteBuffer(byte[] array, int len)
        {
            byte[] result = new byte[len];

            for (int i = 0; i < len; i++)
            {
                result[i] = array[i];
            }

            return result;
        }

        private bool EndOfFileCheck(byte[] file, int start)
        {
            //check that the header we are looking for is actually within the bytearray, or else we throw an exception. 
            //(the only time this should fire and return is when we reach the end of the file
            int tempHeaderLocation = start + 1;

            //if the temp header location is greater than or 
            if (tempHeaderLocation >= file.Length)
            {
                Console.WriteLine("End of File: {0}", file[start]);
                //return true, because the first byte is zero, which denotes the end of the file as far as we can tell.
                if (file[start] == 0) return true;
            }

            //return false so caller knows to continue
            return false;
        }

        private string GetStringContent(byte[] content)
        {
            StringBuilder builder = new StringBuilder(content.Length);
            foreach (byte b in content) { builder.Append((char)b); }
            return builder.ToString();
        }

        private List<short> GetBurstCodes(byte[] buffer, int numPairs)
        {
            List<short> burstCodes = new List<short>();
            int bufferPosition = burstCodeStartPosition;
            for (int i = 1; i <= numPairs; i++)
            {
                byte upperByte = buffer[bufferPosition];
                byte lowerByte = buffer[bufferPosition + 1];

                //shift the upper byte 8 bits so its actually in the right place
                int upperByteShifted = upperByte << 8;
                int burstCode = (upperByteShifted | lowerByte);
                //Console.WriteLine("Upper Byte: {0} | Upper Byte Shifted: {1} | LowerByte: {2} | code: {3}", upperByte, upperByteShifted, lowerByte, burstCode);
                burstCodes.Add((short)burstCode);

                //increment by 2 because we are grabbing pairs of bytes
                bufferPosition = bufferPosition + 2;
            }

            return burstCodes;
        }

        private List<byte> GetCodePattern(byte[] functionCodeBuffer)
        {
            List<byte> codePattern = new List<byte>();
            for (int i = 0; i < functionCodeBuffer.Length; i++)
            {
                //the index of the on & off bytes are encoded in the high and low 4 bits of each byte in the code pattern sequence
                byte onByte  = (byte)(functionCodeBuffer[i] >> 4);
                byte offByte = (byte)(functionCodeBuffer[i] & 0x0F);

                codePattern.Add(onByte);
                codePattern.Add(offByte);
            }
            return codePattern;
        }

        private void ProcessFields(byte[] segmentBuffer)
        {
            //write the true byte buffer length
            Console.WriteLine("File Segment Length: {0}", segmentBuffer.Length);
            //get byte zero and compare agains real length
            byte segmentLength = segmentBuffer[0];
            //throw exception if they dont match (shouldnt ever happen)
            if (segmentLength != segmentBuffer.Length) { Console.WriteLine("Danger Will Robinson, Segment Length Byte and actual Segment Length Dont Match"); throw new OverflowException(); }
            //get segment header value
            byte segmentHeader = segmentBuffer[1];
            //write segment header to console
            Console.WriteLine("Segment Header: {0}", (Fields)segmentHeader);
            //Console.WriteLine("Segment Header Value: {0}", segmentHeader);

            try
            {
                switch ((Fields)segmentHeader)
                {
                    case Fields.FileType:
                        this.FileType = this.GetStringContent(this.TrimByteArray(segmentBuffer, 2));
                        break;
                    case Fields.Comment:
                        this.Comment = this.GetStringContent(this.TrimByteArray(segmentBuffer, 2));
                        break;
                    case Fields.CreationDate:
                        this.CreationDate = DateTime.ParseExact(this.GetStringContent(this.TrimByteArray(segmentBuffer, 2)), "MMddyy", System.Globalization.CultureInfo.InvariantCulture);
                        break;
                    case Fields.DeviceModel:
                        this.DeviceModel = this.GetStringContent(this.TrimByteArray(segmentBuffer, 2));
                        break;
                    case Fields.DeviceType:
                        this.DeviceType = this.GetStringContent(this.TrimByteArray(segmentBuffer, 2));
                        break;
                    case Fields.Manufacturer:
                        this.Manufacturer = this.GetStringContent(this.TrimByteArray(segmentBuffer, 2));
                        break;
                    case Fields.MinRepeatFunctionDelay:
                        byte[] info = this.TrimByteArray(segmentBuffer, 2);
                        this.MinRepeats = info[0];
                        this.FunctionDelay = info[1];
                        break;
                    case Fields.RemoteModel:
                        this.RemoteModel = this.GetStringContent(this.TrimByteArray(segmentBuffer, 2));
                        break;
                    case Fields.F8:
                        this.F8.Add(this.GetStringContent(this.TrimByteArray(segmentBuffer, 2)));
                        break;
                    case Fields.F9:
                        this.F9.Add(this.GetStringContent(this.TrimByteArray(segmentBuffer, 2)));
                        break;
                    case Fields.FA:
                        this.FA = this.GetStringContent(this.TrimByteArray(segmentBuffer, 2));
                        break;
                    case Fields.FB:
                        this.FB = this.GetStringContent(this.TrimByteArray(segmentBuffer, 2));
                        break;
                    case Fields.FC:
                        this.FC = this.GetStringContent(this.TrimByteArray(segmentBuffer, 2));
                        break;
                    case Fields.ButtonLabelHeader:
                        //this field is not always present, must be part of an update, as IR drivers still import properly without it.
                        this.FD = this.GetStringContent(this.TrimByteArray(segmentBuffer, 2));
                        break;
                    case Fields.FE:
                        this.FE = this.GetStringContent(this.TrimByteArray(segmentBuffer, 2));
                        break;
                    case Fields.FF:
                        //throw the flag
                        this.IRFunctionFlag = true;
                        break;
                    default:
                        if (this.IRFunctionFlag == false)
                        {
                            Console.WriteLine("Adding Button Label");
                            this.Commands.Add(new CrestronIRCommand(this.GetStringContent(this.TrimByteArray(segmentBuffer, 2)), (int)segmentHeader));
                        }
                        else if (this.IRFunctionFlag)
                        {
                            //get the matching function from when we parsed the buttons, and add the function to it.
                            CrestronIRCommand function = this.Commands.Find(func => func.ID == (int)segmentHeader);
                            Console.WriteLine("Adding IR Burst Codes to Function {0}", function.Name);
                            //trim out the length, and id
                            byte[] functionCodeBuffer = this.TrimByteArray(segmentBuffer, 2);
                            //generate the necessary freqs
                            function.CrestronFrequency = functionCodeBuffer[0];

                            Console.WriteLine("Crestron: {0} | Pronto: {1} | Hertz/GlobalCache: {2}", function.CrestronFrequency, function.ProntoFrequency, function.FrequencyHertz);

                            //store repeat and onetime burst counts
                            function.OneTimeBurstCount = functionCodeBuffer[1];
                            function.RepeatBurstCount = functionCodeBuffer[2];

                            //this containts both the number of repeats and the number of codewords, probably need to bitshift
                            function.MinimumRepeats = functionCodeBuffer[3] >> 4;
                            //trims out the upper 8 bits.
                            int numBurstCodePairs = functionCodeBuffer[3] & 0x0F;

                            Console.WriteLine("Function Min Repeats: {0}, Number Of Code Words: {1}", function.MinimumRepeats, numBurstCodePairs);
                            
                            //get the burst code pairs
                            function.BurstCodes = GetBurstCodes(functionCodeBuffer, numBurstCodePairs);
                            Console.WriteLine("Burst Codes: {0}", String.Join(",", function.BurstCodes.Select(code => code.ToString()).ToArray()));
                            //remove the codes, which are now stored and un-needed from the buffer.
                            //since there is n number of pairs, to get the actual number, we must multiply by 2
                            int endBurstCodes = burstCodeStartPosition + (numBurstCodePairs * 2);
                            Console.WriteLine("Trimming Buffer Starting @ Pos: {0}", endBurstCodes);
                            byte[] codePatternBuffer = this.TrimByteArray(functionCodeBuffer, endBurstCodes);
                            Console.WriteLine("Remaining Characters In Buffer: {0}", codePatternBuffer.Length);
                            //get the code pattern
                            for (int i = 0; i < codePatternBuffer.Length; i++)
                            {
                                Console.Write(codePatternBuffer[i].ToString() + ", ");
                            }

                            function.CodePattern = GetCodePattern(codePatternBuffer);
                            //Console.WriteLine("Code Pattern: {0}", String.Join(",", function.CodePattern.Select(p => p.ToString()).ToArray())); 
                            //this is basically how one converts crestron IR codes to global cache
                            string commandCodePattern = String.Join(",", function.CodePattern.Select(patternItem => function.BurstCodes[patternItem].ToString()).ToArray());
                            //Console.WriteLine("Code Pattern: {0}", commandCodePattern);
                            string globalCacheCommand = String.Format("{0},{1},{2},{3}", function.FrequencyHertz, function.MinimumRepeats, 1, commandCodePattern);
                            Console.WriteLine(globalCacheCommand);
                        }
                        else { Console.WriteLine("Fucked"); }
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception Parsing Segment: {0}", e.Message);
            }
        }

        internal void ProcessIRFile(string path)
        {
            this.IRFunctionFlag = false;
            try
            {
                //open the file, read its contents then close
                byte[] fileArray = System.IO.File.ReadAllBytes(path);
                //a flag that we can send up to exit the loop should something go awry, determine how to fire this later
                bool emergencyFlag = false;

                //for readability purposes later, filestart will always be the first byte in the array
                int fileStart = 0;

                //process until we reach the end of the string, or emergency flag is flown.
                while (fileArray.Length != 0 || emergencyFlag == false)
                {
                    //notify the user of the current length of the file, and the length of the current segment of data
                    Console.WriteLine("File Array Len: {0}", fileArray.Length);

                    //first byte (in a properly formatted file) will always be the length of the segment of data to be processed.
                    byte segmentLen = fileArray[fileStart];

                    //get the segment buffer for processing
                    byte[] segmentBuffer = GetByteBuffer(fileArray, segmentLen);

                    //check if we reached the end of the file, if so, return.
                    if (this.EndOfFileCheck(fileArray, fileStart)) return;

                    //process the buffer, determining what field it represents and the data contained
                    this.ProcessFields(segmentBuffer);

                    //remove what we just processed from the byte array
                    fileArray = TrimByteArray(fileArray, segmentLen);

                    //continue in human readable time.
                    Console.WriteLine("Hit A Key to Continue Parsing");
                    Console.ReadLine();
                }
            }
            catch(FileNotFoundException e)
            {
                Console.WriteLine("File Not Found {0}", e.Message);
            }
        }

        public CrestronIRCommand FindCommandByName(string name)
        {
            CrestronIRCommand found = this.Commands.Find(cmd => cmd.Name.ToLower() == name.ToLower());
            return found;
        }
    }
}
