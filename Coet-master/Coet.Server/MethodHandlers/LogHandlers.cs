using Coet.Server.Infrastructure;
using Coet.Server.Persistent;
using Cote.GrpcProto;
using System;
using System.Collections.Generic;
using System.Text;
using Grpc.Core;
using System.Threading.Tasks;

namespace Coet.Server.MethodHandlers
{
    class LogHandlers : CoetLog.CoetLogBase
    {
        public override Task<SaveCoteLogResult> SaveLog(SaveCoteLogParm request, ServerCallContext context)
        {
            List<CoetLogInfoEntity> logInfoList = new List<CoetLogInfoEntity>();
            foreach (var item in request.CoetLogInfos)
            {
                logInfoList.Add(new CoetLogInfoEntity {
                    Type = item.Type,
                    JsonInfo = item.JsonInfo,
                    SendIP = item.SendIP,
                    SendName = item.SendName
                });
            }

            LogMethod lm = new LogMethod();
            int executeCount = lm.SaveLog(logInfoList);

            return Task.FromResult(new SaveCoteLogResult { ExecuteCount = executeCount });
        }
    }

    class LogMethod
    {
        public int SaveLog(List<CoetLogInfoEntity> logInfoList)
        {
            try
            {
                string tableName = LogTable.GetCurrentLogTName();

                StringBuilder sb = new StringBuilder();
                foreach (var item in logInfoList)
                {
                    sb.AppendFormat(@"insert into {0}(Type, JsonInfo, SendIP, SendName, Createdt) 
                                      value({1}, {2}, {3}, {4}, now());",
                                      tableName, item.Type, item.JsonInfo, item.SendIP, item.SendName);
                }

                return DBOperate.ExecuteNonQuery(sb.ToString(), new { });
            }
            catch (Exception ex)
            {
                CoetLocalLog.Error(ex.ToString());
                return 0;
            }
        }
    }
}
