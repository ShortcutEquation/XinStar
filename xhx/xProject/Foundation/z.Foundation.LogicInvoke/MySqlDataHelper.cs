using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace z.Foundation.LogicInvoke
{
    /// <summary>
    /// The MySqlHelper class is intended to encapsulate high performance, 
    /// scalable best practices for common uses of MySqlClient.
    /// </summary>
    public class MySqlDataHelper
    {
        /// <summary>
        /// 获取SqlConnection对象
        /// </summary>
        /// <param name="connName">连接串名称</param>
        /// <returns></returns>
        public static MySqlConnection GetSqlConnection(string connName)
        {
            MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings[connName].ConnectionString);
            conn.Open();
            return conn;
        }

        /// <summary>
        /// 执行SQL命令，无返回值
        /// </summary>
        /// <param name="connName">连接串名称</param>
        /// <param name="cmdText">SQL命令</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="commandParameters">参数集合</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(string connName, string cmdText, CommandType cmdType, params MySqlParameter[] commandParameters)
        {
            MySqlCommand cmd = new MySqlCommand();
            using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings[connName].ConnectionString))
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        /// 执行SQL命令，无返回值
        /// </summary>
        /// <param name="connection">MySqlConnection对象</param>
        /// <param name="cmdText">SQL命令</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="commandParameters">参数集合</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(MySqlConnection connection, string cmdText, CommandType cmdType, params MySqlParameter[] commandParameters)
        {
            MySqlCommand cmd = new MySqlCommand();
            PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// 执行SQL命令，无返回值
        /// </summary>
        /// <param name="trans">SqlTransaction对象</param>
        /// <param name="cmdText">SQL命令</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="commandParameters">参数集合</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(MySqlTransaction trans, string cmdText, CommandType cmdType, params MySqlParameter[] commandParameters)
        {
            MySqlCommand cmd = new MySqlCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, commandParameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// 执行SQL命令，返回结果集第一行第一列的值
        /// </summary>
        /// <param name="connName">连接串名称</param>
        /// <param name="cmdText">SQL命令</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="commandParameters">参数集合</param>
        /// <returns></returns>
        public static object ExecuteScalar(string connName, string cmdText, CommandType cmdType, params MySqlParameter[] commandParameters)
        {
            MySqlCommand cmd = new MySqlCommand();
            using (MySqlConnection connection = new MySqlConnection(ConfigurationManager.ConnectionStrings[connName].ConnectionString))
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }
        }

        /// <summary>
        /// 执行SQL命令，返回结果集第一行第一列的值
        /// </summary>
        /// <param name="connection">SqlConnection对象</param>
        /// <param name="cmdText">SQL命令</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="commandParameters">参数集合</param>
        /// <returns></returns>
        public static object ExecuteScalar(MySqlConnection connection, string cmdText, CommandType cmdType, params MySqlParameter[] commandParameters)
        {
            MySqlCommand cmd = new MySqlCommand();
            PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
            object val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// 执行SQL命令，返回结果集第一行第一列的值
        /// </summary>
        /// <param name="trans">SqlTransaction对象</param>
        /// <param name="cmdText">SQL命令</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="commandParameters">参数集合</param>
        /// <returns></returns>
        public static object ExecuteScalar(MySqlTransaction trans, string cmdText, CommandType cmdType, params MySqlParameter[] commandParameters)
        {
            MySqlCommand cmd = new MySqlCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, commandParameters);
            object val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// 执行SQL命令，返回DataTable对象
        /// </summary>
        /// <param name="connName">连接串名称</param>
        /// <param name="cmdText">SQL命令</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="commandParameters">参数集合</param>
        /// <returns></returns>
        public static DataTable ExecuteDataTable(string connName, string cmdText, CommandType cmdType, params MySqlParameter[] commandParameters)
        {
            DataTable dataTable = new DataTable();
            MySqlCommand cmd = new MySqlCommand();
            using (MySqlConnection connection = new MySqlConnection(ConfigurationManager.ConnectionStrings[connName].ConnectionString))
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                MySqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                dataTable = ConvertDataReaderToDataTable(rdr);
                rdr.Close();
            }
            dataTable.RemotingFormat = SerializationFormat.Binary;
            return dataTable;
        }
        
        /// <summary>
        /// Prepare a command for execution
        /// </summary>
        /// <param name="cmd">MySqlCommand object</param>
        /// <param name="conn">SqlConnection object</param>
        /// <param name="trans">SqlTransaction object</param>
        /// <param name="cmdType">Cmd type e.g. stored procedure or text</param>
        /// <param name="cmdText">Command text, e.g. Select * from Products</param>
        /// <param name="cmdParms">SqlParameters to use in the command</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:检查 SQL 查询是否存在安全漏洞")]
        private static void PrepareCommand(MySqlCommand cmd, MySqlConnection conn, MySqlTransaction trans, CommandType cmdType, string cmdText, MySqlParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();

            cmd.Connection = conn;
            cmd.CommandText = cmdText;

            if (trans != null)
                cmd.Transaction = trans;

            cmd.CommandType = cmdType;

            if (cmdParms != null)
            {
                foreach (MySqlParameter parm in cmdParms)
                {
                    if (parm == null)
                    {
                        throw new Exception("请设置参数值");
                    }
                    cmd.Parameters.Add(parm);
                }
            }
        }

        /// <summary>
        /// 将DataReader转化为DataTable
        /// </summary>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        private static DataTable ConvertDataReaderToDataTable(MySqlDataReader dataReader)
        {
            DataTable objDataTable = new DataTable();
            int intFieldCount = dataReader.FieldCount;
            for (int intCounter = 0; intCounter < intFieldCount; intCounter++)
            {
                objDataTable.Columns.Add(dataReader.GetName(intCounter), dataReader.GetFieldType(intCounter));
            }
            objDataTable.BeginLoadData();
            object[] objValues = new object[intFieldCount];
            while (dataReader.Read())
            {
                dataReader.GetValues(objValues);
                objDataTable.LoadDataRow(objValues, true);
            }
            objDataTable.EndLoadData();
            return objDataTable;
        }
    }
}
