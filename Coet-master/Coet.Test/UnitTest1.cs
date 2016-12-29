using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Coet.AnalyseSDK;
using Cote.GrpcProto;
using System.Threading.Tasks;

namespace Coet.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task TestMethod1Async()
        {
            CoetAnalyseSDK cs = new CoetAnalyseSDK("");
            CoetLogSearchResult cr = await cs.GetLogAsync("", "");
        }
    }
}
