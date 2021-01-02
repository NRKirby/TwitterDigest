using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace TwitterDigest.Functions.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendDigest(object templateData)
        {
            var apiKey = _configuration["SendGridApiKey"];
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage
            {
                From = new EmailAddress("no_reply@twitterdigest.com", "Twitter Digest")
            };
            var email = _configuration["ToEmailAddress"];
            var name = _configuration["ToEmailName"];
            msg.AddTo(new EmailAddress(email, name));
            msg.SetTemplateData(templateData);
            var templateId = _configuration["SendGridTemplateId"];
            msg.SetTemplateId(templateId);

            var response = await client.SendEmailAsync(msg).ConfigureAwait(false);
        }
    }
}
