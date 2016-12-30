using Coet.Server.MethodHandlers;
using Coet.GrpcProto;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using System.Threading;

class CoetServer
{
    static void Main(string[] args)
    {
        Console.WriteLine("Begin Start CoetServer....");

        var conf = new ConfigurationBuilder()
           .AddJsonFile("AppConfig.json")
           .Build();

        string listenIP = conf.GetSection("AppConfig:Listen:IP").Value;
        int logPort = int.Parse(conf.GetSection("AppConfig:Listen:LogPort").Value);
        int analysePort = int.Parse(conf.GetSection("AppConfig:Listen:AnalysePort").Value);

        AutoResetEvent autoReset = new AutoResetEvent(false);

        Server LogServer = new Server
        {
            Services = { CoetLog.BindService(new LogHandlers()) },
            Ports = { new ServerPort(listenIP, logPort, ServerCredentials.Insecure) }
        };
        LogServer.Start();

        Console.WriteLine("LogServer Started");

        Server AnalyseServer = new Server
        {
            Services = { CoetAnalyse.BindService(new AnalyseHandlers()) },
            Ports = { new ServerPort(listenIP, analysePort, ServerCredentials.Insecure) }
        };
        AnalyseServer.Start();

        Console.WriteLine("AnalyseServer Started");

        autoReset.WaitOne();
    }
}