using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TwitterDigest.Functions.Services;
using TwitterDigest.Tests.Infrastructure;
using Xunit;

namespace TwitterDigest.Tests.Services
{
    public class TwitterApiShould : TestBase
    {
        private readonly HttpClient _httpClient;

        public TwitterApiShould()
        {
            var json = TestContext.GetStringFromFile("TwitterResponse.json");
            var messageHandler = FakeHttpMessageHandler.GetHttpMessageHandler(
                json, HttpStatusCode.OK);
            _httpClient = new HttpClient(messageHandler);
        }

        [Fact]
        public async Task Return2Tweets()
        {
            var users = new List<string> { "nrkirby" };
            var date = new DateTime(2020, 12, 26, 21,00,00);
            var sut = new TwitterApi(_httpClient);

            var result = (await sut.GetTweetsInLast24Hours(users, date)).ToList();

            result.Count.ShouldBe(2);
            var tweet = result.First();
            tweet.Name.ShouldBe("Nick Kirby");
            tweet.TwitterHandle.ShouldBe("NRKirby");
        }

        [Fact]
        public async Task Return1Tweet()
        {
            var users = new List<string> { "nrkirby" };
            var date = new DateTime(2020, 12, 26, 20, 00, 00);
            var sut = new TwitterApi(_httpClient);

            var result = (await sut.GetTweetsInLast24Hours(users, date)).ToList();

            result.Count.ShouldBe(1);
        }

        [Fact]
        public async Task ReturnNoTweets()
        {
            var users = new List<string> { "nrkirby" };
            var date = new DateTime(2020, 12, 29);
            var sut = new TwitterApi(_httpClient);

            var result = (await sut.GetTweetsInLast24Hours(users, date)).ToList();

            result.Count.ShouldBe(0);
        }

        [Fact]
        public async Task ReturnTweetsWithCorrectProperties()
        {
            var users = new List<string> { "nrkirby" };
            var date = new DateTime(2020, 12, 26, 21, 00, 00);
            var sut = new TwitterApi(_httpClient);

            var result = (await sut.GetTweetsInLast24Hours(users, date)).ToList();

            var tweet = result.First();
            tweet.Name.ShouldBe("Nick Kirby");
            tweet.TwitterHandle.ShouldBe("NRKirby");
            tweet.DateTime.ShouldBe("Sat Dec 26 20:32");
            tweet.ImageUrl.ShouldBe("http://pbs.twimg.com/profile_images/1273898468935897089/6njc-lr3_normal.jpg");
        }
    }
}
