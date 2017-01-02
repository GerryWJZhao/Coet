using System;
using System.Collections.Generic;
using System.Text;

namespace Coet.Server.Persistent
{
    public class LogTable
    {
        const string logTDefine = @"CREATE TABLE `{0}` (
                              `Id` int(64) NOT NULL AUTO_INCREMENT,
                              `Type` varchar(20) DEFAULT NULL,
                              `JsonInfo` json,
                              `SendIP` varchar(64) DEFAULT NULL,
                              `SendName` varchar(64) DEFAULT NULL,
                              `IdentificationId` varchar(64) DEFAULT NULL,
                              `Createdt` datetime DEFAULT NULL,
                              PRIMARY KEY (`Id`),
                              KEY `index_Tpye` (`Type`),
                              KEY `index_Createdt` (`Createdt`),
                              KEY `index_SendIP` (`SendIP`)
                            ) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8;";

        private static string CurrentLogTName
        {
            get
            {
                return string.Format("Log_{0}", DateTime.Now.ToString("yyyyMM"));
            }
        }

        public static string GetCurrentLogTName()
        {
            string tableName = CurrentLogTName;

            string sql = string.Format(@"select TABLE_NAME from INFORMATION_SCHEMA.TABLES where 
                                         TABLE_SCHEMA = 'CoetServer' and TABLE_NAME = '{0}';", tableName);

            List<string> rlist = DBOperate.ExecuteDataList<string>(sql, new object());

            if (rlist.Count <= 0)
            {
                DBOperate.ExecuteNonQuery(string.Format(logTDefine, tableName), new object());
            }

            return tableName;
        }

        public static string GetLogTName(DateTime dt)
        {
            return string.Format("Log_{0}", dt.ToString("yyyyMM"));
        }
    }
}
