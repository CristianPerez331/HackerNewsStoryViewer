using RestApi.Contracts.HackerNews;
using System.Threading.Tasks;

namespace RestApi.BusinessLayers.Interfaces
{
    public interface IHackerNewsBusinessLayer
    {
        /// <summary>
        /// This will retrieve the latest news from the hacker news api asynchronously
        /// </summary>
        /// <param name="page">This is which page of data are we trying to get</param>
        /// <param name="pageSize">This is how large are the pages</param>
        /// <param name="titleSearchQuery">This will filter items to those which contains this string in the title</param>
        Task<HackerNewsData> GetLatestHackerNewsAsync(int page, int pageSize, string titleSearchQuery);
    }
}
