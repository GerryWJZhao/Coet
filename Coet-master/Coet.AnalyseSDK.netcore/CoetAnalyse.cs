using Cote.GrpcProto;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Coet.AnalyseSDK
{
    public class CoetAnalyseSDK
    {
        CoetAnalyse.CoetAnalyseClient client;

        public CoetAnalyseSDK(string coetServerUrl)
        {
            Channel channel = new Channel(coetServerUrl, ChannelCredentials.Insecure);
            client = new CoetAnalyse.CoetAnalyseClient(channel);
        }

        public async Task<CoetLogSearchResult> GetLogAsync(string startDateTime, string endDateTime)
        {
            var reply = await client.GetLogAsync(new CoetLogSearchParm
            {
                StartDateTime = startDateTime,
                EndDateTime = endDateTime
            });

            return reply;
        }
    }
}
