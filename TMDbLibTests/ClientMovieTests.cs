﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TMDbLib.Objects.Authentication;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Reviews;
using TMDbLibTests.Helpers;
using Cast = TMDbLib.Objects.Movies.Cast;
using Credits = TMDbLib.Objects.Movies.Credits;

namespace TMDbLibTests
{
    [TestClass]
    public class ClientMovieTests
    {
        private const int Avatar = 19995;
        private const int AGoodDayToDieHard = 47964;
        private const int TheDarkKnightRises = 49026;
        private const int MadMaxFuryRoad = 76341;
        private const string AGoodDayToDieHardImdb = "tt1606378";
        private const string TheDarkKnightRisesImdb = "tt1345836";

        private static Dictionary<MovieMethods, Func<Movie, object>> _methods;
        private TestConfig _config;

        /// <summary>
        /// Run once, on every test
        /// </summary>
        [TestInitialize]
        public void Initiator()
        {
            _config = new TestConfig();
        }

        /// <summary>
        /// Run once, on test class initialization
        /// </summary>
        [ClassInitialize]
        public static void InitialInitiator(TestContext context)
        {
            _methods = new Dictionary<MovieMethods, Func<Movie, object>>();
            _methods[MovieMethods.AlternativeTitles] = movie => movie.AlternativeTitles;
            _methods[MovieMethods.Credits] = movie => movie.Credits;
            _methods[MovieMethods.Images] = movie => movie.Images;
            _methods[MovieMethods.Keywords] = movie => movie.Keywords;
            _methods[MovieMethods.Releases] = movie => movie.Releases;
            _methods[MovieMethods.Videos] = movie => movie.Videos;
            _methods[MovieMethods.Translations] = movie => movie.Translations;
            _methods[MovieMethods.Similar] = movie => movie.Similar;
            _methods[MovieMethods.Reviews] = movie => movie.Reviews;
            _methods[MovieMethods.Lists] = movie => movie.Lists;
            _methods[MovieMethods.Changes] = movie => movie.Changes;
            _methods[MovieMethods.AccountStates] = movie => movie.AccountStates;
        }

        [TestMethod]
        public void TestMoviesExtrasNone()
        {
            Movie movie = _config.Client.GetMovie(AGoodDayToDieHard);

            Assert.IsNotNull(movie);

            // TODO: Test all properties
            Assert.AreEqual("A Good Day to Die Hard", movie.Title);

            // Test all extras, ensure none of them exist
            foreach (Func<Movie, object> selector in _methods.Values)
            {
                Assert.IsNull(selector(movie));
            }
        }

        [TestMethod]
        public void TestMoviesExtrasExclusive()
        {
            _config.Client.SetSessionInformation(_config.UserSessionId, SessionType.UserSession);
            TestMethodsHelper.TestGetExclusive(_methods, (id, extras) => _config.Client.GetMovie(id, extras), AGoodDayToDieHard);
        }

        [TestMethod]
        public void TestMoviesImdbExtrasAll()
        {
            Dictionary<MovieMethods, Func<Movie, object>> tmpMethods = new Dictionary<MovieMethods, Func<Movie, object>>(_methods);
            tmpMethods.Remove(MovieMethods.Videos);

            _config.Client.SetSessionInformation(_config.UserSessionId, SessionType.UserSession);

            MovieMethods combinedEnum = tmpMethods.Keys.Aggregate((methods, movieMethods) => methods | movieMethods);
            Movie item = _config.Client.GetMovie(TheDarkKnightRisesImdb, combinedEnum);

            TestMethodsHelper.TestAllNotNull(tmpMethods, item);
        }

        [TestMethod]
        public void TestMoviesExtrasAll()
        {
            _config.Client.SetSessionInformation(_config.UserSessionId, SessionType.UserSession);
            MovieMethods combinedEnum = _methods.Keys.Aggregate((methods, movieMethods) => methods | movieMethods);
            Movie item = _config.Client.GetMovie(AGoodDayToDieHard, combinedEnum);

            TestMethodsHelper.TestAllNotNull(_methods, item);
        }

        [TestMethod]
        public void TestMoviesLanguage()
        {
            Movie movie = _config.Client.GetMovie(AGoodDayToDieHard);
            Movie movieItalian = _config.Client.GetMovie(AGoodDayToDieHard, "it");

            Assert.IsNotNull(movie);
            Assert.IsNotNull(movieItalian);

            Assert.AreEqual("A Good Day to Die Hard", movie.Title);
            Assert.AreNotEqual(movie.Title, movieItalian.Title);

            // Test all extras, ensure none of them exist
            foreach (Func<Movie, object> selector in _methods.Values)
            {
                Assert.IsNull(selector(movie));
                Assert.IsNull(selector(movieItalian));
            }
        }

        [TestMethod]
        public void TestMoviesGetMovieAlternativeTitles()
        {
            //GetMovieAlternativeTitles(int id, string country)
            AlternativeTitles respUs = _config.Client.GetMovieAlternativeTitles(AGoodDayToDieHard, "US");
            Assert.IsNotNull(respUs);

            AlternativeTitles respFrench = _config.Client.GetMovieAlternativeTitles(AGoodDayToDieHard, "FR");
            Assert.IsNotNull(respFrench);

            Assert.IsFalse(respUs.Titles.Any(s => s.Title == "Duro de matar 5"));
            Assert.IsTrue(respFrench.Titles.Any(s => s.Title == "Die Hard 5 - Belle Journée Pour mourir"));

            Assert.IsTrue(respUs.Titles.All(s => s.Iso_3166_1 == "US"));
            Assert.IsTrue(respFrench.Titles.All(s => s.Iso_3166_1 == "FR"));
        }

        [TestMethod]
        public void TestMoviesGetMovieAlternativeTitlesCountry()
        {
            AlternativeTitles respUs = _config.Client.GetMovieAlternativeTitles(AGoodDayToDieHard, "US");
            Assert.IsNotNull(respUs);

            _config.Client.DefaultCountry = "US";

            AlternativeTitles respUs2 = _config.Client.GetMovieAlternativeTitles(AGoodDayToDieHard);
            Assert.IsNotNull(respUs2);

            Assert.AreEqual(respUs.Titles.Count, respUs2.Titles.Count);
        }

        [TestMethod]
        public void TestMoviesGetMovieCasts()
        {
            Credits resp = _config.Client.GetMovieCredits(AGoodDayToDieHard);
            Assert.IsNotNull(resp);

            Cast cast = resp.Cast.SingleOrDefault(s => s.Name == "Bruce Willis");
            Assert.IsNotNull(cast);

            Assert.AreEqual(1, cast.CastId);
            Assert.AreEqual("John McClane", cast.Character);
            Assert.AreEqual("52fe4751c3a36847f812f049", cast.CreditId);
            Assert.AreEqual(62, cast.Id);
            Assert.AreEqual("Bruce Willis", cast.Name);
            Assert.AreEqual(0, cast.Order);
            Assert.AreEqual("/kI1OluWhLJk3pnR19VjOfABpnTY.jpg", cast.ProfilePath);

            Crew crew = resp.Crew.SingleOrDefault(s => s.Name == "Marco Beltrami");
            Assert.IsNotNull(crew);

            Assert.AreEqual("5336b0e09251417d9b000cc7", crew.CreditId);
            Assert.AreEqual("Sound", crew.Department);
            Assert.AreEqual(7229, crew.Id);
            Assert.AreEqual("Music", crew.Job);
            Assert.AreEqual("Marco Beltrami", crew.Name);
            Assert.AreEqual("/AvmWWykshwdR9sUWdbI8DaRLpRs.jpg", crew.ProfilePath);

        }

        [TestMethod]
        public void TestMoviesGetMovieImages()
        {
            ImagesWithId resp = _config.Client.GetMovieImages(AGoodDayToDieHard);
            Assert.IsNotNull(resp);

            ImageData backdrop = resp.Backdrops.SingleOrDefault(s => s.FilePath == "/17zArExB7ztm6fjUXZwQWgGMC9f.jpg");
            Assert.IsNotNull(backdrop);

            Assert.IsTrue(Math.Abs(1.77777777777778 - backdrop.AspectRatio) < double.Epsilon);
            Assert.AreEqual("/17zArExB7ztm6fjUXZwQWgGMC9f.jpg", backdrop.FilePath);
            Assert.AreEqual(1080, backdrop.Height);
            Assert.AreEqual("xx", backdrop.Iso_639_1);
            Assert.IsTrue(backdrop.VoteAverage > 0);
            Assert.IsTrue(backdrop.VoteCount > 0);
            Assert.AreEqual(1920, backdrop.Width);

            ImageData poster = resp.Posters.SingleOrDefault(s => s.FilePath == "/c2SQMd00CCGTiDxGXVqA2J9lmzF.jpg");
            Assert.IsNotNull(poster);

            Assert.IsTrue(Math.Abs(0.666666666666667 - poster.AspectRatio) < double.Epsilon);
            Assert.AreEqual("/c2SQMd00CCGTiDxGXVqA2J9lmzF.jpg", poster.FilePath);
            Assert.AreEqual(1500, poster.Height);
            Assert.AreEqual("en", poster.Iso_639_1);
            Assert.IsTrue(poster.VoteAverage > 0);
            Assert.IsTrue(poster.VoteCount > 0);
            Assert.AreEqual(1000, poster.Width);
        }

        [TestMethod]
        public void TestMoviesGetMovieKeywords()
        {
            KeywordsContainer resp = _config.Client.GetMovieKeywords(AGoodDayToDieHard);
            Assert.IsNotNull(resp);

            Keyword keyword = resp.Keywords.SingleOrDefault(s => s.Id == 186447);
            Assert.IsNotNull(keyword);

            Assert.AreEqual(186447, keyword.Id);
            Assert.AreEqual("rogue", keyword.Name);
        }

        [TestMethod]
        public void TestMoviesGetMovieReleases()
        {
            Releases resp = _config.Client.GetMovieReleases(AGoodDayToDieHard);
            Assert.IsNotNull(resp);

            Country country = resp.Countries.SingleOrDefault(s => s.Iso_3166_1 == "US");
            Assert.IsNotNull(country);

            Assert.AreEqual("R", country.Certification);
            Assert.AreEqual("US", country.Iso_3166_1);
            Assert.AreEqual(true, country.Primary);
            Assert.AreEqual(new DateTime(2013, 2, 14), country.ReleaseDate);
        }

        [TestMethod]
        public void TestMoviesGetMovieVideos()
        {
            ResultContainer<Video> resp = _config.Client.GetMovieVideos(AGoodDayToDieHard);
            Assert.IsNotNull(resp);

            Assert.IsNotNull(resp);
            Assert.IsNotNull(resp.Results);

            Video video = resp.Results[0];
            Assert.IsNotNull(video);

            Assert.AreEqual("533ec6a7c3a368544800556f", video.Id);
            Assert.AreEqual("en", video.Iso_639_1);
            Assert.AreEqual("7EgVRvG2mM0", video.Key);
            Assert.AreEqual("A Good Day To Die Hard Official Trailer", video.Name);
            Assert.AreEqual("YouTube", video.Site);
            Assert.AreEqual(720, video.Size);
            Assert.AreEqual("Trailer", video.Type);
        }

        [TestMethod]
        public void TestMoviesGetMovieTranslations()
        {
            TranslationsContainer resp = _config.Client.GetMovieTranslations(AGoodDayToDieHard);
            Assert.IsNotNull(resp);

            Translation translation = resp.Translations.SingleOrDefault(s => s.EnglishName == "German");
            Assert.IsNotNull(translation);

            Assert.AreEqual("German", translation.EnglishName);
            Assert.AreEqual("de", translation.Iso_639_1);
            Assert.AreEqual("Deutsch", translation.Name);
        }

        [TestMethod]
        public void TestMoviesGetMovieSimilarMovies()
        {
            SearchContainer<MovieResult> resp = _config.Client.GetMovieSimilar(AGoodDayToDieHard);
            Assert.IsNotNull(resp);

            SearchContainer<MovieResult> respGerman = _config.Client.GetMovieSimilar(AGoodDayToDieHard, language: "de");
            Assert.IsNotNull(respGerman);

            Assert.AreEqual(resp.Results.Count, respGerman.Results.Count);

            int differentTitles = 0;
            for (int i = 0; i < resp.Results.Count; i++)
            {
                Assert.AreEqual(resp.Results[i].Id, respGerman.Results[i].Id);

                // At least one title should be different, as German is a big language and they dub all their titles.
                differentTitles++;
            }

            Assert.IsTrue(differentTitles > 0);
        }

        [TestMethod]
        public void TestMoviesGetMovieReviews()
        {
            SearchContainer<Review> resp = _config.Client.GetMovieReviews(TheDarkKnightRises);
            Assert.IsNotNull(resp);

            Assert.AreNotEqual(0, resp.Results.Count);
            Assert.IsNotNull(resp.Results[0].Content);
        }

        [TestMethod]
        public void TestMoviesGetMovieLists()
        {
            //GetMovieLists(int id, string language, int page = -1)
            SearchContainer<ListResult> resp = _config.Client.GetMovieLists(AGoodDayToDieHard);
            Assert.IsNotNull(resp);

            SearchContainer<ListResult> respPage2 = _config.Client.GetMovieLists(AGoodDayToDieHard, 2);
            Assert.IsNotNull(respPage2);

            Assert.AreEqual(1, resp.Page);
            Assert.AreEqual(2, respPage2.Page);
            Assert.AreEqual(resp.TotalResults, resp.TotalResults);
        }

        [TestMethod]
        public void TestMoviesGetMovieChanges()
        {
            //GetMovieChanges(int id, DateTime? startDate = null, DateTime? endDate = null)
            // Find latest changed title
            int latestChanged = _config.Client.GetMovieLatest().Id;

            // Fetch changelog
            DateTime lower = DateTime.UtcNow.AddDays(-13);
            DateTime higher = DateTime.UtcNow.AddDays(1);
            List<Change> respRange = _config.Client.GetMovieChanges(latestChanged, lower, higher);

            Assert.IsNotNull(respRange);
            Assert.IsTrue(respRange.Count > 0);

            // As TMDb works in days, we need to adjust our values also
            lower = lower.AddDays(-1);
            higher = higher.AddDays(1);

            foreach (Change change in respRange)
                foreach (ChangeItem changeItem in change.Items)
                {
                    DateTime date = changeItem.Time;
                    Assert.IsTrue(lower <= date);
                    Assert.IsTrue(date <= higher);
                }
        }

        [TestMethod]
        public void TestMoviesImages()
        {
            // Get config
            _config.Client.GetConfig();

            // Test image url generator
            ImagesWithId images = _config.Client.GetMovieImages(AGoodDayToDieHard);

            Assert.AreEqual(AGoodDayToDieHard, images.Id);
            TestImagesHelpers.TestImages(_config, images);
        }

        [TestMethod]
        public void TestMoviesList()
        {
            //GetMovieList(MovieListType type, string language, int page = -1)
            foreach (MovieListType type in Enum.GetValues(typeof(MovieListType)).OfType<MovieListType>())
            {
                SearchContainer<MovieResult> list = _config.Client.GetMovieList(type);

                Assert.IsNotNull(list);
                Assert.IsTrue(list.Results.Count > 0);
                Assert.AreEqual(1, list.Page);

                SearchContainer<MovieResult> listPage2 = _config.Client.GetMovieList(type, 2);

                Assert.IsNotNull(listPage2);
                Assert.IsTrue(listPage2.Results.Count > 0);
                Assert.AreEqual(2, listPage2.Page);

                SearchContainer<MovieResult> listDe = _config.Client.GetMovieList(type, "de");

                Assert.IsNotNull(listDe);
                Assert.IsTrue(listDe.Results.Count > 0);
                Assert.AreEqual(1, listDe.Page);

                // At least one title should differ
                Assert.IsTrue(list.Results.Any(s => listDe.Results.Any(x => x.Title != s.Title)));
            }
        }

        [TestMethod]
        public void TestMoviesAccountStateFavoriteSet()
        {
            _config.Client.SetSessionInformation(_config.UserSessionId, SessionType.UserSession);
            AccountState accountState = _config.Client.GetMovieAccountState(MadMaxFuryRoad);

            // Remove the favourite
            if (accountState.Favorite)
                _config.Client.AccountChangeFavoriteStatus(MediaType.Movie, MadMaxFuryRoad, false);

            // Allow TMDb to cache our changes
            Thread.Sleep(2000);

            // Test that the movie is NOT favourited
            accountState = _config.Client.GetMovieAccountState(MadMaxFuryRoad);

            Assert.AreEqual(MadMaxFuryRoad, accountState.Id);
            Assert.IsFalse(accountState.Favorite);

            // Favourite the movie
            _config.Client.AccountChangeFavoriteStatus(MediaType.Movie, MadMaxFuryRoad, true);

            // Allow TMDb to cache our changes
            Thread.Sleep(2000);

            // Test that the movie IS favourited
            accountState = _config.Client.GetMovieAccountState(MadMaxFuryRoad);
            Assert.AreEqual(MadMaxFuryRoad, accountState.Id);
            Assert.IsTrue(accountState.Favorite);
        }

        [TestMethod]
        public void TestMoviesAccountStateWatchlistSet()
        {
            _config.Client.SetSessionInformation(_config.UserSessionId, SessionType.UserSession);
            AccountState accountState = _config.Client.GetMovieAccountState(MadMaxFuryRoad);

            // Remove the watchlist
            if (accountState.Watchlist)
                _config.Client.AccountChangeWatchlistStatus(MediaType.Movie, MadMaxFuryRoad, false);

            // Allow TMDb to cache our changes
            Thread.Sleep(2000);

            // Test that the movie is NOT watchlisted
            accountState = _config.Client.GetMovieAccountState(MadMaxFuryRoad);

            Assert.AreEqual(MadMaxFuryRoad, accountState.Id);
            Assert.IsFalse(accountState.Watchlist);

            // Watchlist the movie
            _config.Client.AccountChangeWatchlistStatus(MediaType.Movie, MadMaxFuryRoad, true);

            // Allow TMDb to cache our changes
            Thread.Sleep(2000);

            // Test that the movie IS watchlisted
            accountState = _config.Client.GetMovieAccountState(MadMaxFuryRoad);
            Assert.AreEqual(MadMaxFuryRoad, accountState.Id);
            Assert.IsTrue(accountState.Watchlist);
        }

        [TestMethod]
        public void TestMoviesAccountStateRatingSet()
        {
            _config.Client.SetSessionInformation(_config.UserSessionId, SessionType.UserSession);
            AccountState accountState = _config.Client.GetMovieAccountState(MadMaxFuryRoad);

            Assert.Inconclusive("Alter when TMDb has the option to remove ratings");

            // Remove the rating
            if (accountState.Rating.HasValue)
                // TODO: Alter when TMDb has the option to remove ratings
                _config.Client.MovieSetRating(MadMaxFuryRoad, 0);

            // Allow TMDb to cache our changes
            Thread.Sleep(2000);

            // Test that the movie is NOT rated
            accountState = _config.Client.GetMovieAccountState(MadMaxFuryRoad);

            Assert.AreEqual(MadMaxFuryRoad, accountState.Id);
            Assert.IsFalse(accountState.Rating.HasValue);

            // Rate the movie
            _config.Client.MovieSetRating( MadMaxFuryRoad, 5);

            // Allow TMDb to cache our changes
            Thread.Sleep(2000);

            // Test that the movie IS rated
            accountState = _config.Client.GetMovieAccountState(MadMaxFuryRoad);
            Assert.AreEqual(MadMaxFuryRoad, accountState.Id);
            Assert.IsTrue(accountState.Rating.HasValue);
        }

        [TestMethod]
        public void TestMoviesSetRatingBadRating()
        {
            _config.Client.SetSessionInformation(_config.UserSessionId, SessionType.UserSession);
            Assert.IsFalse(_config.Client.MovieSetRating(Avatar, 7.1));
        }

        [TestMethod]
        public void TestMoviesSetRatingRatingOutOfBounds()
        {
            _config.Client.SetSessionInformation(_config.UserSessionId, SessionType.UserSession);
            Assert.IsFalse(_config.Client.MovieSetRating(Avatar, 10.5));
        }

        [TestMethod]
        public void TestMoviesSetRatingRatingLowerBoundsTest()
        {
            _config.Client.SetSessionInformation(_config.UserSessionId, SessionType.UserSession);
            Assert.IsFalse(_config.Client.MovieSetRating(Avatar, 0));
        }

        [TestMethod]
        public void TestMoviesSetRatingUserSession()
        {
            _config.Client.SetSessionInformation(_config.UserSessionId, SessionType.UserSession);

            // Ensure that the test movie has a different rating than our test rating
            var rating = _config.Client.GetMovieAccountState(Avatar).Rating;
            Assert.IsNotNull(rating);

            double originalRating = rating.Value;
            double newRating = Math.Abs(originalRating - 7.5) < double.Epsilon ? 2.5 : 7.5;

            // Try changing the rating
            Assert.IsTrue(_config.Client.MovieSetRating(Avatar, newRating));

            // Allow TMDb to not cache our data
            Thread.Sleep(2000);

            // Check if it worked
            Assert.AreEqual(newRating, _config.Client.GetMovieAccountState(Avatar).Rating);

            // Try changing it back to the previous rating
            Assert.IsTrue(_config.Client.MovieSetRating(Avatar, originalRating));

            // Allow TMDb to not cache our data
            Thread.Sleep(2000);

            // Check if it worked
            Assert.AreEqual(originalRating, _config.Client.GetMovieAccountState(Avatar).Rating);
        }

        [TestMethod]
        public void TestMoviesSetRatingGuestSession()
        {
            // There is no way to validate the change besides the success return of the api call since the guest session doesn't have access to anything else
            _config.Client.SetSessionInformation(_config.GuestTestSessionId, SessionType.GuestSession);
            // Try changing the rating
            Assert.IsTrue(_config.Client.MovieSetRating(Avatar, 7.5));

            // Try changing it back to the previous rating
            Assert.IsTrue(_config.Client.MovieSetRating(Avatar, 8));
        }

        [TestMethod]
        public void TestMoviesGet()
        {
            Movie item = _config.Client.GetMovie(AGoodDayToDieHard);

            Assert.IsNotNull(item);
            Assert.AreEqual(AGoodDayToDieHard, item.Id);
            Assert.AreEqual(AGoodDayToDieHardImdb, item.ImdbId);

            // Check all properties
            Assert.AreEqual("A Good Day to Die Hard", item.Title);
            Assert.AreEqual("A Good Day to Die Hard", item.OriginalTitle);
            Assert.AreEqual("en", item.OriginalLanguage);

            Assert.AreEqual("Released", item.Status);
            Assert.AreEqual("Yippee Ki-Yay Mother Russia", item.Tagline);
            Assert.AreEqual("Iconoclastic, take-no-prisoners cop John McClane, finds himself for the first time on foreign soil after traveling to Moscow to help his wayward son Jack - unaware that Jack is really a highly-trained CIA operative out to stop a nuclear weapons heist. With the Russian underworld in pursuit, and battling a countdown to war, the two McClanes discover that their opposing methods make them unstoppable heroes.", item.Overview);
            Assert.AreEqual("http://www.diehardmovie.com/", item.Homepage);

            Assert.AreEqual("/17zArExB7ztm6fjUXZwQWgGMC9f.jpg", item.BackdropPath);
            Assert.AreEqual("/c2SQMd00CCGTiDxGXVqA2J9lmzF.jpg", item.PosterPath);

            Assert.AreEqual(false, item.Adult);
            Assert.AreEqual(false, item.Video);

            Assert.AreEqual(1, item.BelongsToCollection.Count);
            Assert.AreEqual(1570, item.BelongsToCollection[0].Id);
            Assert.AreEqual("Die Hard Collection", item.BelongsToCollection[0].Name);
            Assert.AreEqual("/5kHVblr87FUScuab1PVSsK692IL.jpg", item.BelongsToCollection[0].BackdropPath);
            Assert.AreEqual("/dQP1lu4tBtCiAMeCRcuTFpJiM7y.jpg", item.BelongsToCollection[0].PosterPath);

            Assert.AreEqual(2, item.Genres.Count);
            Assert.AreEqual(28, item.Genres[0].Id);
            Assert.AreEqual("Action", item.Genres[0].Name);
            Assert.AreEqual(53, item.Genres[1].Id);
            Assert.AreEqual("Thriller", item.Genres[1].Name);

            Assert.AreEqual(new DateTime(2013, 02, 14), item.ReleaseDate);
            Assert.AreEqual(304654182, item.Revenue);
            Assert.AreEqual(92000000, item.Budget);
            Assert.AreEqual(98, item.Runtime);
        }
    }
}
