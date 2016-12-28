using System;
using System.Collections.Generic;
using System.Text;

namespace Coet.Server.MethodHandlers
{
    public class CoetLogInfoEntity
    {
        public string Type { get; set; }
        public string JsonInfo { get; set; }
        public string SendIP { get; set; }
        public string SendName { get; set; }
        public string Createdt { get; set; }
    }
}
