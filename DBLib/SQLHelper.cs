using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Loglib;

namespace DBLib
{
    public class SQLHelper
    {
        //连接字符串
         string strConn  ;

        public SQLHelper(string connstring=null)
        {
            if(connstring == null)
            {
                strConn = ConfigurationManager.ConnectionStrings["connstring"].ConnectionString;
            }
            else
            {
                strConn = connstring;
            }
        }

        #region 执行查询，返回DataTable对象-----------------------


        public  DataTable GetTable(string strSQL)
        {
            return GetTable(strSQL, null);
        }
        public  DataTable GetTable(string strSQL, SqlParameter[] pas)
        {
            return GetTable(strSQL, pas, CommandType.Text);
        }
        /// <summary>
        /// 执行查询，返回DataTable对象
        /// </summary>
        /// <param name="strSQL">sql语句</param>
        /// <param name="pas">参数数组</param>
        /// <param name="cmdtype">Command类型</param>
        /// <returns>DataTable对象</returns>
        public  DataTable GetTable(string strSQL, SqlParameter[] pas, CommandType cmdtype)
        {
            DataTable dt = new DataTable(); ;
            using (SqlConnection conn = new SqlConnection(strConn))
            {
                try { 
                SqlDataAdapter da = new SqlDataAdapter(strSQL, conn);
                da.SelectCommand.CommandType = cmdtype;
                if (pas != null)
                {
                    da.SelectCommand.Parameters.AddRange(pas);
                }
                da.Fill(dt);
                }
                catch(Exception e)
                {
                    MyLog.writeLog("ERROR", logtype.Error, e);
                    return null;
                }
            }
            return dt;
        }



        #endregion




        #region 执行查询，返回DataSet对象-------------------------




        public  DataSet GetDataSet(string strSQL)
        {
            return GetDataSet(strSQL, null);
        }

        public  DataSet GetDataSet(string strSQL, SqlParameter[] pas)
        {
            return GetDataSet(strSQL, pas, CommandType.Text);
        }
        /// <summary>
        /// 执行查询，返回DataSet对象
        /// </summary>
        /// <param name="strSQL">sql语句</param>
        /// <param name="pas">参数数组</param>
        /// <param name="cmdtype">Command类型</param>
        /// <returns>DataSet对象</returns>
        public  DataSet GetDataSet(string strSQL, SqlParameter[] pas, CommandType cmdtype)
        {
            DataSet dt = new DataSet();
            try
            {
                using (SqlConnection conn = new SqlConnection(strConn))
                {
                    SqlDataAdapter da = new SqlDataAdapter(strSQL, conn);
                    da.SelectCommand.CommandType = cmdtype;
                    da.SelectCommand.CommandTimeout = 1800;
                    if (pas != null)
                    {
                        da.SelectCommand.Parameters.AddRange(pas);
                    }
                    da.Fill(dt);
                }
            }
            catch(Exception e)
            {
                MyLog.writeLog("ERROR", logtype.Error, e);
                return null;
            }
           
            return dt;
        }
        #endregion





        #region 执行非查询存储过程和SQL语句-----------------------------




        public  int ExcuteProc(string ProcName)
        {
            return ExcuteSQL(ProcName, null, CommandType.StoredProcedure);
        }

        public  int ExcuteProc(string ProcName, SqlParameter[] pars)
        {
            return ExcuteSQL(ProcName, pars, CommandType.StoredProcedure);
        }

        public  int ExcuteSQL(string strSQL)
        {
            return ExcuteSQL(strSQL, null);
        }

        public  int ExcuteSQL(string strSQL, SqlParameter[] paras)
        {
            return ExcuteSQL(strSQL, paras, CommandType.Text);
        }

        /// 执行非查询存储过程和SQL语句
        /// 增、删、改
        /// </summary>
        /// <param name="strSQL">要执行的SQL语句</param>
        /// <param name="paras">参数列表，没有参数填入null</param>
        /// <param name="cmdType">Command类型</param>
        /// <returns>返回影响行数</returns>
        public  int ExcuteSQL(string strSQL, SqlParameter[] paras, CommandType cmdType)
        {
            int i = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(strConn))
                {
                    SqlCommand cmd = new SqlCommand(strSQL, conn);
                    cmd.CommandType = cmdType;
                    cmd.CommandTimeout = 1800;
                    if (paras != null)
                    {
                        cmd.Parameters.AddRange(paras);
                    }
                    conn.Open();
                    i = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch(Exception e)
            {
                MyLog.writeLog("ERROR", logtype.Error, e);
                return i;
            }
            
            return i;

        }


        #endregion








        #region 执行查询返回第一行，第一列---------------------------------




        public  int ExcuteScalarSQL(string strSQL)
        {
            return ExcuteScalarSQL(strSQL, null);
        }

        public  int ExcuteScalarSQL(string strSQL, SqlParameter[] paras)
        {
            return ExcuteScalarSQL(strSQL, paras, CommandType.Text);
        }
        public  int ExcuteScalarProc(string strSQL, SqlParameter[] paras)
        {
            return ExcuteScalarSQL(strSQL, paras, CommandType.StoredProcedure);
        }
        /// <summary>
        /// 执行SQL语句，返回第一行，第一列
        /// </summary>
        /// <param name="strSQL">要执行的SQL语句</param>
        /// <param name="paras">参数列表，没有参数填入null</param>
        /// <returns>返回影响行数</returns>
        public  int ExcuteScalarSQL(string strSQL, SqlParameter[] paras, CommandType cmdType)
        {
            int i = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(strConn))
                {
                    SqlCommand cmd = new SqlCommand(strSQL, conn);
                    cmd.CommandType = cmdType;
                    if (paras != null)
                    {
                        cmd.Parameters.AddRange(paras);
                    }
                    conn.Open();
                    i = Convert.ToInt32(cmd.ExecuteScalar());
                    conn.Close();
                }
            }
            catch(Exception e)
            {
                MyLog.writeLog("ERROR", logtype.Error, e);
                return i;
            }     
            return i;

        }


        #endregion









        #region 查询获取单个值------------------------------------




        /// <summary>
        /// 调用不带参数的存储过程获取单个值
        /// </summary>
        /// <param name="ProcName"></param>
        /// <returns></returns>
        public  object GetObjectByProc(string ProcName)
        {
            return GetObjectByProc(ProcName, null);
        }
        /// <summary>
        /// 调用带参数的存储过程获取单个值
        /// </summary>
        /// <param name="ProcName"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        public  object GetObjectByProc(string ProcName, SqlParameter[] paras)
        {
            return GetObject(ProcName, paras, CommandType.StoredProcedure);
        }
        /// <summary>
        /// 根据sql语句获取单个值
        /// </summary>
        /// <param name="strSQL"></param>
        /// <returns></returns>
        public  object GetObject(string strSQL)
        {
            return GetObject(strSQL, null);
        }
        /// <summary>
        /// 根据sql语句 和 参数数组获取单个值
        /// </summary>
        /// <param name="strSQL"></param>
        /// <param name="paras"></param>
        /// <returns></returns>
        public  object GetObject(string strSQL, SqlParameter[] paras)
        {
            return GetObject(strSQL, paras, CommandType.Text);
        }

        /// <summary>
        /// 执行SQL语句，返回首行首列
        /// </summary>
        /// <param name="strSQL">要执行的SQL语句</param>
        /// <param name="paras">参数列表，没有参数填入null</param>
        /// <returns>返回的首行首列</returns>
        public  object GetObject(string strSQL, SqlParameter[] paras, CommandType cmdtype)
        {
            object o = null;
            try
            {
                using (SqlConnection conn = new SqlConnection(strConn))
                {
                    SqlCommand cmd = new SqlCommand(strSQL, conn);
                    cmd.CommandType = cmdtype;
                    if (paras != null)
                    {
                        cmd.Parameters.AddRange(paras);

                    }

                    conn.Open();
                    o = cmd.ExecuteScalar();
                    conn.Close();
                }
            }
            catch(Exception e)
            {
                MyLog.writeLog("ERROR", logtype.Error, e);
                return o;
            }
            return o;

        }



        #endregion





        #region 查询获取DataReader------------------------------------




        /// <summary>
        /// 调用不带参数的存储过程，返回DataReader对象
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <returns>DataReader对象</returns>
        public  SqlDataReader GetReaderByProc(string procName)
        {
            return GetReaderByProc(procName, null);
        }
        /// <summary>
        /// 调用带有参数的存储过程，返回DataReader对象
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="paras">参数数组</param>
        /// <returns>DataReader对象</returns>
        public  SqlDataReader GetReaderByProc(string procName, SqlParameter[] paras)
        {
            return GetReader(procName, paras, CommandType.StoredProcedure);
        }
        /// <summary>
        /// 根据sql语句返回DataReader对象
        /// </summary>
        /// <param name="strSQL">sql语句</param>
        /// <returns>DataReader对象</returns>
        public  SqlDataReader GetReader(string strSQL)
        {
            return GetReader(strSQL, null);
        }
        /// <summary>
        /// 根据sql语句和参数返回DataReader对象
        /// </summary>
        /// <param name="strSQL">sql语句</param>
        /// <param name="paras">参数数组</param>
        /// <returns>DataReader对象</returns>
        public  SqlDataReader GetReader(string strSQL, SqlParameter[] paras)
        {
            return GetReader(strSQL, paras, CommandType.Text);
        }
        /// <summary>
        /// 查询SQL语句获取DataReader
        /// </summary>
        /// <param name="strSQL">查询的SQL语句</param>
        /// <param name="paras">参数列表，没有参数填入null</param>
        /// <returns>查询到的DataReader（关闭该对象的时候，自动关闭连接）</returns>
        public  SqlDataReader GetReader(string strSQL, SqlParameter[] paras, CommandType cmdtype)
        {
            SqlDataReader sqldr = null;
            try
            {
               
                SqlConnection conn = new SqlConnection(strConn);
                SqlCommand cmd = new SqlCommand(strSQL, conn);
                cmd.CommandType = cmdtype;
                if (paras != null)
                {
                    cmd.Parameters.AddRange(paras);
                }
                conn.Open();
                //CommandBehavior.CloseConnection的作用是如果关联的DataReader对象关闭，则连接自动关闭
                sqldr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch(Exception e)
            {
                MyLog.writeLog("ERROR", logtype.Error, e);
                return sqldr;
            }
            
            return sqldr;
        }



        #endregion




        #region 批量插入数据---------------------------------------------




        /// <summary>
        /// 往数据库中批量插入数据
        /// </summary>
        /// <param name="sourceDt">数据源表</param>
        /// <param name="targetTable">服务器上目标表</param>
        public  void BulkToDB(DataTable sourceDt, string targetTable)
        {
            SqlConnection conn = new SqlConnection(strConn);
            SqlBulkCopy bulkCopy = new SqlBulkCopy(conn);   //用其它源的数据有效批量加载sql server表中
            bulkCopy.DestinationTableName = targetTable;    //服务器上目标表的名称
            bulkCopy.BatchSize = sourceDt.Rows.Count;   //每一批次中的行数
            bulkCopy.BulkCopyTimeout = 3600000;
            try
            {
                conn.Open();
                if (sourceDt != null && sourceDt.Rows.Count != 0)
                    bulkCopy.WriteToServer(sourceDt);   //将提供的数据源中的所有行复制到目标表中
            }
            catch (Exception ex)
            {
                MyLog.writeLog("ERROR", logtype.Error, ex);
                throw ex;
            }
            finally
            {
                conn.Close();
                if (bulkCopy != null)
                    bulkCopy.Close();
            }

        }

        /// <summary>
        /// 往数据库中批量插入数据
        /// </summary>
        /// <param name="sourceDt">数据源表</param>
        /// <param name="targetTable">服务器上目标表</param>
        public  void BulkToDB(DataTable sourceDt, string targetTable, DataTable dtMap)
        {
            SqlConnection conn = new SqlConnection(strConn);
            SqlBulkCopy bulkCopy = new SqlBulkCopy(conn);   //用其它源的数据有效批量加载sql server表中
            bulkCopy.DestinationTableName = targetTable;    //服务器上目标表的名称
            bulkCopy.BatchSize = sourceDt.Rows.Count;   //每一批次中的行数
            bulkCopy.BulkCopyTimeout = 3600000;
            int iCount = dtMap.Rows.Count;
            for (int i = 0; i < iCount; i++)
            {
                bulkCopy.ColumnMappings.Add(dtMap.Rows[i]["SourceField"].ToString(), dtMap.Rows[i]["TargetField"].ToString());
            }

            try
            {
                conn.Open();
                if (sourceDt != null && sourceDt.Rows.Count != 0)
                    bulkCopy.WriteToServer(sourceDt);   //将提供的数据源中的所有行复制到目标表中
            }
            catch (Exception ex)
            {
                MyLog.writeLog("ERROR", logtype.Error, ex);
                throw ex;
            }
            finally
            {
                conn.Close();
                if (bulkCopy != null)
                    bulkCopy.Close();
            }

        }

        #endregion


    }
}