using Moq;
using RestApi.Constants;
using RestApi.Helpers;
using RestApi.Helpers.Interfaces;
using RestApi.Repositories;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace RestApiUnitTests.HelperTests.ApiAccessHelperTests
{
    public class GetApiAsyncTests
    {
        private ApiAccessHelper _helper;
        private Mock<IHttpClientHandler> _httpClient; 
        private string _url;

        public GetApiAsyncTests()
        {
            _url = "http://url.url/url";
            _httpClient = new Mock<IHttpClientHandler>(MockBehavior.Strict);
            _httpClient.Setup(x => x.GetAsync(_url)).ReturnsAsync(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("[ 1 ]"),
            });
            
            _helper = new ApiAccessHelper(_httpClient.Object);
        }

        [Fact]
        public async Task GetApiAsync_CallMethod_CallsHttpClientFunctionCorrectly()
        {
            await CallMethod();

            _httpClient.Verify(x => x.GetAsync(_url), Times.Once());
            _httpClient.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task GetApiAsync_CallMethod_ReturnsCorrectList()
        {
            var result = await CallMethod();

            Assert.Single(result);
            Assert.Equal(1, result[0]);
        }

        private async Task<List<int>> CallMethod()
            => await _helper.GetApiAsync<List<int>>(_url);

    }
}
