using Moq;
using RestApi.BusinessLayers.Interfaces;
using RestApi.Contracts;
using RestApi.Controllers;
using RestApiUnitTests.MockContracts;
using System.Threading.Tasks;
using Xunit;

namespace RestApiUnitTests.ControllerTests.HackerNewsControllerTests
{
    public class GetLatestHackerNewsTests
    {
        private HackerNewsController _controller;
        private Mock<IHackerNewsBusinessLayer> _businessLayer;

        public GetLatestHackerNewsTests()
        {
            _businessLayer = new Mock<IHackerNewsBusinessLayer>(MockBehavior.Strict);
            _businessLayer.Setup(x =>
                x.GetLatestHackerNewsAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>())
            ).ReturnsAsync(MockHackerNewsContractFactory.CreateHackerNewsData());
            _controller = new HackerNewsController(_businessLayer.Object);
        }


        [Fact]
        public async Task GetLatestHackerNews_CallMethodWithFilledParameters_CallsBusinessLayerCorrectly()
        {
            await _controller.GetLatestHackerNews(70, 70, "Test String");
            _businessLayer.Verify(x => x.GetLatestHackerNewsAsync(70, 70, "Test String"), Times.Once());
            _businessLayer.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task GetLatestHackerNews_CallMethodWithNoParameters_CallsBusinessLayerCorrectlyWithDefaults()
        {
            await _controller.GetLatestHackerNews();
            _businessLayer.Verify(x => x.GetLatestHackerNewsAsync(0, 10, null), Times.Once());
            _businessLayer.VerifyNoOtherCalls();
        }

    }
}
