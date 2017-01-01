using Coet.GrpcProto;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Coet.AnalyseSDK
{
    public enum CoetAnalyseAddPart
    {
        AddDays,
        AddHours,
        AddMinutes,
        AddSeconds
    }

    public class CoetAnalyseSDK
    {
        CoetAnalyse.CoetAnalyseClient client;

        public CoetAnalyseSDK(string coetServerUrl)
        {
            Channel channel = new Channel(coetServerUrl, ChannelCredentials.Insecure);
            client = new CoetAnalyse.CoetAnalyseClient(channel);
        }

        public async void GetLogAsync(string startDateTime, string endDateTime, CoetAnalyseAddPart part, Func<CoetLogSearchResult, object> func, int addNum = 1)
        {
            DateTime sDate = Convert.ToDateTime(startDateTime);
            DateTime eDate = Convert.ToDateTime(endDateTime);

            List<AcrossDateEntity> acrossDateList = GetAcrossDate(sDate, eDate, part, addNum);

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

        private List<AcrossDateEntity> GetAcrossDate(DateTime sDate, DateTime eDate, CoetAnalyseAddPart part, int addNum)
        {
            List<AcrossDateEntity> acrossDateList = new List<AcrossDateEntity>();
            DateTime acrossDate = sDate;

            if (eDate > sDate)
            {
                if (sDate.AddDate(addNum, part) > eDate)
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
                            acrossDate = acrossDate.AddDate(addNum, part);

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

    static class CoetDateTimeExtension
    {
        internal static DateTime AddDate(this DateTime date, int value, CoetAnalyseAddPart part)
        {
            TypeInfo ti = typeof(DateTime).GetTypeInfo();
            MethodInfo mi = ti.GetMethod(part.ToString());

            var newDate = mi.Invoke(date, new object[1] { value });

            return Convert.ToDateTime(newDate);
        }
    }

    class AcrossDateEntity
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
