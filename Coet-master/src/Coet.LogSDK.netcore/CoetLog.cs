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
        private const int _queueMaxCount = 50000;
        private const int _sendMaxCount = 10000;
        private const int _timerinterval = 1000;

        private static Queue<CoetLogInfo> _sendLogQueue = new Queue<CoetLogInfo>();
        private static Timer _sendTimer = null;
        private static bool _isSending = false;

        public static void Log(string type, string jsonInfo, string sendIP, string sendName)
        {
            Task.Run(() => {
                lock (_sendLogQueue)
                {
                    try
                    {
                        if (_sendLogQueue.Count > _queueMaxCount)
                        {
                            _sendLogQueue.Dequeue();
                        }
                        _sendLogQueue.Enqueue(new CoetLogInfo
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
                Channel channel = new Channel(coetServerUrl, ChannelCredentials.Insecure);
                var client = new CoetLog.CoetLogClient(channel);

                if (_sendTimer == null)
                {
                    _sendTimer = new Timer(new TimerCallback(async d =>
                    {
                        if (!_isSending)
                        {
                            _isSending = true;

                            SaveCoetLogParm sp = new SaveCoetLogParm();

                            while (_sendLogQueue.Count > 0 && sp.CoetLogInfos.Count < _sendMaxCount)
                            {
                                CoetLogInfo ci = _sendLogQueue.Dequeue();

                                if (ci != null)
                                {
                                    sp.CoetLogInfos.Add(ci);
                                }
                            }

                            if (sp.CoetLogInfos.Count > 0)
                            {
                                int executeCount = 0;
                                try
                                {
                                    var reply = await client.SaveLogAsync(sp);
                                    executeCount = reply.ExecuteCount;
                                    Console.WriteLine(executeCount);
                                }
                                catch (Exception)
                                {
                                    channel.ShutdownAsync().Wait();
                                }

                                if (executeCount < sp.CoetLogInfos.Count)
                                {
                                    foreach (var item in sp.CoetLogInfos)
                                    {
                                        lock (_sendLogQueue)
                                        {
                                            _sendLogQueue.Enqueue(item);
                                        }
                                    }
                                }
                            }

                            _isSending = false;
                        }
                    }), new object(), 0, _timerinterval);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
