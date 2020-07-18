using Moq;
using RestApi.Constants;
using RestApi.Contracts.HackerNews;
using RestApi.Helpers.Interfaces;
using RestApi.Repositories;
using RestApiUnitTests.MockContracts;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace RestApiUnitTests.RepositoryTests.HackerNewsRepositoryTests
{
    public class GetHackerNewsItemAsyncTests
    {
        private HackerNewsRepository _repository;
        private Mock<IApiAccessHelper> _apiAccessHelper;
        private HackerNewsItem _hackerNewsItem;
        private int _id;

        public GetHackerNewsItemAsyncTests()
        {
            _id = 1;
            _hackerNewsItem = MockHackerNewsContractFactory.CreateMockHackerNewsItem();
            _apiAccessHelper = new Mock<IApiAccessHelper>(MockBehavior.Strict);
            _apiAccessHelper.Setup(x =>
                x.GetApiAsync<HackerNewsItem>(string.Format(ExternalApiUrls.HackerNewsItemUrl, _id))
            ).ReturnsAsync(_hackerNewsItem);

            _repository = new HackerNewsRepository(_apiAccessHelper.Object);
        }

        [Fact]
        public async Task GetHackerNewsItemAsync_CallMethod_ReturnsCorrectResult()
        {
            var result = await CallMethod();

            Assert.Equal(_hackerNewsItem, result);
        }

        [Fact]
        public async Task GetHackerNewsItemAsync_CallMethod_CallsRepositoryFunctionsCorrectly()
        {
            await CallMethod();

            _apiAccessHelper.Verify(x => x.GetApiAsync<HackerNewsItem>(string.Format(ExternalApiUrls.HackerNewsItemUrl, _id)), Times.Once());
            _apiAccessHelper.VerifyNoOtherCalls();
        }


        private async Task<HackerNewsItem> CallMethod()
            => await _repository.GetHackerNewsItemAsync(_id);

    }
}
