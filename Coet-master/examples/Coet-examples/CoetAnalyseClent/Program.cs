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
            ca.GetLogAsync("2016-12-31 09:25:00", "2017-1-1 13:20:00", CoetAnalyseAddPart.AddSeconds, (c) =>
            {
                Console.WriteLine(c);
                return new object();
            }, 5);

            Console.ReadLine();
        }
    }
}
