using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Facebook;
using System.Dynamic;
using System.Net;
using Newtonsoft.Json;

namespace FallenMoon.Library
{
    public class Facebook
    {
        private string GraphURL = "https://graph.facebook.com/oauth/access_token?grant_type=client_credentials&client_id=APPID&client_secret=APPSECRET&scope=user_about_me,publish_stream,offline_access";
        private string AppId = "";
        private string Secret = "";
        private string PageId = "";
        private string AccessToken = "";

        public void Post(string message)
        {
            GraphURL = GraphURL.Replace("APPID", AppId).Replace("APPSECRET", Secret);

            var json = new WebClient().DownloadString(GraphURL);
            dynamic obj = JsonConvert.DeserializeObject(json);

            AccessToken = obj.access_token.ToString();

            FacebookClient client = new FacebookClient(AccessToken);

            dynamic parameters = new ExpandoObject();
            parameters.access_token = AccessToken;
            parameters.message = message;
            dynamic result = client.Post(PageId + "/feed", parameters);
        }
    }
}
