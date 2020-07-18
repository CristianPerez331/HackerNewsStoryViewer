using RestApi.Contracts.HackerNews;
using System.Collections.Generic;

namespace RestApiUnitTests.MockContracts
{
    class MockHackerNewsContractFactory
    {

        public static HackerNewsItem CreateMockHackerNewsItem()
            => new HackerNewsItem()
            {
                Id = 1,
                Author = "Me",
                TimePublished = 1,
                Title = "Title",
                Url = "http://Url.url/url"
            };

        public static HackerNewsData CreateHackerNewsData()
            => new HackerNewsData()
            {
                ResultCount = 1,
                TotalPages = 1,
                HackerNewsItems = new List<HackerNewsItem> { CreateMockHackerNewsItem() }
            };

    }
}
