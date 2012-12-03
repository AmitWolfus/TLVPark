using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace TLVPark.DataAccess.FacebookQuerying
{
    public class FacebookQueryFactory
    {
        #region Other Methods

        public static IFacebookQuery CreateQuery()
        {
            return new FacebookQuery(GetAccessToken());
        }

        /// <summary>
        /// Gets the application's facebook access token
        /// </summary>
        private static string GetAccessToken()
        {
            // Get the id and secret of the application
            var clientId = ConfigurationManager.AppSettings["fb_client_id"];
            var clientSecret = ConfigurationManager.AppSettings["fb_client_secret"];
            // Create the uri to facebook to get the access token
            var uri =
                string.Format(
                    "https://graph.facebook.com/oauth/access_token?client_id={0}&client_secret={1}&grant_type=client_credentials",
                    clientId, clientSecret);
            var request = HttpWebRequest.Create(uri) as HttpWebRequest;
            request.Method = "GET";
            using (var response = request.GetResponse() as HttpWebResponse)
            {
                using (var stream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd().Replace("access_token=", "");
                    }
                }
            }
        }

        #endregion
    }
}