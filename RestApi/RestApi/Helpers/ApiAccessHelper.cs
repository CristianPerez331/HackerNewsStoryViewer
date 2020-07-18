using RestApi.Helpers.Interfaces;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace RestApi.Helpers
{
    public class ApiAccessHelper : IApiAccessHelper
    {
        #region Fields

        // This is so that there only needs to be one httpClient created for the application
        private static IHttpClientHandler _httpClient;

        #endregion

        #region Constructor

        public ApiAccessHelper(IHttpClientHandler httpClient)
        {
            _httpClient = httpClient;
        }

        #endregion

        #region IApiAccessHelper

        /// <summary>
        /// This will retrieve data from an external api asynchronously
        /// </summary>
        /// <param name="url">Where to get the data from</param>
        /// <typeparam name="T">The data retrieved will be deserialized into this object type</typeparam>
        /// <returns>The data from the GET external api endpoint</returns>
        public async Task<T> GetApiAsync<T>(string url) 
        {
            try
            {                
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var resultAsString = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<T>(resultAsString);
                }
            } catch (Exception ex)
            {
                Console.WriteLine($"Exception in ApiAccessHelper. Message: {ex.Message}");
            }
            return default(T);
        }

        #endregion

    }
}
