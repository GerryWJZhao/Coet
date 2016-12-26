using Coet.Server.Infrastructure;
using Coet.Server.Persistent;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coet.Server.MethodHandlers
{
    class LogHandlers
    {
        public void SaveLog(string type, string jsonInfo, string sendIP, string sendName)
        {
            try
            {
                string tableName = LogTable.GetCurrentLogTName();

                string sql = string.Format(@"insert into {0}(Type, JsonInfo, SendIP, SendName, Createdt) 
                                             value(@Type, @JsonInfo, @SendIP, @SendName, now());", tableName);

                DBOperate.ExecuteNonQuery(sql,
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
            }
        }
    }
}
