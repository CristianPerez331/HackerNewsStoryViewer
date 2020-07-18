using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace RestApi.Contracts.HackerNews
{
    [DataContract]
    public class HackerNewsData
    {
        [DataMember(Name = "totalResultCount")]
        public int ResultCount { get; set; }
        
        [DataMember(Name = "totalPages")]
        public int TotalPages { get; set; }
        
        [DataMember(Name = "data")]
        public IEnumerable<HackerNewsItem> HackerNewsItems { get; set; }
        
        [DataMember(Name = "hasError")]
        public bool? HasError { get; set; }

    }
}