using Coet.GrpcProto;
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
        const int queueMaxCount = 50000;
        const int sendMaxCount = 10000;
        const int timerinterval = 10000;

        static Queue<CoetLogInfo> SendLogQueue = new Queue<CoetLogInfo>();
        static Timer sendTimer = null;
        static bool isSending = false;

        public static void Log(string type, string jsonInfo, string sendIP, string sendName)
        {
            Task.Run(() => {
                lock (SendLogQueue)
                {
                    try
                    {
                        if (SendLogQueue.Count > queueMaxCount)
                        {
                            SendLogQueue.Dequeue();
                        }
                        SendLogQueue.Enqueue(new CoetLogInfo
                        {
                            Type = type,
                            JsonInfo = jsonInfo,
                            SendIP = sendIP,
                            SendName = sendName
                        });
                    }
                    catch (Exception)
                    {
                    }
                }
            });
        }

        public static void Start(string coetServerUrl)
        {
            try
            {
                if (sendTimer == null)
                {
                    sendTimer = new Timer(new TimerCallback(async d =>
                    {
                        if (!isSending)
                        {
                            isSending = true;

                            SaveCoetLogParm sp = new SaveCoetLogParm();

                            while (SendLogQueue.Count > 0 && sp.CoetLogInfos.Count < sendMaxCount)
                            {
                                CoetLogInfo ci = SendLogQueue.Dequeue();

                                if (ci != null)
                                {
                                    sp.CoetLogInfos.Add(ci);
                                }
                            }

                            if (sp.CoetLogInfos.Count > 0)
                            {
                                Channel channel = new Channel(coetServerUrl, ChannelCredentials.Insecure);
                                var client = new CoetLog.CoetLogClient(channel);
                                int executeCount = 0;
                                try
                                {
                                    var reply = await client.SaveLogAsync(sp);
                                    executeCount = reply.ExecuteCount;
                                }
                                catch (Exception) { }

                                if (executeCount < sp.CoetLogInfos.Count)
                                {
                                    foreach (var item in sp.CoetLogInfos)
                                    {
                                        lock (SendLogQueue)
                                        {
                                            SendLogQueue.Enqueue(item);
                                        }
                                    }
                                }
                            }

                            isSending = false;
                        }
                    }), new object(), 0, timerinterval);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
