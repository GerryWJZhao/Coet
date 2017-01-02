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

                List<CoetLogInfoEntity> logInfoList = am.GetLog(request.StartDateTime, request.EndDateTime, request.LogType);

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
        public List<CoetLogInfoEntity> GetLog(string startDateTime, string endDateTime, string logType)
        {
            try
            {
                string execSql = string.Empty;
                string typeWhereTemplate = string.Empty;

                string sqlTemplate = @"select Type, JsonInfo, SendIP, SendName, Createdt 
                                       from {0} where Createdt between @startDateTime and @endDateTime";

                if (logType == "ALL")
                {
                    typeWhereTemplate = "and 1 = 1";
                }
                else
                {
                    typeWhereTemplate = string.Format("and Type = '{0}'", logType);
                }

                sqlTemplate = string.Format("{0} {1}", sqlTemplate, typeWhereTemplate);

                DateTime sDate = Convert.ToDateTime(startDateTime);
                DateTime eDate = Convert.ToDateTime(endDateTime);

                if (sDate.Year != eDate.Year || sDate.Month != eDate.Month)
                {
                    string stableName = LogTable.GetLogTName(sDate);
                    string etableName = LogTable.GetLogTName(eDate);

                    execSql = string.Format("{0} union {1}",
                        string.Format(sqlTemplate, stableName),
                        string.Format(sqlTemplate, etableName));
                }
                else
                {
                    string tableName = LogTable.GetLogTName(sDate);
                    execSql = string.Format(sqlTemplate, tableName);
                }

                return DBOperate.ExecuteDataList<CoetLogInfoEntity>(execSql, new { startDateTime = startDateTime, endDateTime = endDateTime });
            }
            catch (Exception ex)
            {
                CoetLocalLog.Error(ex.ToString());
                return new List<CoetLogInfoEntity>();
            }
        }
    }
}
