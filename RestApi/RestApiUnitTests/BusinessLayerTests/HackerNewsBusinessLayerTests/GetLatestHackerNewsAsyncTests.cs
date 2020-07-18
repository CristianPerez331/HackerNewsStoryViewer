using Moq;
using RestApi.BusinessLayers;
using RestApi.Contracts.HackerNews;
using RestApi.Repositories.Interfaces;
using RestApiUnitTests.MockContracts;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace RestApiUnitTests.BusinessLayerTests.HackerNewsBusinessLayerTests
{
    public class GetLatestHackerNewsAsyncTests
    {
        private HackerNewsBusinessLayer _businessLayer;
        private Mock<IHackerNewsRepository> _hackerNewsRepository;
        private HackerNewsItem _hackerNewsItem;
        private int _page;
        private int _pageSize;
        private string _titleSearchQuery;

        public GetLatestHackerNewsAsyncTests()
        {
            _hackerNewsItem = MockHackerNewsContractFactory.CreateMockHackerNewsItem();
            _hackerNewsRepository = new Mock<IHackerNewsRepository>(MockBehavior.Strict);
            SetupMockRepository(new List<int>() { _hackerNewsItem.Id }, _hackerNewsItem);

            _page = 1;
            _pageSize = 10;
            _titleSearchQuery = null;

            _businessLayer = new HackerNewsBusinessLayer(_hackerNewsRepository.Object);
        }

        [Fact]
        public async Task GetLatestHackerNewsAsync_CallMethod_CallsRepositoryFunctionsCorrectly()
        {
            // we have to set the id here to something different because if not when running these tests one after another
            // the GetHackerNewsItemAsync function won't be called because of the caching
            _hackerNewsItem.Id = 5000000;
            SetupMockRepository(new List<int>() { _hackerNewsItem.Id }, _hackerNewsItem);

            await CallMethod();

            _hackerNewsRepository.Verify(x => x.GetLatestHackerNewsStoryIdsAsync(), Times.Once());
            _hackerNewsRepository.Verify(x => x.GetHackerNewsItemAsync(_hackerNewsItem.Id), Times.Once());
            _hackerNewsRepository.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task GetLatestHackerNewsAsync_CallMethodTwice_CachesCorrectly()
        {
            await CallMethod();
            await CallMethod();

            _hackerNewsRepository.Verify(x => x.GetLatestHackerNewsStoryIdsAsync(), Times.Exactly(2));
            _hackerNewsRepository.Verify(x => x.GetHackerNewsItemAsync(_hackerNewsItem.Id), Times.AtMostOnce());
            _hackerNewsRepository.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task GetLatestHackerNewsAsync_NegativePageSize_ReturnsAllData()
        {
            _pageSize = -1;
            var result = await CallMethod();

            Assert.Equal(1, result.ResultCount);
            Assert.Equal(1, result.TotalPages);
            Assert.Single(result.HackerNewsItems);
            Assert.False(result.HasError);
        }

        [Fact]
        public async Task GetLatestHackerNewsAsync_NegativePage_ReturnsAllData()
        {
            _page = -1;
            var result = await CallMethod();

            Assert.Equal(1, result.ResultCount);
            Assert.Equal(1, result.TotalPages);
            Assert.Single(result.HackerNewsItems);
        }

        [Fact]
        public async Task GetLatestHackerNewsAsync_PageThatDoesntExist_ReturnsNoData()
        {
            _page = 1000000;
            var result = await CallMethod();

            Assert.Equal(1, result.ResultCount);
            Assert.Equal(1, result.TotalPages);
            Assert.Empty(result.HackerNewsItems);
        }

        [Fact]
        public async Task GetLatestHackerNewsAsync_SearchQueryDoesntExist_FiltersCorrectly()
        {
            _titleSearchQuery = "Not Here";
            var result = await CallMethod();
            Assert.Empty(result.HackerNewsItems);
        }

        [Fact]
        public async Task GetLatestHackerNewsAsync_SearchQueryExists_FiltersCorrectly()
        {
            _titleSearchQuery = "Here";
            _hackerNewsItem.Title = "This is here";
            var result = await CallMethod();
            Assert.Single(result.HackerNewsItems);
        }

        [Fact]
        public async Task GetLatestHackerNewsAsync_NewsItemRepositoryCallFails_HasErrorIsTrue()
        {
            // we have to set the id here to something different because if not when running these tests one after another
            // the GetHackerNewsItemAsync function won't be called because of the caching
            _hackerNewsItem.Id = 6000000;
            SetupMockRepository(new List<int>() { _hackerNewsItem.Id }, (HackerNewsItem)null);

            var result = await CallMethod();

            Assert.True(result.HasError);
        }

        [Fact]
        public async Task GetLatestHackerNewsAsync_NewsStoryIdsRepositoryCallFails_HasErrorIsTrue()
        {
            _hackerNewsRepository.Setup(x => x.GetLatestHackerNewsStoryIdsAsync()).ReturnsAsync((List<int>)null);

            var result = await CallMethod();

            Assert.True(result.HasError);
        }


        private void SetupMockRepository(List<int> newsStoryIds, HackerNewsItem newsItem)
        {
            _hackerNewsRepository.Setup(x => x.GetLatestHackerNewsStoryIdsAsync()).ReturnsAsync(newsStoryIds);
            _hackerNewsRepository.Setup(x => x.GetHackerNewsItemAsync(_hackerNewsItem.Id)).ReturnsAsync(newsItem);
        }

        private async Task<HackerNewsData> CallMethod()
            => await _businessLayer.GetLatestHackerNewsAsync(_page, _pageSize, _titleSearchQuery);

    }
}
