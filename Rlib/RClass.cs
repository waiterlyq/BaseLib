using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using RDotNet;
using Pylib;
using Loglib;


namespace Rlib
{
    /// <summary>
    /// C#调用R环境基础对象
    /// </summary>
    public class RClass : IDisposable
    {
        public static REngine engine;

        public RClass()
        {
            REngine.SetEnvironmentVariables();
            engine = REngine.GetInstance();
            engine.Initialize();
        }

        /// <summary>
        /// 根据R脚本获取文本结果
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public string getStringByR(string rs)
        {
            string strResult = "";
            try
            {
                strResult = engine.Evaluate(rs).AsCharacter()[0];
            }
            catch (Exception e)
            {
                MyLog.writeLog("ERROR", logtype.Error, e);
                return "ERROR";
            }
            return strResult;
        }

        public int getIntByR(string rs)
        {
            int iResult = -1;
            try
            {
                iResult = engine.Evaluate(rs).AsInteger()[0];
            }
            catch (Exception e)
            {
                MyLog.writeLog("ERROR", logtype.Error, e);
                return -1;
            }
            return iResult;
        }

        /// <summary>
        /// 执行r脚本不返回结果
        /// </summary>
        /// <param name="rs"></param>
        /// <returns></returns>
        public bool EvaluateByR(string rs)
        {
            try
            {
                engine.Evaluate(rs);
                return true;
            }
            catch (Exception e)
            {
                MyLog.writeLog("ERROR", logtype.Error, e);
                return false;
            }
        }



        public void Dispose()
        {
            engine.ClearGlobalEnvironment();
        }
    }
}
