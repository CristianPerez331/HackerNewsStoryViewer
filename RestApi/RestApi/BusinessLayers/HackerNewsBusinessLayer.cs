using RestApi.BusinessLayers.Interfaces;
using RestApi.Contracts.HackerNews;
using RestApi.Repositories.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApi.BusinessLayers
{
    /// <summary>
    /// This class is where the business logic for getting Hacker News is
    /// </summary>
    public class HackerNewsBusinessLayer : IHackerNewsBusinessLayer
    {
        #region Fields

        private IHackerNewsRepository _hackerNewsRepository;
        private static ConcurrentDictionary<int, HackerNewsItem> hackerNewsItemsDictionary = new ConcurrentDictionary<int, HackerNewsItem>();

        #endregion

        #region Contructor

        public HackerNewsBusinessLayer(IHackerNewsRepository hackerNewsRepository)
        {
            _hackerNewsRepository = hackerNewsRepository;
        }

        #endregion

        #region IHackerNewsManager

        /// <summary>
        /// This will retrieve the latest news from the hacker news api asynchronously
        /// </summary>
        /// <param name="page">This is which page of data are we trying to get</param>
        /// <param name="pageSize">This is how large are the pages</param>
        /// <param name="titleSearchQuery">This will filter items to those which contains this string in the title</param>
        public async Task<HackerNewsData> GetLatestHackerNewsAsync(int page, int pageSize, string titleSearchQuery)
        {
            var latestStoryIds = await _hackerNewsRepository.GetLatestHackerNewsStoryIdsAsync();

            if (latestStoryIds == null)
                return new HackerNewsData() { HasError = true };

            var hackerNewsData = titleSearchQuery == null ? await GetLatestHackerData(latestStoryIds, page, pageSize) : await SearchLatestHackerData(latestStoryIds, page, pageSize, titleSearchQuery);

            // fire and forget for the rest of the data so it will be ready later
            // but we don't have to wait to return the data
            UpdateCacheWithLatestStoryData(latestStoryIds).ConfigureAwait(false);

            return hackerNewsData;
        }

        #endregion

        #region Helpers

        private async Task<HackerNewsData> GetLatestHackerData(List<int> idList, int page, int pageSize)
        {
            var totalPages = GetTotalPages(pageSize, idList.Count);
            var dataStartLocation = GetDataStartLocation(page, totalPages, pageSize);
            var amountOfDataToCollect = GetDataToCollectCount(page, totalPages, pageSize, idList.Count);

            var storyIdsToRetreive = idList.GetRange(dataStartLocation, amountOfDataToCollect);

            var hasError = await UpdateCacheWithLatestStoryData(storyIdsToRetreive);

            return new HackerNewsData()
            {
                TotalPages = totalPages,
                ResultCount = idList.Count,
                HackerNewsItems = FilterHackerNewsItems(storyIdsToRetreive, null),
                HasError = hasError
            };
        }

        private async Task<HackerNewsData> SearchLatestHackerData(List<int> idList, int page, int pageSize, string titleSearchQuery)
        {
            var hasError = await UpdateCacheWithLatestStoryData(idList);

            var itemsToReturn = FilterHackerNewsItems(idList, titleSearchQuery);

            var totalPages = GetTotalPages(pageSize, itemsToReturn.Count);
            var dataStartLocation = GetDataStartLocation(page, totalPages, pageSize);
            var amountOfDataToCollect = GetDataToCollectCount(page, totalPages, pageSize, itemsToReturn.Count);

            return new HackerNewsData()
            {
                TotalPages = totalPages,
                ResultCount = itemsToReturn.Count,
                HackerNewsItems = itemsToReturn.GetRange(dataStartLocation, GetDataToCollectCount(page, totalPages, pageSize, itemsToReturn.Count)),
                HasError = hasError
            };
        }

        /// <summary>
        /// This will return the total amount of pages we can return given a pageSize
        /// </summary>
        private int GetTotalPages(double pageSize, double latestStoryIdCount)
            => (pageSize < 1) ? 1 : (int) Math.Ceiling(latestStoryIdCount / pageSize);

        /// <summary>
        /// This will return where the start point should be when returning the data
        /// </summary>
        private int GetDataStartLocation(int page, int totalPages, int pageSize)
            => (page >= 0 && page < totalPages) ? page * pageSize : 0;

        /// <summary>
        /// This will return the total amount of data to return
        /// </summary>
        private int GetDataToCollectCount(int page, int totalPages, int pageSize, int totalDataCount)
        {
            if (pageSize < 1)
                return totalDataCount;
            else if (page == totalPages)
                return totalDataCount % pageSize;
            else if (page > totalPages)
                return 0;
            else if (pageSize * (page + 1) > totalDataCount)
                return totalDataCount - (pageSize * page);
            return pageSize;
        }

        /// <summary>
        /// This will filter the results by whether or not the title contains the title search query
        /// </summary>
        private List<HackerNewsItem> FilterHackerNewsItems(IReadOnlyCollection<int> storyIdsToRetreive, string titleSearchQuery)
        {
            if (titleSearchQuery == null)
                return storyIdsToRetreive
                    .Where(x => hackerNewsItemsDictionary.ContainsKey(x))
                    .Select(x => hackerNewsItemsDictionary[x])
                    .ToList();
            else
                //this next line can be more elaborate if need be
                return storyIdsToRetreive
                        .Where(x => hackerNewsItemsDictionary.ContainsKey(x))
                        .Select(x => hackerNewsItemsDictionary[x])
                        .Where(x => x?.Title != null ? ContainsText(x.Title, titleSearchQuery) : false)
                        .ToList();
        }

        /// <summary>
        /// This will return a boolean of whether or not the search text exists in the text
        /// </summary>
        private bool ContainsText(string text, string searchText)
            => text.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0;

        /// <summary>
        /// This will look through every id and for each one that does not exist in the cache
        /// it will then send the get request and add it to the dictionary
        /// </summary>
        private async Task<bool> UpdateCacheWithLatestStoryData(IReadOnlyCollection<int> latestStoryIds)
        {
            var hasError = false;

            await Task.WhenAll(latestStoryIds.Select(async x =>
            {
                if (!hackerNewsItemsDictionary.ContainsKey(x))
                {
                    var item = await _hackerNewsRepository.GetHackerNewsItemAsync(x);
                    if (item == null || !hackerNewsItemsDictionary.TryAdd(x, item))
                    {
                        hasError = true;
                    }
                }
            }));

            return hasError;
        }

        #endregion

    }
}
