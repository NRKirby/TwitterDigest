using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TwitterDigest.Functions.Models;

namespace TwitterDigest.Functions.Services
{

    public class TwitterApi
    {
        private readonly HttpClient _httpClient;

        public TwitterApi(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Tweet>> GetTweetsInLast24Hours(IEnumerable<string> users, DateTime date)
        {
            var tasks = users.Select(x => GetTweetsInLast24HoursFor(x, date)).ToList();
            await Task.WhenAll(tasks);
            var tweets = tasks.SelectMany(x => x.Result);
            return tweets;
        }

        private async Task<IEnumerable<Tweet>> GetTweetsInLast24HoursFor(string username, DateTime date)
        {
            var response = await _httpClient.GetAsync($"https://api.twitter.com/1.1/statuses/user_timeline.json?screen_name={username}");
            var json = await response.Content.ReadAsStringAsync();
            var twitterResponses = JsonConvert.DeserializeObject<List<TwitterResponse>>(json);
            var beginning = date.AddDays(-1);
            var end = date;
            return twitterResponses
                .Where(x => GetDateTime(x.created_at) >= beginning)
                .Where(x=> GetDateTime(x.created_at) < end)
                .Select(x => new Tweet
                {
                    TwitterHandle = x.user.screen_name,
                    Text = x.text,
                    DateTime = GetDateTime(x.created_at).ToString("ddd MMM dd HH:mm"),
                    Name = x.user.name,
                    ImageUrl = x.user.profile_image_url
                });
        }

        private static DateTime GetDateTime(string dateStr) =>
            DateTime.ParseExact(dateStr, "ddd MMM dd HH:mm:ss zzz yyyy", null);
    }
}
