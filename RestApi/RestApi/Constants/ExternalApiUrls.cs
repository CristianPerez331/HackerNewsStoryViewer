using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApi.Constants
{
    public class ExternalApiUrls
    {
        public static string HackerNewsLatestStoriesUrl = "https://hacker-news.firebaseio.com/v0/newstories.json";
        public static string HackerNewsItemUrl = "https://hacker-news.firebaseio.com/v0/item/{0}.json";
    }
}
