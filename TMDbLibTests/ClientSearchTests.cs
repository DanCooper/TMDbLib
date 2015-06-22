﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Search;
using System.Linq;
using TMDbLib.Objects.TvShows;
using TMDbLibTests.Helpers;

namespace TMDbLibTests
{
    [TestClass]
    public class ClientSearchTests
    {
        private TestConfig _config;

        /// <summary>
        /// Run once, on every test
        /// </summary>
        [TestInitialize]
        public void Initiator()
        {
            _config = new TestConfig();
        }

        [TestMethod]
        public void TestSearchMovie()
        {
            TestHelpers.SearchPages(i => _config.Client.SearchMovie("007", i));

            // Search pr. Year
            // 1962: First James Bond movie, "Dr. No"
            SearchContainer<SearchMovie> result = _config.Client.SearchMovie("007", year: 1962);

            Assert.IsTrue(result.Results.Any());
            SearchMovie item = result.Results.SingleOrDefault(s => s.Id == 646);

            Assert.IsNotNull(item);
            Assert.AreEqual(646, item.Id);
            Assert.AreEqual(false, item.Adult);
            Assert.AreEqual("/bplDiT5JhaXf9S5arO8g5QsFtDi.jpg", item.BackdropPath);
            Assert.AreEqual("en", item.OriginalLanguage);
            Assert.AreEqual("Dr. No", item.OriginalTitle);
            Assert.AreEqual("When Strangways, the British SIS Station Chief in Jamaica goes missing, MI6 send James Bond - Agent 007 to investigate. His investigation leads him to the mysterious Crab Key; the secret base of Dr No who he suspects is trying to sabotage the American space program using a radio beam. With the assistance of local fisherman Quarrel, who had been helping Strangways, Bond sneaks onto Crab Key where he meets the beautiful Honey Ryder. Can the three of them defeat an army of henchmen and a \"fire breathing dragon\" in order to stop Dr No, save the space program and get revenge for Strangways? Dr. No is the first film of legendary James Bond series starring Sean Connery in the role of Fleming's British super agent.", item.Overview);
            Assert.AreEqual(false, item.Video);
            Assert.AreEqual("/gRdfLVVf6FheOw6mw6wOsKhZG1l.jpg", item.PosterPath);
            Assert.AreEqual(new DateTime(1962, 10, 5), item.ReleaseDate);
            Assert.AreEqual("Dr. No", item.Title);
            Assert.IsTrue(item.Popularity > 0);
            Assert.IsTrue(item.VoteAverage > 0);
            Assert.IsTrue(item.VoteCount > 0);

            Assert.IsNotNull(item.GenreIds);
            Assert.IsTrue(item.GenreIds.Contains(12));
            Assert.IsTrue(item.GenreIds.Contains(28));
            Assert.IsTrue(item.GenreIds.Contains(53));
        }

        [TestMethod]
        public void TestSearchCollection()
        {
            TestHelpers.SearchPages(i => _config.Client.SearchCollection("007", i));

            SearchContainer<SearchResultCollection> result = _config.Client.SearchCollection("James Bond");

            Assert.IsTrue(result.Results.Any());
            SearchResultCollection item = result.Results.SingleOrDefault(s => s.Id == 645);

            Assert.IsNotNull(item);
            Assert.AreEqual(645, item.Id);
            Assert.AreEqual("James Bond Collection", item.Name);
            Assert.AreEqual("/6VcVl48kNKvdXOZfJPdarlUGOsk.jpg", item.BackdropPath);
            Assert.AreEqual("/HORpg5CSkmeQlAolx3bKMrKgfi.jpg", item.PosterPath);
        }

        [TestMethod]
        public void TestSearchPerson()
        {
            TestHelpers.SearchPages(i => _config.Client.SearchPerson("Willis", i));

            SearchContainer<SearchPerson> result = _config.Client.SearchPerson("Willis");

            Assert.IsTrue(result.Results.Any());
            SearchPerson item = result.Results.SingleOrDefault(s => s.Id == 62);

            Assert.IsNotNull(item);
            Assert.AreEqual(62, item.Id);
            Assert.AreEqual("Bruce Willis", item.Name);
            Assert.AreEqual("/kI1OluWhLJk3pnR19VjOfABpnTY.jpg", item.ProfilePath);
            Assert.AreEqual(false, item.Adult);
            Assert.IsTrue(item.Popularity > 0);

            Assert.IsNotNull(item.KnownFor);
            Assert.IsTrue(item.KnownFor.Any(s => s.Id == 680));
        }

        [TestMethod]
        public void TestSearchList()
        {
            TestHelpers.SearchPages(i => _config.Client.SearchList("to watch", i));

            SearchContainer<SearchList> result = _config.Client.SearchList("to watch");

            Assert.IsTrue(result.Results.Any());
            SearchList item = result.Results.SingleOrDefault(s => s.Id == "54a5c0ceaed56c28c300013a");

            Assert.IsNotNull(item);
            Assert.AreEqual("to watch", item.Description);
            Assert.AreEqual("54a5c0ceaed56c28c300013a", item.Id);
            Assert.AreEqual("en", item.Iso_639_1);
            Assert.AreEqual("movie", item.ListType);
            Assert.AreEqual("Movies", item.Name);
            Assert.AreEqual("/w28byq1ITUk944o9jPcIrfDlywC.jpg", item.PosterPath);
            Assert.IsTrue(item.FavoriteCount > 0);
            Assert.IsTrue(item.ItemCount > 0);
        }

        [TestMethod]
        public void TestSearchCompany()
        {
            TestHelpers.SearchPages(i => _config.Client.SearchCompany("20th", i));

            SearchContainer<SearchCompany> result = _config.Client.SearchCompany("20th");

            Assert.IsTrue(result.Results.Any());
            SearchCompany item = result.Results.SingleOrDefault(s => s.Id == 25);

            Assert.IsNotNull(item);
            Assert.AreEqual(25, item.Id);
            Assert.AreEqual("/nM2MfoMqzJQRiSynsDabOtFKetD.png", item.LogoPath);
            Assert.AreEqual("20th Century Fox", item.Name);
        }

        [TestMethod]
        public void TestSearchKeyword()
        {
            TestHelpers.SearchPages(i => _config.Client.SearchKeyword("plot", i));

            SearchContainer<SearchKeyword> result = _config.Client.SearchKeyword("plot");

            Assert.IsTrue(result.Results.Any());
            SearchKeyword item = result.Results.SingleOrDefault(s => s.Id == 11121);

            Assert.IsNotNull(item);
            Assert.AreEqual(11121, item.Id);
            Assert.AreEqual("plot", item.Name);
        }

        [TestMethod]
        public void TestSearchTvShow()
        {
            TestHelpers.SearchPages(i => _config.Client.SearchTvShow("Breaking Bad", i));

            SearchContainer<SearchTv> result = _config.Client.SearchTvShow("Breaking Bad");

            Assert.IsTrue(result.Results.Any());
            SearchTv item = result.Results.SingleOrDefault(s => s.Id == 1396);

            Assert.IsNotNull(item);
            Assert.AreEqual(1396, item.Id);
            Assert.AreEqual("/eSzpy96DwBujGFj0xMbXBcGcfxX.jpg", item.BackdropPath);
            Assert.AreEqual(new DateTime(2008, 1, 19), item.FirstAirDate);
            Assert.AreEqual("Breaking Bad", item.Name);
            Assert.AreEqual("Breaking Bad", item.OriginalName);
            Assert.AreEqual("/4yMXf3DW6oCL0lVPZaZM2GypgwE.jpg", item.PosterPath);
            Assert.IsTrue(item.Popularity > 0);
            Assert.IsTrue(item.VoteAverage > 0);
            Assert.IsTrue(item.VoteCount > 0);
            Assert.IsNotNull(item.OriginCountry);
            Assert.AreEqual(1, item.OriginCountry.Count);
            Assert.AreEqual("US", item.OriginCountry[0]);
        }

        [TestMethod]
        public void TestSearchMulti()
        {
            TestHelpers.SearchPages(i => _config.Client.SearchMulti("Arrow", i));

            SearchContainer<SearchMulti> result = _config.Client.SearchMulti("Arrow");

            Assert.IsTrue(result.Results.Any());
            SearchMulti item = result.Results.SingleOrDefault(s => s.Id == 1412);

            Assert.IsNotNull(item);
            Assert.AreEqual(1412, item.Id);
            Assert.AreEqual("/dXTyVDTIgeByvUOUEiHjbi8xX9A.jpg", item.BackdropPath);
            Assert.AreEqual(new DateTime(2012, 10, 10), item.FirstAirDate);
            Assert.AreEqual(MediaType.TVShow, item.Type);
            Assert.AreEqual("Arrow", item.Name);
            Assert.AreEqual("Arrow", item.OriginalName);
            Assert.AreEqual("/mo0FP1GxOFZT4UDde7RFDz5APXF.jpg", item.PosterPath);
            Assert.IsTrue(item.Popularity > 0);
            Assert.IsTrue(item.VoteAverage > 0);
            Assert.IsTrue(item.VoteCount > 0);

            Assert.IsNotNull(item.OriginCountry);
            Assert.AreEqual(2, item.OriginCountry.Count);
            Assert.IsTrue(item.OriginCountry.Contains("US"));
            Assert.IsTrue(item.OriginCountry.Contains("CA"));
        }
    }
}
