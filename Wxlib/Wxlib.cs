using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Configuration;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Loglib;

namespace Wxlib
{
    /// <summary>
    /// 微信access_token
    /// </summary>
    public class WxAt
    {
        public string access_token;
        public int expires_in;
        public WxAt()
        {
            getAccess_Token();
        }

        public void getAccess_Token()
        {
            string appid = ConfigurationManager.AppSettings["appid"];
            string appsecret = ConfigurationManager.AppSettings["appsecret"];
            string tokenUrl = @"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=" + appid + "&secret=" + appsecret;
            WebClient wc = new WebClient();
            string strReturn = wc.DownloadString(tokenUrl);
            JObject array;
            
            try
            {
                array = (JObject)JsonConvert.DeserializeObject(strReturn);
                access_token = array["access_token"].ToString().Trim('"');
                expires_in = int.Parse(array["expires_in"].ToString());
            }
            catch(Exception e) {
                MyLog.writeLog(strReturn, e);
                throw (e);
            }
        }
    }
    
}
