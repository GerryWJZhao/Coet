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
        private static Queue<logInfo> _logQueue = new Queue<logInfo>();
        private static string _localLogPath = string.Empty;
        private static Timer _saveTimer = null;
        private static bool _isSaveing = false;

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
                    lock (_logQueue)
                    {
                        _logQueue.Enqueue(loginfo);
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
                if (_saveTimer == null)
                {
                    var conf = new ConfigurationBuilder()
                               .AddJsonFile("AppConfig.json")
                               .Build();
                    _localLogPath = conf.GetSection("AppConfig:LocalLogPath").Value;

                    _saveTimer = new Timer(new TimerCallback(async d =>
                    {
                        if (!_isSaveing)
                        {
                            _isSaveing = true;

                            StringBuilder sb = new StringBuilder();

                            while (_logQueue.Count > 0)
                            {
                                logInfo l = _logQueue.Dequeue();
                                sb.AppendFormat("{0} {1} {2}\r\n", l.Time.ToString(), l.Type, l.Msg);
                            }

                            string logStr = sb.ToString();

                            if (!string.IsNullOrWhiteSpace(logStr))
                            {
                                string dateLocalLogPath = string.Format("{0}_{1}.txt", _localLogPath, DateTime.Now.ToString("yyyyMMdd"));

                                FileStream fs = new FileStream(dateLocalLogPath, FileMode.Append, FileAccess.Write);
                                StreamWriter sr = new StreamWriter(fs);

                                await sr.WriteLineAsync(logStr);
                                Console.WriteLine(logStr);

                                sr.Dispose();
                                fs.Dispose();
                            }

                            _isSaveing = false;
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
