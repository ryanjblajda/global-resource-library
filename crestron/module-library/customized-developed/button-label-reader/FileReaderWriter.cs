using System;
using System.Text;
using Crestron.SimplSharp;// For Basic SIMPL# Classes
using Crestron.SimplSharp.CrestronIO;
using Newtonsoft.Json;

namespace FileOperations
{
    public class FileReaderWriter
    {
        private bool FileInUse;

        public string Refresh(string path)
        {
            string result = null;
            if (File.Exists(path))
            {
                try
                {
                    if (FileInUse == false)
                    {
                        CrestronConsole.PrintLine("File Not Currently In Use, Refresh Action Proceeding");
                        FileInUse = true;
                        FileStream file = File.Open(path, FileMode.Open);
                        StreamReader fileReader = new StreamReader(file);
                        lock (file)
                        {
                            if (file.CanRead)
                            {
                                try
                                {
                                    result = fileReader.ReadToEnd();
                                }
                                catch (Exception e)
                                {
                                    CrestronConsole.PrintLine("Error Reading File @ {0} | {1}", path, e.Message);
                                }
                                finally
                                {
                                    file.Close();
                                    FileInUse = false;
                                }

                            }
                        }
                    }
                    else { CrestronConsole.PrintLine("File Not Currently In Use, Refresh Action Proceeding"); }
                }
                catch (Exception e)
                {
                    CrestronConsole.PrintLine("Error Opening File @ {0} | {1}", path, e.Message);
                }
            }
            else
            {
                CrestronConsole.PrintLine("File @ {0} Does Not Exist", path);
            }

            return (result);
        }

        public bool Save(string path, string text)
        {
            FileStream file;
            bool success = false;
            try
            {
                if (FileInUse == false)
                {
                    CrestronConsole.PrintLine("File Not Currently In Use, Save Action Proceeding");
                    FileInUse = true;
                    if (File.Exists(path))
                    {
                        file = File.Open(path, FileMode.Truncate);
                        CrestronConsole.PrintLine("File @ {0}, Attempting To Open & Truncate...", path);
                    }
                    else
                    {
                        CrestronConsole.PrintLine("File @ {0} Does Not Exist, Creating One Now...", path);
                        file = File.Create(path);
                    }

                    if (file != null)
                    {
                        lock (file)
                        {
                            if (file.CanWrite)
                            {
                                try
                                {
                                    file.Position = 0;
                                    if (file != null)
                                    {
                                        StreamWriter jsonWriter = new StreamWriter(file);
                                        //CrestronConsole.PrintLine("TO WRITE" + json);
                                        jsonWriter.Write(text);
                                        jsonWriter.Flush();
                                        success = true;
                                    }
                                }
                                catch (Exception e)
                                {
                                    CrestronConsole.PrintLine("Error Writing File @ {0} | {1}", path, e.Message);
                                }
                                finally
                                {
                                    file.Close();
                                    FileInUse = false;
                                }
                            }
                        }
                    }
                    else { CrestronConsole.PrintLine("Created File Null"); }
                }
                else { CrestronConsole.PrintLine("File Currently In Use, Save Action Ignored"); }
            }
            catch (Exception e)
            {
                CrestronConsole.PrintLine("Error Opening File For Writing @ {0} | {1}", path, e.Message);
            }

            return success;
        }
    }
}
