using System;
using System.Text.Json.Serialization;

namespace RestApi.Contracts.HackerNews
{
    public class HackerNewsItem
    {

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("by")]
        public string Author { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("time")]
        public double TimePublished { get; set; }

    }
}