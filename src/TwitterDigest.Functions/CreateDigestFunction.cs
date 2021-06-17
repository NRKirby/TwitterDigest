using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using NodaTime;
using TwitterDigest.Functions.Models;
using TwitterDigest.Functions.Repositories;
using TwitterDigest.Functions.Services;

namespace TwitterDigest.Functions
{
    public class CreateDigestFunction
    {
        private readonly TwitterApi _twitterApi;
        private readonly TwitterUserRepository _twitterUserRepository;
        private readonly EmailService _emailService;
        private const string TwitterHandle = "nrkirby";

        public CreateDigestFunction(TwitterApi twitterApi,
            TwitterUserRepository twitterUserRepository, 
            EmailService emailService)
        {
            _twitterApi = twitterApi;
            _twitterUserRepository = twitterUserRepository;
            _emailService = emailService;
        }

        [FunctionName(nameof(CreateDigestFunction))] // 0 1 * * * *  run every hour at 1 minute past the hour
        public async Task Run([TimerTrigger("0 1 * * * *")] TimerInfo myTimer, ILogger log)
        {
            var now = SystemClock.Instance.GetCurrentInstant();
            var londonDateTimeZone = DateTimeZoneProviders.Tzdb["Europe/London"];
            var zonedDateTime = now.InZone(londonDateTimeZone);

            if (zonedDateTime.TimeOfDay.Hour != 7)
            {
                return;
            }

            var date = DateTime.Now;

            try
            {
                var twitterUsers = await _twitterUserRepository.List(TwitterHandle);

                var latestTweets = (await _twitterApi.GetTweetsInLast24Hours(twitterUsers.Select(x => x.RowKey), date)).ToList();

                await SendEmail(date, TwitterHandle, latestTweets);

                log.LogInformation($"C# Timer trigger function executed at: {date}");
            }
            catch (Exception e)
            {
                log.LogError(e, $"Exception thrown: {e.StackTrace}");
                throw;
            }
        }

        private async Task SendEmail(DateTime date, string twitterHandle, List<Tweet> latestTweets)
        {
            var templateData = new
            {
                Date = date.ToLongDateString(),
                TwitterHandle = twitterHandle,
                Tweets = latestTweets,
                TweetCount = latestTweets.Count
            };
            await _emailService.SendDigest(templateData);
        }
    }
}
