using System;
using log4net;

namespace Loglib
{
    public enum logtype { Info, Debug, Warn, Fatal, Error }
    public class MyLog
    {
        public static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 分类型记录日志
        /// </summary>
        /// <param name="Type">类型</param>
        /// <param name="text"></param>
        public static void writeLog(string text,Exception e=null, logtype Type = logtype.Error)
        {
            switch (Type)
            {
                case logtype.Info:
                    log.Info(text);
                    break;
                case logtype.Debug:
                    log.Debug(text);
                    break;
                case logtype.Warn:
                    log.Warn(text);
                    break;
                case logtype.Fatal:
                    log.Fatal(text, e);
                    break;
                default:
                    log.Error(text, e);
                    break;
            }
        }
    }
}
