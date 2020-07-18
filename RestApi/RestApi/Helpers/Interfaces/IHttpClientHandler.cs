using System.Net.Http;
using System.Threading.Tasks;

namespace RestApi.Helpers.Interfaces
{
    public interface IHttpClientHandler
    {

        /// <summary>
        /// This will call the HttpClient Get Async function
        /// </summary>
        /// <param name="url">The url to access</param>
        Task<HttpResponseMessage> GetAsync(string url);

    }
}
