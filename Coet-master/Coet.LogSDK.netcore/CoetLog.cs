using Cote.GrpcProto;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Coet.LogSDK
{
    public class CoetLogSDK
    {
        static Queue<CoetLogInfo> SendLogQueue = new Queue<CoetLogInfo>();
        static Timer sendTimer = null;
        static bool isSending = false;

        public static void Log(string type, string jsonInfo, string sendIP, string sendName)
        {
            Task.Run(() => {
                lock (SendLogQueue)
                {
                    SendLogQueue.Enqueue(new CoetLogInfo {
                        Type = type,
                        JsonInfo = jsonInfo,
                        SendIP = sendIP,
                        SendName = sendName
                    });
                }
            });
        }

        public static void Start(string coetServerUrl)
        {
            try
            {
                if (sendTimer == null)
                {
                    Timer saveTimer = new Timer(new TimerCallback(async d =>
                    {
                        if (!isSending)
                        {
                            isSending = true;

                            Channel channel = new Channel(coetServerUrl, ChannelCredentials.Insecure);
                            var client = new CoetLog.CoetLogClient(channel);

                            SaveCoteLogParm sp = new SaveCoteLogParm();

                            while (SendLogQueue.Count > 0)
                            {
                                CoetLogInfo ci = SendLogQueue.Dequeue();
                                sp.CoetLogInfos.Add(ci);
                            }

                            var reply = await client.SaveLogAsync(sp);

                            if (reply.ExecuteCount < sp.CoetLogInfos.Count)
                            {
                                foreach (var item in sp.CoetLogInfos)
                                {
                                    lock (SendLogQueue)
                                    {
                                        SendLogQueue.Enqueue(item);
                                    }
                                }
                            }

                            isSending = false;
                        }
                    }), new object(), 0, 10000);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
