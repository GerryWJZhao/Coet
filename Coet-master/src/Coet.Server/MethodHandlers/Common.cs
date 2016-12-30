using Coet.Server.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coet.Server.MethodHandlers
{
    internal class Common
    {
        internal static bool isAllowHost(string host)
        {
            try
            {
                host = host.Substring(0, host.IndexOf(":"));
                List<string> list = CoetConfig.GetAllowConnectIP();
                if (list.Any(d => d == host))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
