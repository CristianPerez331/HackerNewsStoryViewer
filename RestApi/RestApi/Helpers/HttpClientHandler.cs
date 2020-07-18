using RestApi.Helpers.Interfaces;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace RestApi.Helpers
{
    public class HttpClientHandler : IHttpClientHandler
    {
        #region Fields
        
        private static HttpClient _httpClient;

        #endregion

        #region Constructor

        public HttpClientHandler(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        #endregion

        #region IApiAccessHelper

        /// <summary>
        /// This will call the HttpClient Get Async function
        /// </summary>
        /// <param name="url">The url to access</param>
        public async Task<HttpResponseMessage> GetAsync(string url) 
            => await _httpClient.GetAsync(url);

        #endregion

    }
}
