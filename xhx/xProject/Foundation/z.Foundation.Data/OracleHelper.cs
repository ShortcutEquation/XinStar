using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace z.Foundation.Data
{
    public class OracleHelper
    {
        /// <summary>
        /// 获取OracleConnection对象
        /// </summary>
        /// <param name="connName">连接串名称</param>
        /// <returns></returns>
        public static OracleConnection GetSqlConnection(string connName)
        {
            OracleConnection conn = new OracleConnection(ConfigurationManager.ConnectionStrings[connName].ConnectionString);
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
        public static int ExecuteNonQuery(string connName, string cmdText, CommandType cmdType, params OracleParameter[] commandParameters)
        {
            OracleCommand cmd = new OracleCommand(); 
            using (OracleConnection conn = new OracleConnection(ConfigurationManager.ConnectionStrings[connName].ConnectionString))
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
        /// <param name="connection">OracleConnection对象</param>
        /// <param name="cmdText">SQL命令</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="commandParameters">参数集合</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(OracleConnection connection, string cmdText, CommandType cmdType, params OracleParameter[] commandParameters)
        {
            OracleCommand cmd = new OracleCommand();
            PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// 执行SQL命令，无返回值
        /// </summary>
        /// <param name="trans">OracleTransaction对象</param>
        /// <param name="cmdText">SQL命令</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="commandParameters">参数集合</param>
        /// <returns></returns>
        public static int ExecuteNonQuery(OracleTransaction trans, string cmdText, CommandType cmdType, params OracleParameter[] commandParameters)
        {
            OracleCommand cmd = new OracleCommand();
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
        public static object ExecuteScalar(string connName, string cmdText, CommandType cmdType, params OracleParameter[] commandParameters)
        {
            OracleCommand cmd = new OracleCommand();
            using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings[connName].ConnectionString))
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
        /// <param name="connection">OracleConnection对象</param>
        /// <param name="cmdText">SQL命令</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="commandParameters">参数集合</param>
        /// <returns></returns>
        public static object ExecuteScalar(OracleConnection connection, string cmdText, CommandType cmdType, params OracleParameter[] commandParameters)
        {
            OracleCommand cmd = new OracleCommand();
            PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
            object val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// 执行SQL命令，返回结果集第一行第一列的值
        /// </summary>
        /// <param name="trans">OracleTransaction对象</param>
        /// <param name="cmdText">SQL命令</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="commandParameters">参数集合</param>
        /// <returns></returns>
        public static object ExecuteScalar(OracleTransaction trans, string cmdText, CommandType cmdType, params OracleParameter[] commandParameters)
        {
            OracleCommand cmd = new OracleCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, commandParameters);
            object val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return val;
        }

        /// <summary>
        /// 执行SQL命令，返回OracleDataReader对象
        /// </summary>
        /// <param name="connName">连接串名称</param>
        /// <param name="cmdText">SQL命令</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="commandParameters">参数集合</param>
        /// <returns></returns>
        public static OracleDataReader ExecuteDataReader(string connName, string cmdText, CommandType cmdType, params OracleParameter[] commandParameters)
        {
            OracleCommand cmd = new OracleCommand();
            OracleConnection conn = new OracleConnection(ConfigurationManager.ConnectionStrings[connName].ConnectionString);
            // we use a try/catch here because if the method throws an exception we want to 
            // close the connection throw code, because no datareader will exist, hence the 
            // commandBehaviour.CloseConnection will not work
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                OracleDataReader oracleDataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return oracleDataReader;
            }
            catch
            {
                conn.Close();
                throw;
            }
        }

        /// <summary>
        /// 执行SQL命令，返回DataTable对象
        /// </summary>
        /// <param name="connName">连接串名称</param>
        /// <param name="cmdText">SQL命令</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="commandParameters">参数集合</param>
        /// <returns></returns>
        public static DataTable ExecuteDataTable(string connName, string cmdText, CommandType cmdType, params OracleParameter[] commandParameters)
        {
            DataTable dataTable = new DataTable();
            OracleCommand cmd = new OracleCommand();
            using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings[connName].ConnectionString))
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                OracleDataAdapter oracleDataAdapter = new OracleDataAdapter(cmd);
                DataSet dataSet = new DataSet();
                oracleDataAdapter.Fill(dataSet);
                cmd.Parameters.Clear();
                if (dataSet.Tables.Count > 0)
                {
                    dataTable = dataSet.Tables[0];
                }
            }
            return dataTable;
        }

        /// <summary>
        /// 执行SQL命令，返回DataSet对象
        /// </summary>
        /// <param name="connName">连接串名称</param>
        /// <param name="cmdText">SQL命令</param>
        /// <param name="cmdType">命令类型</param>
        /// <param name="commandParameters">参数集合</param>
        /// <returns></returns>
        public static DataSet ExecuteDataSet(string connName, string cmdText, CommandType cmdType, params OracleParameter[] commandParameters)
        {
            DataSet dataSet = new DataSet();
            OracleCommand cmd = new OracleCommand();
            using (OracleConnection connection = new OracleConnection(ConfigurationManager.ConnectionStrings[connName].ConnectionString))
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                OracleDataAdapter oracleDataAdapter = new OracleDataAdapter(cmd);
                oracleDataAdapter.Fill(dataSet);
                cmd.Parameters.Clear();
            }
            return dataSet;
        }

        /// <summary>
        /// Prepare a command for execution
        /// </summary>
        /// <param name="cmd">OracleCommand object</param>
        /// <param name="conn">OracleConnection object</param>
        /// <param name="trans">OracleTransaction object</param>
        /// <param name="cmdType">Cmd type e.g. stored procedure or text</param>
        /// <param name="cmdText">Command text, e.g. Select * from Products</param>
        /// <param name="cmdParms">SqlParameters to use in the command</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:检查 SQL 查询是否存在安全漏洞")]
        private static void PrepareCommand(OracleCommand cmd, OracleConnection conn, OracleTransaction trans, CommandType cmdType, string cmdText, OracleParameter[] cmdParms)
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
                foreach (OracleParameter parm in cmdParms)
                {
                    if (parm == null)
                    {
                        throw new Exception("请设置参数值");
                    }
                    cmd.Parameters.Add(parm);
                }
            }
        }
    }
}
