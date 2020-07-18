using Microsoft.AspNetCore.Mvc;
using RestApi.BusinessLayers.Interfaces;
using RestApi.Contracts.HackerNews;
using System.Threading.Tasks;

namespace RestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HackerNewsController : ControllerBase
    {

        #region Fields

        private IHackerNewsBusinessLayer _hackerNewsManager;

        #endregion

        #region Constructor

        public HackerNewsController(IHackerNewsBusinessLayer hackerNewsManager)
        {
            _hackerNewsManager = hackerNewsManager;
        }

        #endregion

        #region Routes
        
        [HttpGet]
        public async Task<HackerNewsData> GetLatestHackerNews(int page = 0, int pageSize = 10, string includes = null)
            => await _hackerNewsManager.GetLatestHackerNewsAsync(page, pageSize, includes);

        #endregion
    }
}
