using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using TwitterDigest.Functions;
using TwitterDigest.Functions.Repositories;
using TwitterDigest.Functions.Services;

[assembly: FunctionsStartup(typeof(Startup))]
namespace TwitterDigest.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var configuration = builder.GetContext().Configuration;

            builder.Services.AddTransient<TwitterApi>();
            builder.Services.AddHttpClient<TwitterApi>(client =>
            {
                var token = configuration["TwitterBearerToken"];
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            });
            builder.Services.AddTransient(x => new TwitterUserRepository(configuration["ConnectionString"], configuration["TableName"]));
            builder.Services.AddTransient<EmailService>();
        }
    }
}
