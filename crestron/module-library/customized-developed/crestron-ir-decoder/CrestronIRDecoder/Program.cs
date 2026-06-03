using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CrestronIRDecoder.Crestron;

namespace CrestronIRDecoder
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter the full path to the IR driver you wish to parse");
            string driverPath = Console.ReadLine();
            CrestronIRDriver driver = new CrestronIRDriver();
            try
            {
                driver = new Crestron.CrestronIRDriver(driverPath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                Console.WriteLine(e.Source);
                Console.WriteLine(e.InnerException);
                Console.WriteLine(e.Data);
            }
            finally
            {
                Console.WriteLine("\r\nDriver Details\r\n\tFile Type: {0}\r\n\tComment: {1}\r\n\tDate Created: {2}\r\n\tRemote Model:{3}\r\n\tManu:{4}\r\n\tDevModel:{5}\r\n\tDevType:{6}\r\n\tMinRepeats:{7}\r\n\tFuncDelay:{8}", driver.FileType, driver.Comment, driver.CreationDate, driver.RemoteModel, driver.Manufacturer, driver.DeviceModel, driver.DeviceType, driver.MinRepeats, driver.FunctionDelay);
                Console.WriteLine();
                driver.Commands.ForEach(delegate(CrestronIRCommand f)
                {
                    Console.WriteLine("Function: {0} - {1}", f.ID, f.Name);
                });
                Console.WriteLine("Press Any Key To Exit");
                Console.ReadLine();
            }
        }
    }
}
