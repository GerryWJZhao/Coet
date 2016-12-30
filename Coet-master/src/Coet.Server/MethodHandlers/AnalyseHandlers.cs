using Coet.Server.Infrastructure;
using Coet.Server.Persistent;
using Coet.GrpcProto;
using System;
using System.Collections.Generic;
using System.Text;
using Grpc.Core;
using System.Threading.Tasks;

namespace Coet.Server.MethodHandlers
{
    class AnalyseHandlers: CoetAnalyse.CoetAnalyseBase
    {
        public override Task<CoetLogSearchResult> GetLog(CoetLogSearchParm request, ServerCallContext context)
        {
            if (Common.isAllowHost(context.Host))
            {
                AnalyseMethod am = new AnalyseMethod();

                List<CoetLogInfoEntity> logInfoList = am.GetLog(request.StartDateTime, request.EndDateTime);

                CoetLogSearchResult csr = new CoetLogSearchResult();
                foreach (var item in logInfoList)
                {
                    csr.CoetLogInfos.Add(new CoetLogInfo
                    {
                        Type = item.Type,
                        JsonInfo = item.JsonInfo,
                        SendIP = item.SendIP,
                        SendName = item.SendName
                    });
                }

                return Task.FromResult(csr);
            }
            else
            {
                return Task.FromResult(new CoetLogSearchResult());
            }
        }
    }

    class AnalyseMethod
    {
        public List<CoetLogInfoEntity> GetLog(string startDateTime, string endDateTime)
        {
            try
            {
                string tableName = LogTable.GetCurrentLogTName();

                string sql = string.Format(@"select Type, JsonInfo, SendIP, SendName, Createdt 
                                             from {0} where Createdt between @startDateTime and @endDateTime", tableName);

                return DBOperate.ExecuteDataList<CoetLogInfoEntity>(sql, new { startDateTime = startDateTime, endDateTime = endDateTime });
            }
            catch (Exception ex)
            {
                CoetLocalLog.Error(ex.ToString());
                return new List<CoetLogInfoEntity>();
            }
        }
    }
}
