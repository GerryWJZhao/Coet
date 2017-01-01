using Coet.GrpcProto;
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

        public async void GetLogAsync(string startDateTime, string endDateTime, Func<CoetLogSearchResult, object> func)
        {
            DateTime sDate = Convert.ToDateTime(startDateTime);
            DateTime eDate = Convert.ToDateTime(endDateTime);

            List<AcrossDateEntity> acrossDateList = GetAcrossDate(sDate, eDate);

            foreach (var item in acrossDateList)
            {
                var reply = await client.GetLogAsync(new CoetLogSearchParm
                {
                    StartDateTime = item.StartDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    EndDateTime = item.EndDate.ToString("yyyy-MM-dd HH:mm:ss")
                });

                func(reply);
            }
        }

        private List<AcrossDateEntity> GetAcrossDate(DateTime sDate, DateTime eDate)
        {
            List<AcrossDateEntity> acrossDateList = new List<AcrossDateEntity>();
            DateTime acrossDate = sDate;

            if (eDate > sDate)
            {
                if (sDate.AddSeconds(1) > eDate)
                {
                    acrossDateList.Add(new AcrossDateEntity
                    {
                        StartDate = acrossDate,
                        EndDate = eDate
                    });
                }
                else
                {
                    while (true)
                    {
                        if (acrossDate < eDate)
                        {
                            AcrossDateEntity ad = new AcrossDateEntity();

                            ad.StartDate = acrossDate;
                            acrossDate = acrossDate.AddSeconds(1);

                            if (acrossDate > eDate)
                            {
                                ad.EndDate = eDate;
                            }
                            else
                            {
                                ad.EndDate = acrossDate;
                            }

                            acrossDateList.Add(ad);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            return acrossDateList;
        }
    }

    class AcrossDateEntity
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
