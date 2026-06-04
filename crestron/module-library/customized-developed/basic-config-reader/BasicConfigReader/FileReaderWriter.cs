using System;
using System.Text;
using Crestron.SimplSharp;// For Basic SIMPL# Classes
using Crestron.SimplSharp.CrestronIO;
using Newtonsoft.Json;

namespace FileOperations
{
    public class FileReaderWriter
    {
        public string Refresh(string path)
        {
            string result = null;
            try
            {
                FileStream file = File.Open(path, FileMode.Open);
                StreamReader fileReader = new StreamReader(file);

                if (file.CanRead)
                {
                    try { result = fileReader.ReadToEnd(); }
                    catch (Exception e) { CrestronConsole.PrintLine("Error Reading File @ {0} | {1}", path, e.Message); }
                    finally { file.Close(); }
                }

                file.Close();
            }
            catch (Exception e) { CrestronConsole.PrintLine("Error Opening File @ {0} | {1}", path, e.Message); }

            return (result);
        }

        public bool Save(string path, string text)
        {
            FileStream file;
            bool success = false;
            
            try
            {
                file = File.Open(path, FileMode.Create);
                if (file.CanWrite)
                {
                    try
                    {
                        if (file != null)
                        {
                            StreamWriter jsonWriter = new StreamWriter(file);
                            jsonWriter.Write(text);
                            jsonWriter.Flush();
                            success = true;
                        }
                    }
                    catch (Exception e) { CrestronConsole.PrintLine("Error Writing File @ {0} | {1}", path, e.Message); }
                    finally { file.Close(); }
                }
            }
            catch (Exception e) { CrestronConsole.PrintLine("Error Opening File For Writing @ {0} | {1}", path, e.Message); }

            return success;
        }
    }
}
