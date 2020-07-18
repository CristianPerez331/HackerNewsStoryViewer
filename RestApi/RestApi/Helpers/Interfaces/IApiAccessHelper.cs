using System.Threading.Tasks;

namespace RestApi.Helpers.Interfaces
{
    public interface IApiAccessHelper
    {
        /// <summary>
        /// This will retrieve data from an external api asynchronously
        /// </summary>
        /// <param name="url">Where to get the data from</param>
        /// <typeparam name="T">The data retrieved will be deserialized into this object type</typeparam>
        /// <returns>The data from the GET external api endpoint</returns>
        Task<T> GetApiAsync<T>(string url);
    }
}
