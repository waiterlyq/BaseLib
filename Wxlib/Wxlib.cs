using System;
using System.Net;
using System.Configuration;
using Newtonsoft.Json;
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
    }


    /// <summary>
    /// 微信openid和access_token
    /// </summary>
    public class WxOpenIdAt
    {
        public string access_token;
        public int expires_in;
        public string refresh_token;
        public string openid;
        public string scope;
    }

    /// <summary>
    /// 微信用户信息
    /// </summary>
    public class WxUserInfo
    {
        public string openid;
        public string nickname;
        public int sex;
        public string province;
        public string city;
        public string country;
        public string headimgurl;
        public string privilege;
        public string unionid;
    }
    /// <summary>
    /// 微信access_token
    /// </summary>
    public static class WxUtil
    {
        private static string appid = ConfigurationManager.AppSettings["appid"];
        private static string appsecret = ConfigurationManager.AppSettings["appsecret"];
        /// <summary>
        /// 获取access_token
        /// </summary>
        /// <returns></returns>
        public static WxAt GetAccess_Token()
        {

            string url = @"https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=" + appid + "&secret=" + appsecret;
            WebClient wc = new WebClient();
            string strReturn = wc.DownloadString(url);
            try
            {
                WxAt wa = JsonConvert.DeserializeObject<WxAt>(strReturn);
                return wa;
            }
            catch (Exception e)
            {
                MyLog.writeLog(strReturn, e);
                throw e;
            }
        }

        /// <summary>
        /// 获取Openid的access_token
        /// </summary>
        /// <param name="strcode"></param>
        /// <returns></returns>
        public static WxOpenIdAt GetOpenIdAccess_Token(string strcode)
        {
            string url = @" https://api.weixin.qq.com/sns/oauth2/access_token?appid=" + appid + "&secret=" + appsecret + "&code=" + strcode + "&grant_type=authorization_code ";
            WebClient wc = new WebClient();
            string strReturn = wc.DownloadString(url);
            try
            {
                WxOpenIdAt woa = JsonConvert.DeserializeObject<WxOpenIdAt>(strReturn);
                return woa;
            }
            catch (Exception e)
            {
                MyLog.writeLog(strReturn, e);
                throw e;
            }
        }



        /// <summary>
        /// 验证openid的access_token是否过去
        /// </summary>
        /// <param name="woa"></param>
        /// <returns></returns>
        public static bool CheckOpenIdAccess_TokenTimeOut(WxOpenIdAt woa)
        {
            string url = @"https://api.weixin.qq.com/sns/auth?access_token=" + woa.access_token + "&openid=" + woa.openid;
            WebClient wc = new WebClient();
            string strReturn = wc.DownloadString(url);
            if (strReturn.IndexOf("OK") > -1)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="woa"></param>
        /// <returns></returns>
        public static WxUserInfo GetWxUserInfo(WxOpenIdAt woa)
        {
            string url = @" https://api.weixin.qq.com/sns/userinfo?access_token=" + woa.access_token + "&openid=" + woa.openid + "&lang=zh_CN ";
            WebClient wc = new WebClient();
            string strReturn = wc.DownloadString(url);
            try
            {
                WxUserInfo wu = JsonConvert.DeserializeObject<WxUserInfo>(strReturn);
                return wu;
            }
           catch(Exception e)
            {
                MyLog.writeLog(strReturn, e);
                throw e;
            }
        }

    }

}
