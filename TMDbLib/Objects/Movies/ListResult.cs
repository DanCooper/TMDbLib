﻿using Newtonsoft.Json;

namespace TMDbLib.Objects.Movies
{
    public class ListResult
    {
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("favorite_count")]
        public int FavoriteCount { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("item_count")]
        public int ItemCount { get; set; }
        [JsonProperty("iso_639_1")]
        public string Iso_639_1 { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("poster_path")]
        public string PosterPath { get; set; }
    }
}