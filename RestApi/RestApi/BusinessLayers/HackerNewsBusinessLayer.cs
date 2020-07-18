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
            // this will retrieve the newest story ids
            // in the future we can probably add a variable to store the latest time these were gotten
            // and then only get it after x time has passed
            var latestStoryIds = await _hackerNewsRepository.GetLatestHackerNewsStoryIdsAsync();

            // if that repository returns nothing then an error happened and we want to return that
            if (latestStoryIds == null)
                return new HackerNewsData() { HasError = true };

            // Depending on whether or not there is a search query we have to gather the data in two different ways
            // The main reason for this is to make sure we only have to get the data for the range decided on by page/pageSize when we are not searching
            // that way we dont have to make sure we have latestStoryIds.Count amount of elements everytime this runs
            var hackerNewsData = titleSearchQuery == null ?
                await GetLatestHackerData(latestStoryIds, page, pageSize) :
                await SearchLatestHackerData(latestStoryIds, page, pageSize, titleSearchQuery);

            // fire and forget for the rest of the data so it will be ready later
            // but we don't have to wait to return the data for this call
            UpdateCacheWithLatestStoryData(latestStoryIds).ConfigureAwait(false);

            return hackerNewsData;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// This will determine how many results need to be sent back, gather the data for them, and then return it.
        /// </summary>
        private async Task<HackerNewsData> GetLatestHackerData(List<int> idList, int page, int pageSize)
        {
            // we first want to gather information so we can decide the range of the idList to send back
            var totalPages = GetTotalPages(pageSize, idList.Count);
            var dataStartLocation = GetDataStartLocation(page, totalPages, pageSize);
            var amountOfDataToCollect = GetDataToCollectCount(page, totalPages, pageSize, idList.Count);

            // we then can limit the idList to the range we decided on
            var storyIdsToRetreive = idList.GetRange(dataStartLocation, amountOfDataToCollect);

            // we now need to get the data for those specific ids
            var hasError = await UpdateCacheWithLatestStoryData(storyIdsToRetreive);

            // now that we have the data in the cache we can go through and create our response
            return new HackerNewsData()
            {
                TotalPages = totalPages,
                ResultCount = idList.Count,
                HackerNewsItems = GetHackerNewsItemsFromCache(storyIdsToRetreive, null),
                HasError = hasError
            };
        }


        /// <summary>
        /// This will retrieve all the data for the ids in the idList. It will then search through it and get back only the elements that contain the titleSearchQuery in the title.
        /// After it will determine how many results need to be sent back from those result, and then it will return those results.
        /// </summary>
        private async Task<HackerNewsData> SearchLatestHackerData(List<int> idList, int page, int pageSize, string titleSearchQuery)
        {
            // we need to first get all the data for the idList so that we can search through it
            var hasError = await UpdateCacheWithLatestStoryData(idList);

            // we then want to create a full list of HackerNewsItems that contain the titleSearchQuery so we can 
            // figure out how to limit it based on the pagination
            var itemsToReturn = GetHackerNewsItemsFromCache(idList, titleSearchQuery);

            // here we calculate the pagination
            var totalPages = GetTotalPages(pageSize, itemsToReturn.Count);
            var dataStartLocation = GetDataStartLocation(page, totalPages, pageSize);
            var amountOfDataToCollect = GetDataToCollectCount(page, totalPages, pageSize, itemsToReturn.Count);

            // now that we have all the information we need, we can return it.
            return new HackerNewsData()
            {
                TotalPages = totalPages,
                ResultCount = itemsToReturn.Count,
                HackerNewsItems = itemsToReturn.GetRange(dataStartLocation, amountOfDataToCollect),
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
            // if the page size is 0 or negative we just want to return all the data
            if (pageSize < 1)
                return totalDataCount;

            // if the page we are on is above the total page size then we want to return 0
            // we also want to return 0 if the user passed in a negative page
            else if (page >= totalPages || page < 0)
                return 0;

            // if we are on the last page then we want to return whatever is left
            else if (pageSize * (page + 1) > totalDataCount)
                return totalDataCount - (pageSize * page);

            // we now return the pageSize that was requested now that we know its possible
            return pageSize;
        }

        /// <summary>
        /// This will get the HackerNewsItems from the cache and filter the results by whether or not the title contains the title search query
        /// </summary>
        private List<HackerNewsItem> GetHackerNewsItemsFromCache(IReadOnlyCollection<int> storyIdsToRetreive, string titleSearchQuery)
        {
            // if the title search query is empty we can just return back all the storyIds
            if (titleSearchQuery == null)
                return storyIdsToRetreive
                    .Where(x => hackerNewsItemsDictionary.ContainsKey(x))
                    .Select(x => hackerNewsItemsDictionary[x])
                    .ToList();
            // if the title search query is not empty we need to search for when the title of the story contains the title search query
            else
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

            // for every id in the latestStoryIds list spawn a new task to validate that the data for it exists in the static dictionary if not we will add it
            await Task.WhenAll(latestStoryIds.Select(async x =>
            {
                if (!hackerNewsItemsDictionary.ContainsKey(x))
                {
                    var item = await _hackerNewsRepository.GetHackerNewsItemAsync(x);

                    // if the item from the repository comes back as null or we cant add it to the dictionary for some reason
                    // we want to say that theres an error that happened somewhere
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
