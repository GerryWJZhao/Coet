using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Coet.Server.Infrastructure
{
    public class CoetLocalLog
    {
        static Queue<logInfo> logQueue = new Queue<logInfo>();
        static string localLogPath = string.Empty;
        static Timer saveTimer = null;
        static bool isSaveing = false;

        public static void Info(string msg)
        {
            StartSaveTimer();
            AddLocalLog("info", msg);
        }

        public static void Error(string msg)
        {
            StartSaveTimer();
            AddLocalLog("error", msg);
        }

        private static void AddLocalLog(string type, string msg)
        {
            try
            {
                logInfo loginfo = new logInfo
                {
                    Type = type,
                    Msg = msg,
                    Time = DateTime.Now
                };

                Task.Run(() => {
                    lock (logQueue)
                    {
                        logQueue.Enqueue(loginfo);
                    }
                });
            }
            catch (Exception)
            {
            }
        }

        private static void StartSaveTimer()
        {
            try
            {
                if (saveTimer == null)
                {
                    Timer saveTimer = new Timer(new TimerCallback(async d =>
                    {
                        if (!isSaveing)
                        {
                            isSaveing = true;

                            if (string.IsNullOrEmpty(localLogPath))
                            {
                                var conf = new ConfigurationBuilder()
                                           .AddJsonFile("AppConfig.json")
                                           .Build();
                                localLogPath = conf.GetSection("AppConfig:LocalLogPath").Value;
                            }

                            string dateLocalLogPath = string.Format("{0}_{1}.txt", localLogPath, DateTime.Now.ToString("yyyyMMdd"));

                            FileStream fs = new FileStream(dateLocalLogPath, FileMode.OpenOrCreate, FileAccess.Write);

                            while (logQueue.Count > 0)
                            {
                                logInfo l = logQueue.Dequeue();

                                if (l != null)
                                {
                                    string logStr = string.Format("{0} {1} {2}", l.Time.ToString(), l.Type, l.Msg);
                                    StreamWriter sr = new StreamWriter(fs);
                                    await sr.WriteLineAsync(logStr);
                                    sr.Dispose();
                                }
                            }

                            fs.Dispose();

                            isSaveing = false;
                        }

                    }), new object(), 0, 5000);
                }
            }
            catch (Exception)
            {
            }
        }

    }

    class logInfo
    {
        public string Type { get; set; }
        public string Msg { get; set; }
        public DateTime Time { get; set; }
    }
}
