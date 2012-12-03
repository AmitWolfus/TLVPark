using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace TLVPark.DataAccess.FacebookQuerying
{
    internal class FacebookQuery : IFacebookQuery
    {
        #region Instance Variables

        private const string BASE_ADDRESS = "https://api.facebook.com/method/fql.query?query=";

        private readonly string _accessToken;
        private bool _isDisposed;
        private string[] _columns;
        private FacebookTable _table;
        private long _id;

        #endregion

        #region Constructors

        public FacebookQuery(string accessToken)
        {
            _accessToken = accessToken;
        }

        #endregion

        #region IFacebookQuery Methods

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            _isDisposed = true;
        }

        public IFacebookQuery Select(params string[] columns)
        {
            _columns = columns;
            return this;
        }

        public IFacebookQuery From(FacebookTable table)
        {
            _table = table;
            return this;
        }

        public IFacebookQuery WhereId(long id)
        {
            _id = id;
            return this;
        }

        public Dictionary<string, string> Execute()
        {
            // build the query
            var query = new StringBuilder(string.Format("{0}SELECT ",BASE_ADDRESS));
            foreach (var column in _columns)
            {
                query.Append(string.Format("{0},", column));
            }
            query.Remove(query.Length - 1, 1);
            query.Append(string.Format(" FROM {0}", Enum.GetName(typeof (FacebookTable), _table).ToLower()));
            query.Append(string.Format(" WHERE page_id={0}", _id));
            query.Append(string.Format("&access_token={0}", _accessToken));
            var request = HttpWebRequest.Create(query.ToString()) as HttpWebRequest;
            request.Method = "GET";
            string xmlData;
            using (var response = request.GetResponse() as HttpWebResponse)
            {
                using (var stream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(stream))
                    {
                        xmlData = reader.ReadToEnd().Replace("&lt;", "<").Replace("&gt;", ">");
                    }
                }
            }

            return ParsePlace(xmlData);
        }

        #endregion

        #region Other Methods

        private static Dictionary<string, string> ParsePlace(string xmlData)
        {
            var rootElement = XElement.Load(new StringReader(xmlData));
            var place =
                rootElement.DescendantNodes()
                           .Where(element => element is XElement)
                           .Cast<XElement>().FirstOrDefault(element => element.Name.LocalName == "place");
            if (place == null)
            {
                return null;
            }
            var dictionary = new Dictionary<string, string>();
            foreach (XElement element in place.DescendantNodes().Where(node => node is XElement))
            {
                dictionary.Add(element.Name.LocalName, element.Value);
            }
            return dictionary;
        }

        #endregion
    }
}