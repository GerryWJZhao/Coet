using System;
using System.Collections.Generic;
using System.Text;

namespace Coet.Server.Persistent
{
    public class LogTable
    {
        const string logTDefine = @"CREATE TABLE `Log_{0}` (
                              `Id` int(64) NOT NULL AUTO_INCREMENT,
                              `Type` varchar(20) DEFAULT NULL,
                              `JsonInfo` longtext,
                              `SendIP` varchar(64) DEFAULT NULL,
                              `Createdt` datetime DEFAULT NULL,
                              PRIMARY KEY (`Id`),
                              KEY `index_Tpye` (`Type`),
                              KEY `index_Createdt` (`Createdt`),
                              KEY `index_SendIP` (`SendIP`)
                            ) ENGINE=InnoDB AUTO_INCREMENT=1 DEFAULT CHARSET=utf8;";

        public static string CurrentLogTDefine
        {
            get
            {
                return string.Format(logTDefine, DateTime.Now.ToString("yyyyMM"));
            }
        }
    }
}
