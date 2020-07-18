using RestApi.Constants;
using RestApi.Contracts.HackerNews;
using RestApi.Helpers.Interfaces;
using RestApi.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestApi.Repositories
{
    public class HackerNewsRepository : IHackerNewsRepository
    {
        #region Fields

        private IApiAccessHelper _apiAccessHelper;

        #endregion

        #region Contructor

        public HackerNewsRepository(IApiAccessHelper apiAccessHelper)
        {
            _apiAccessHelper = apiAccessHelper;
        }

        #endregion

        #region IHackerNewsRepository

        /// <summary>
        /// This will retrieve the latest news story ids from the hacker news api asynchronously
        /// https://hacker-news.firebaseio.com/v0/newstories.json
        /// </summary>
        public async Task<List<int>> GetLatestHackerNewsStoryIdsAsync()
        {
            var url = ExternalApiUrls.HackerNewsLatestStoriesUrl;
            return await _apiAccessHelper.GetApiAsync<List<int>>(url);
        }
        
        /// <summary>
        /// This will retrieve information from the specified story id from the hacker news api asynchronously
        /// https://hacker-news.firebaseio.com/v0/item/<paramref name="id"/>.json
        /// </summary>
        /// <param name="id">The identifier to get</param>
        public async Task<HackerNewsItem> GetHackerNewsItemAsync(int id)
        {
            var url = string.Format(ExternalApiUrls.HackerNewsItemUrl, id);
            return await _apiAccessHelper.GetApiAsync<HackerNewsItem>(url);
        }

        #endregion

    }
}
