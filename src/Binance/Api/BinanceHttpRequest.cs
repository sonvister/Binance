using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Binance.Api
{
    public sealed class BinanceHttpRequest
    {
        #region Public Properties

        /// <summary>
        /// Get or set the API key.
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Get or set the message body.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Get the URI.
        /// </summary>
        public string Uri
        {
            get
            {
                if (_parameters.Count == 0)
                    return _path;

                return $"{_path}?{QueryString}";
            }
        }

        /// <summary>
        /// Get the request query string.
        /// </summary>
        public string QueryString
        {
            get
            {
                if (_queryString != null)
                    return _queryString;

                return _queryString = BuildQueryString();
            }
        }

        #endregion Public Properties

        #region Private Constants

        private const string RequestHeaderKeyName = "X-MBX-APIKEY";

        #endregion Private Constants

        #region Private Fields

        private readonly IDictionary<string, string> _parameters
            = new Dictionary<string, string>();

        private string _queryString;

        private string _path;

        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="path"></param>
        public BinanceHttpRequest(string path)
        {
            Throw.IfNullOrWhiteSpace(path, nameof(path));

            _path = path;
        }

        #endregion Constructors

        #region Public Methods

        public void AddParameter<T>(string field, T value)
        {
            Throw.IfNull(value, nameof(value));

            _parameters[field] = value.ToString();

            _queryString = null;
        }

        public HttpRequestMessage CreateMessage(HttpMethod method)
        {
            var requestMessage = new HttpRequestMessage(method, Uri);

            if (ApiKey != null)
            {
                requestMessage.Headers.Add(RequestHeaderKeyName, ApiKey);
            }

            if (Body != null)
            {
                requestMessage.Content = new StringContent(Body, Encoding.UTF8, "application/x-www-form-urlencoded");
            }

            return requestMessage;
        }

        #endregion Public Methods

        #region Private Methods

        private string BuildQueryString()
        {
            var query = new StringBuilder();

            foreach (var parameter in _parameters)
            {
                if (query.Length > 0) query.Append("&");

                query.Append($"{parameter.Key}={parameter.Value}");
            }

            return query.ToString();
        }

        #endregion Private Methods
    }
}
