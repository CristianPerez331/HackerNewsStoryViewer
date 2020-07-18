using RestApi.Contracts.HackerNews;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestApi.Repositories.Interfaces
{
    public interface IHackerNewsRepository
    {
        /// <summary>
        /// This will retrieve the latest news story ids from the hacker news api asynchronously
        /// https://hacker-news.firebaseio.com/v0/newstories.json
        /// </summary>
        Task<List<int>> GetLatestHackerNewsStoryIdsAsync();

        /// <summary>
        /// This will retrieve information from the specified story id from the hacker news api asynchronously
        /// https://hacker-news.firebaseio.com/v0/item/<paramref name="id"/>.json
        /// </summary>
        /// <param name="id">The identifier to get</param>
        Task<HackerNewsItem> GetHackerNewsItemAsync(int id);
    }
}
