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
            AddLocalLog("info", msg);
        }

        public static void Error(string msg)
        {
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

        public static void StartSave()
        {
            try
            {
                if (saveTimer == null)
                {
                    var conf = new ConfigurationBuilder()
                               .AddJsonFile("AppConfig.json")
                               .Build();
                    localLogPath = conf.GetSection("AppConfig:LocalLogPath").Value;

                    Timer saveTimer = new Timer(new TimerCallback(async d =>
                    {
                        if (!isSaveing)
                        {
                            isSaveing = true;

                            StringBuilder sb = new StringBuilder();

                            while (logQueue.Count > 0)
                            {
                                logInfo l = logQueue.Dequeue();
                                sb.AppendFormat("{0} {1} {2}\r\n", l.Time.ToString(), l.Type, l.Msg);
                            }

                            string logStr = sb.ToString();

                            if (!string.IsNullOrWhiteSpace(logStr))
                            {
                                string dateLocalLogPath = string.Format("{0}_{1}.txt", localLogPath, DateTime.Now.ToString("yyyyMMdd"));

                                FileStream fs = new FileStream(dateLocalLogPath, FileMode.Append, FileAccess.Write);
                                StreamWriter sr = new StreamWriter(fs);

                                await sr.WriteLineAsync(logStr);
                                Console.WriteLine(logStr);

                                sr.Dispose();
                                fs.Dispose();
                            }

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
