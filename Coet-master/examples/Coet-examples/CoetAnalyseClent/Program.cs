using Coet.AnalyseSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CoetAnalyseClent
{
    class Program
    {
        static void Main(string[] args)
        {
            CoetAnalyseSDK ca = new CoetAnalyseSDK("192.168.1.104:50052");
            ca.GetLogAsync("2017-1-2 20:22:00", "2017-1-2 20:30:00", CoetAnalyseAddPart.AddSeconds, (c) =>
            {
                Console.WriteLine(c);
                return new object();
            }, "ALL", 5);

            Console.ReadLine();
        }
    }
}
