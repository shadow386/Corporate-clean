using FallenMoon.Library.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace FallenMoon.Library
{
    public class Twitter
    {
        private string OAuthConsumerKey = "";
        private string OAuthConsumerSecret = "";
        private string TwitterApi = "https://api.twitter.com/1.1/statuses/user_timeline.json?count={0}&screen_name={1}&exclude_replies=1";
        private string AccessToken = "";
        public List<Tweet> Tweets { get; set; }

        public Twitter()
        {
            AccessToken = GetAccessToken();
        }

        public string GetAccessToken()
        {
            using(WebClient client = new WebClient())
            {
                var customerInfo = Convert.ToBase64String(new UTF8Encoding().GetBytes(OAuthConsumerKey + ":" + OAuthConsumerSecret));
                Uri uri = new Uri("https://api.twitter.com/oauth2/token");

                client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                client.Headers.Add("Authorization", "Basic " + customerInfo);

                string json = client.UploadString(uri, "POST", "grant_type=client_credentials");
                dynamic item = JsonConvert.DeserializeObject<object>(json);
                return item["access_token"];
            }
        }

        public void LoadTweets(string userName, int count)
        {
            using (WebClient client = new WebClient())
            {
                var tweets = new List<Tweet>();
                var url = string.Format(TwitterApi, count, userName);

                client.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + AccessToken);

                var json = client.DownloadString(url);
                JArray jsonDat = JArray.Parse(json);
                for (int x = 0; x < 3; x++)
                {
                    var tweet = jsonDat[x];
                    var tweetId = tweet["id"].ToString();
                    var tweetUser = tweet["user"]["screen_name"].ToString();
                    var tweetUserName = tweet["user"]["name"].ToString();
                    var tweetText = tweet["text"].ToString();
                    var tweetDate = tweet["created_at"].ToString();

                    string Const_TwitterDateTemplate = "ddd MMM dd HH:mm:ss +ffff yyyy";

                    tweets.Add(new Tweet()
                    {
                        TweetId = tweetId,
                        TweetUser = tweetUser,
                        TweetUserName = tweetUserName,
                        TweetText = HighlightTwitter(tweetText),
                        TweetDate = DateTime.ParseExact(tweetDate, Const_TwitterDateTemplate, new System.Globalization.CultureInfo("en-US"))
                    });
                }

                Tweets = tweets.ToList();
            }
        }

        private static string HighlightTwitter(string input)
        {
            return input.ParseURL().ParseUsername().ParseHashtag();
        }
    }

    public class Tweet
    {
        public string TweetId { get; set; }
        public string TweetUser { get; set; }
        public string TweetUserName { get; set; }
        public string TweetText { get; set; }
        public DateTime TweetDate { get; set; }
    }
}
