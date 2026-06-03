using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quartz;

namespace TestProject
{
    class Program
    {
        static void Main(string[] args)
        {
            QuartzProtocol.IsWindows = true;
            QuartzProtocol.IsDebug = true;
            QuartzServerDevice magnumDevice = new QuartzServerDevice();
            magnumDevice.ID = 1;
            QuartzServerRouter virtualRouter = new QuartzServerRouter();
            virtualRouter.ServerDeviceID = 1;
            Console.ReadLine();
            virtualRouter.ServerDeviceID = 0;
            Console.ReadLine();
            virtualRouter.ServerDeviceID = 1;
            Console.ReadLine();
        }
    }
}
