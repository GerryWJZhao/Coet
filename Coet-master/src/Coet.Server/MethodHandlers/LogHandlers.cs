using Coet.Server.Infrastructure;
using Coet.Server.Persistent;
using System;
using System.Collections.Generic;
using System.Text;
using Grpc.Core;
using System.Threading.Tasks;
using Coet.GrpcProto;

namespace Coet.Server.MethodHandlers
{
    class LogHandlers : CoetLog.CoetLogBase
    {
        public override Task<SaveCoetLogResult> SaveLog(SaveCoetLogParm request, ServerCallContext context)
        {
            if (Common.isAllowHost(context.Peer))
            {
                List<CoetLogInfoEntity> logInfoList = new List<CoetLogInfoEntity>();
                foreach (var item in request.CoetLogInfos)
                {
                    logInfoList.Add(new CoetLogInfoEntity
                    {
                        Type = item.Type,
                        JsonInfo = item.JsonInfo,
                        SendIP = item.SendIP,
                        SendName = item.SendName
                    });
                }

                LogMethod lm = new LogMethod();
                int executeCount = lm.SaveLog(logInfoList);

                return Task.FromResult(new SaveCoetLogResult { ExecuteCount = executeCount });
            }
            else
            {
                return Task.FromResult(new SaveCoetLogResult { ExecuteCount = 0 });
            }
        }
    }

    class LogMethod
    {
        public int SaveLog(List<CoetLogInfoEntity> logInfoList)
        {
            try
            {
                string tableName = LogTable.GetCurrentLogTName();

                int executeCount = 0;
                int levelCount = 200;
                int partCount = 1;

                if (logInfoList.Count > levelCount)
                {
                    partCount = logInfoList.Count / levelCount;
                }
                List<List<CoetLogInfoEntity>> partList = CoetArry.splitList(logInfoList, partCount);

                foreach (var part in partList)
                {
                    StringBuilder sb = new StringBuilder();

                    foreach (var item in part)
                    {
                        sb.AppendFormat(@"insert into {0}(Type, JsonInfo, SendIP, SendName, IdentificationId, Createdt) value('{1}', '{2}', '{3}', '{4}', '{5}', now());",
                                          tableName, item.Type, item.JsonInfo, item.SendIP, item.SendName, Guid.NewGuid().ToString());
                    }

                    executeCount += DBOperate.ExecuteNonQuery(sb.ToString(), new { });
                }

                return executeCount;
            }
            catch (Exception ex)
            {
                CoetLocalLog.Error(ex.ToString());
                return 0;
            }
        }
    }
}
