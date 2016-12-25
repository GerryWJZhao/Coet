using Coet.Server.Infrastructure;
using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace Coet.Server.Persistent
{
    public class DBOperate
    {
        public static string connectStr = string.Empty;

        public static int ExecuteNonQuery(string sql, object param)
        {
            try
            {
                if (string.IsNullOrEmpty(connectStr))
                {
                    connectStr = GetConnectStr();
                }
                int result = 0;
                using (MySqlConnection con = new MySqlConnection(connectStr))
                {
                    result = con.Execute(sql, param);
                }
                return result;
            }
            catch (Exception ex)
            {
                CoetLocalLog.Error(ex.ToString());
                throw ex;
            }
        }

        public static List<T> ExecuteDataList<T>(string sql, object param)
        {
            try
            {
                if (string.IsNullOrEmpty(connectStr))
                {
                    connectStr = GetConnectStr();
                }
                List<T> list = null;
                using (MySqlConnection con = new MySqlConnection(connectStr))
                {
                    list = con.Query<T>(sql, param).AsList();
                }
                return list;
            }
            catch (Exception ex)
            {
                CoetLocalLog.Error(ex.ToString());
                throw ex;
            }
        }

        private static string GetConnectStr()
        {
            var conf = new ConfigurationBuilder()
                       .AddJsonFile("AppConfig.json")
                       .Build();

            return conf.GetSection("AppConfig:MySqlConnetString").Value;
        }
    }
}
