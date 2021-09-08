using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PowerSystemLibrary.Entity;
using PowerSystemLibrary.Enum;

namespace PowerSystemLibrary.Util
{
    public static class WeChatAPI
    {
        public class TemporaryAccessToken
        {
            public string AccessToken { get; set; }
            public string Secret { get; set; }
            //企业微信为7200秒
            public DateTime Expire { get; set; }
        }
        public static List<TemporaryAccessToken> temporaryAccessTokenList = new List<TemporaryAccessToken>();

        public static JObject BlindUser(string access_token, User user)
        {
            StringBuilder sBuilder = new StringBuilder();
            sBuilder.Append("{");
            sBuilder.Append("\"userid\": \"" + user.Cellphone + "\",");
            sBuilder.Append("\"name\": \"" + user.Realname + "\",");
            sBuilder.Append("\"department\": [1],");
            sBuilder.Append("\"mobile\": \"" + user.Cellphone + "\",");
            sBuilder.Append("\"enable\": " + 1 + ",");
            sBuilder.Append("\"extattr\": {}");
            sBuilder.Append("}");

            string result = RequestHelper.Post(string.Format("https://qyapi.weixin.qq.com/cgi-bin/user/create?access_token={0}", access_token), sBuilder.ToString());
            return (JObject)JsonConvert.DeserializeObject(result);
        }

        public static JObject DeleteUser(string access_token, List<User> userList)
        {
            JObject userJObject = new JObject();
            JArray userWeChatIDJArray = new JArray();
            foreach (User user in userList)
            {
                userWeChatIDJArray.Add(user.WeChatID);
            }
            userJObject.Add("useridlist", userWeChatIDJArray);

            string result = RequestHelper.Post(string.Format("https://qyapi.weixin.qq.com/cgi-bin/user/batchdelete?access_token={0}", access_token), JsonConvert.SerializeObject(userJObject));
            return (JObject)JsonConvert.DeserializeObject(result);
        }

        public static string GetUserInfo(string access_token, string code)
        {
            string result = RequestHelper.RequestUrl(string.Format("https://qyapi.weixin.qq.com/cgi-bin/user/getuserinfo?access_token={0}&code={1}", access_token, code));
            if (RequestHelper.Json(result, "UserId") != "")
                return RequestHelper.Json(result, "UserId");
            else
                return RequestHelper.Json(result, "errcode");
        }

        /// <summary>
        /// 获取微信用户信息
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="userid">用户id</param>
        /// <returns>手机号</returns>
        public static string GetUser(string access_token, string userid)
        {
            return RequestHelper.RequestUrl(string.Format("https://qyapi.weixin.qq.com/cgi-bin/user/get?access_token={0}&userid={1}", access_token, userid));
            //return Json(user, "mobile");
        }

        /// <summary>
        /// 获取access_token
        /// </summary>
        /// <param name="CorpID"></param>
        /// <param name="CorpSecret"></param>
        /// <returns></returns>
        public static string GetAccessToken(string CorpID, string CorpSecret)
        {
            string access_Token = RequestHelper.RequestUrl(string.Format("https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid={0}&corpsecret={1}", CorpID, CorpSecret));
            return RequestHelper.Json(access_Token, "access_token");

        }

        /// <summary>
        /// 文本消息
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="touser"></param>
        /// <param name="agentid"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string SendMessage(string access_token, string touser, string agentid, string content)
        {
            string json = "{\"touser\":\"" + touser + "\",\"msgtype\":\"text\",\"agentid\":\"" + agentid + "\",\"text\":{\"content\":\"" + content + "\"}}";
            string result = RequestHelper.Post(string.Format("https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token={0}", access_token), json);
            return RequestHelper.Json(result, "errmsg");
        }

        /// <summary>
        /// 卡片链接消息
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="touser"></param>
        /// <param name="agentid"></param>
        /// <param name="serverid"></param>
        /// <returns></returns>
        public static string SendMessagePhoto(string access_token, string touser, int agentid, string serverid)
        {
            string json = "{\"touser\":\"" + touser
                        + "\",\"msgtype\":\"image\",\"agentid\":\"" + agentid
                        + "\",\"image\":{\"media_id\":\"" + serverid + "\"}}";
            string result = RequestHelper.Post(string.Format("https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token={0}", access_token), json);
            return RequestHelper.Json(result, "errmsg");
        }

        /// <summary>
        /// 发送文本卡片消息
        /// </summary>
        /// <param name="access_token"></param>
        /// <param name="touser"></param>
        /// <param name="agentid"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="url"></param>
        /// <param name="senddate"></param>
        /// <param name="sendusername"></param>
        /// <returns></returns>
        public static string SendMessageCard(string access_token, string touser, string agentid, string title, string description, string url, string senddate, string sendusername)
        {
            string json = "{\"touser\" : \"" + touser + "\",\"msgtype\" : \"textcard\",\"agentid\" :" + agentid
                + ",\"textcard\" : {\"title\" : \"" + title
                  //+ "\",\"description\" : \"<div class=\\\"gray\\\">" + title + "</div> <div class=\\\"normal\\\">" + description + "</div><div class=\\\"highlight\\\">政同事领取</div>\",\"url\" : \""+ url+"\",\"btntxt\":\"更多\"}";
                  + "\",\"description\" : \"<div class=\\\"highlight\\\">通知内容：" + description + "</div><div class=\\\"normal\\\">发送人： " + sendusername + "</div><div class=\\\"gray\\\"> 时间： " + senddate + "</div> \",\"url\" : \"" + url + "\",\"btntxt\":\"更多\"}";


            string result = RequestHelper.Post(string.Format("https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token={0}", access_token), json);
            return RequestHelper.Json(result, "errmsg");
        }

        public static string GetToken(string corpID, string secret)
        {
            DateTime date = DateTime.Now;

            string accessToken = string.Empty;

            TemporaryAccessToken temporaryAccessToken = temporaryAccessTokenList.FirstOrDefault(t => t.Secret == secret);
            if (temporaryAccessToken == null)
            {
                temporaryAccessToken = new TemporaryAccessToken
                {
                    AccessToken = GetAccessToken(corpID, secret),
                    Secret = secret,
                    //过期时间
                    Expire = date.AddSeconds(6800),
                };
                temporaryAccessTokenList.Add(temporaryAccessToken);
                accessToken = temporaryAccessToken.AccessToken;
            }
            else
            {
                //未过有效期
                if (temporaryAccessToken.Expire > date)
                {
                    accessToken = temporaryAccessToken.AccessToken;

                }
                //过了有效期
                else
                {
                    temporaryAccessToken.AccessToken = GetAccessToken(corpID, secret);
                    temporaryAccessToken.Expire = date.AddSeconds(6800);
                    accessToken = temporaryAccessToken.AccessToken;
                }
            }
            return accessToken;
        }



    }
}
