using Coet.Server.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coet.Server.MethodHandlers
{
    internal class Common
    {
        internal static bool isAllowHost(string peer)
        {
            try
            {
                peer = peer.Split(':')[0];
                List<string> list = CoetConfig.GetAllowConnectIP();
                if (list.Any(d => d == peer))
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
