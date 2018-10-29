using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDeviceConsole
{
    class DBUtils
    {
        private static readonly DBUtils instance = new DBUtils();
        private readonly string dbServerAddress = "rm-uf6fss8nalfgr254efo.mysql.rds.aliyuncs.com";
        private readonly string dbPort = "3306";
        private string connectStr = "";
        private DBUtils()
        {

        }

        public static DBUtils Instance
        {
            get
            {
                return instance;
            }
        }

        public void setDatabase(string user, string password, string databaseName)
        {
            connectStr += "server=" + dbServerAddress + ";"
                            + "port=" + dbPort + ";"
                            + "user=" + user + ";"
                            + "password=" + password + ";"
                            + "database=" + databaseName + ";"
                            + "SslMode = none;";
        }

        public List<Dictionary<string, object> > QueryDB(string tableName, List<string> cols = null)
        {
            MySqlConnection conn = new MySqlConnection(connectStr);

            List<Dictionary<string, object>> ret = new List<Dictionary<string, object>>();
            try
            {
                conn.Open();
                Console.WriteLine("连接已建立");

                string sql = "select ";
                if(cols == null || cols.Count == 0)
                {
                    sql += "* ";
                }
                else
                {
                    for(int i = 0; i < cols.Count; i++)
                    {
                        sql += cols[i];
                        if(i + 1 != cols.Count)
                        {
                            sql += ", ";
                        }
                    }
                }

                sql += (" from " + tableName);

                Console.WriteLine(sql);

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Dictionary<string, object> item = new Dictionary<string, object>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {                    
                        if (!reader.IsDBNull(i))
                        {                     
                            item.Add(reader.GetName(i), reader.GetValue(i));
                        }
                    }

                    ret.Add(item);
                }

            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
        
            }

            return ret;
        }

    }

    class DBConstant
    {
    
        
    }
}
