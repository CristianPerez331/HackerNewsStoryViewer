using Moq;
using RestApi.Constants;
using RestApi.Helpers.Interfaces;
using RestApi.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace RestApiUnitTests.RepositoryTests.HackerNewsRepositoryTests
{
    public class GetLatestHackerNewsStoryIdsAsyncTests
    {
        private HackerNewsRepository _repository;
        private Mock<IApiAccessHelper> _apiAccessHelper;
        private List<int> _idList;

        public GetLatestHackerNewsStoryIdsAsyncTests()
        {
            _idList = new List<int>() { 1 };
            _apiAccessHelper = new Mock<IApiAccessHelper>(MockBehavior.Strict);
            _apiAccessHelper.Setup(x => x.GetApiAsync<List<int>>(ExternalApiUrls.HackerNewsLatestStoriesUrl)).ReturnsAsync(_idList);

            _repository = new HackerNewsRepository(_apiAccessHelper.Object);
        }

        [Fact]
        public async Task GetLatestHackerNewsStoryIdsAsync_CallMethod_ReturnsCorrectResult()
        {
            var result = await CallMethod();

            Assert.Equal(_idList, result);
        }

        [Fact]
        public async Task GetLatestHackerNewsStoryIdsAsync_CallMethod_CallsRepositoryFunctionsCorrectly()
        {
            await CallMethod();

            _apiAccessHelper.Verify(x => x.GetApiAsync<List<int>>(ExternalApiUrls.HackerNewsLatestStoriesUrl), Times.Once());
            _apiAccessHelper.VerifyNoOtherCalls();
        }


        private async Task<List<int>> CallMethod()
            => await _repository.GetLatestHackerNewsStoryIdsAsync();

    }
}
