using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DynamicAVConfiguration.Members;
using DynamicAVConfiguration.Configuration;

namespace DebugProject
{
    class Program
    {
        static void Main(string[] args)
        {
            Configuration.EnableDebug(1);
            AVSystem system = new AVSystem();
            Configuration.AVSystem = system;
            Source source = new Source(1, "Apple TV", 1, 1, new List<Route>() { new Route(1, 1, 0, true) });
            system.Sources.Add(source);

            system.Matrices.Add(new VideoMatrix(1));

            system.Matrices[0].UpdateInputOnlineStatus(1, 1);
            system.Matrices[0].UpdateInputOnlineStatus(1, 0);
            system.Matrices[0].UpdateInputDetectionStatus(1, 1);
            system.Matrices[0].UpdateInputDetectionStatus(1, 0);



            Console.ReadLine();
        }
    }
}