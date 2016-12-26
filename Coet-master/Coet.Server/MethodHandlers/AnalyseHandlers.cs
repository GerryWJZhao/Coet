using Coet.Server.Infrastructure;
using Coet.Server.Persistent;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coet.Server.MethodHandlers
{
    class AnalyseHandlers
    {
        public List<CoetLogInfo> GetLog(string startDateTime, string endDateTime)
        {
            try
            {
                string tableName = LogTable.GetCurrentLogTName();

                string sql = string.Format(@"select Type, JsonInfo, SendIP, SendName, Createdt 
                                             from {0} where Createdt between @startDateTime and @endDateTime", tableName);

                return DBOperate.ExecuteDataList<CoetLogInfo>(sql, new { startDateTime = startDateTime, endDateTime = endDateTime });
            }
            catch (Exception ex)
            {
                CoetLocalLog.Error(ex.ToString());
                return new List<CoetLogInfo>();
            }
        }
    }
}
