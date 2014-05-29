﻿using System;
using Newtonsoft.Json;
using TMDbLib.Converters;

namespace TMDbLib.Objects.General
{
    public class MovieResult
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("backdrop_path")]
        public string BackdropPath { get; set; }
        [JsonProperty("original_title")]
        public string OriginalTitle { get; set; }
        [JsonProperty("release_date")]
        [JsonConverter(typeof(DateTimeConverterYearMonthDay))]
        public DateTime? ReleaseDate { get; set; }
        [JsonProperty("poster_path")]
        public string PosterPath { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("popularity")]
        public double Popularity { get; set; }
        [JsonProperty("vote_average")]
        public double VoteAverage { get; set; }
        [JsonProperty("vote_count")]
        public int VoteCount { get; set; }
        [JsonProperty("adult")]
        public bool Adult { get; set; }
    }
}