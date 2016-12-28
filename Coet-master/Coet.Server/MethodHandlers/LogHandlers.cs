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
        public override Task<SaveCoteLogResult> SaveLog(CoetLogInfo request, ServerCallContext context)
        {
            LogMethod lm = new LogMethod();
            int executeCount = lm.SaveLog(request.Type, request.JsonInfo, request.SendIP, request.SendName);
            return Task.FromResult(new SaveCoteLogResult { ExecuteCount = executeCount });
        }
    }

    class LogMethod
    {
        public int SaveLog(string type, string jsonInfo, string sendIP, string sendName)
        {
            try
            {
                string tableName = LogTable.GetCurrentLogTName();

                string sql = string.Format(@"insert into {0}(Type, JsonInfo, SendIP, SendName, Createdt) 
                                             value(@Type, @JsonInfo, @SendIP, @SendName, now());", tableName);

                return DBOperate.ExecuteNonQuery(sql,
                       new
                       {
                           Type = type,
                           JsonInfo = jsonInfo,
                           SendIP = sendIP,
                           SendName = sendName
                       });
            }
            catch (Exception ex)
            {
                CoetLocalLog.Error(ex.ToString());
                return 0;
            }
        }
    }
}
