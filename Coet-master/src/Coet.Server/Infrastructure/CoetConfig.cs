using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coet.Server.Infrastructure
{
    internal class CoetConfig
    {
        internal static List<string> GetAllowConnectIP()
        {
            List<string> allowIPList = new List<string>();

            var conf = new ConfigurationBuilder()
                .AddJsonFile("AppConfig.json")
                .Build();

            int i = 0;
            while (true)
            {
                string allowIP = conf.GetSection(string.Format("AppConfig:AllowConnectIP:{0}", i)).Value;
                if (allowIP != null)
                {
                    allowIPList.Add(allowIP);
                }
                else
                {
                    break;
                }
                i++;
            }

            return allowIPList;
        }
    }
}
