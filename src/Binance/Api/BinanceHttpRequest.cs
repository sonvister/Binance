using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text;

// ReSharper disable once CheckNamespace
namespace Binance
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
        /// Get the path and query string.
        /// </summary>
        public string PathAndQuery => _parameters.Count == 0 ? _path : $"{_path}?{QueryString}";

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

        /// <summary>
        /// Get the total parameters string.
        /// </summary>
        public string TotalParams => QueryString + Body;

        #endregion Public Properties

        #region Private Constants

        private const string RequestHeaderKeyName = "X-MBX-APIKEY";

        #endregion Private Constants

        #region Private Fields

        private readonly IDictionary<string, string> _parameters
            = new Dictionary<string, string>();

        private string _queryString;

        private readonly string _path;

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

        public void AddParameter(string field, decimal value)
            => AddParameter<decimal>(field, value.Normalize());

        public void AddParameter<T>(string field, T value)
        {
            Throw.IfNull(value, nameof(value));

            if (_parameters.ContainsKey(field))
                throw new InvalidOperationException($"{nameof(BinanceHttpRequest)}: request already has a '{field}' parameter.");

            if (_parameters.ContainsKey("signature"))
                throw new InvalidOperationException($"{nameof(BinanceHttpRequest)}: all parameters must be added before request is signed.");

            _parameters[field] = Convert.ToString(value, CultureInfo.InvariantCulture);

            _queryString = null; // reset query string.
        }

        public HttpRequestMessage CreateMessage(HttpMethod method)
        {
            var requestMessage = new HttpRequestMessage(method, PathAndQuery);

            if (ApiKey != null)
            {
                requestMessage.Headers.Add(RequestHeaderKeyName, ApiKey);
            }

            if (method == HttpMethod.Get && Body != null)
                throw new InvalidOperationException($"{nameof(BinanceHttpRequest)}: parameters must be sent in query string for GET requests ({nameof(Body)} must be null).");

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
