using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Lab.Accounting.API.Common.Helpers
{
    public class SendEmailHelper
    {
        private readonly EmailSettings _settings;

        public SendEmailHelper(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public async Task SendEmail(string userEmail, string verificationCode)
        {
            var client = new SendGridClient(_settings.ApiKey);

            var from = new EmailAddress(_settings.SenderEmail, _settings.SenderName);
            var to = new EmailAddress(userEmail);
            var subject = "忘記密碼 - 驗證碼";
            var htmlContent =
                $"<h1>忘記密碼驗證</h1><p>您的驗證碼是:<b>{verificationCode}</b></p><p>10 分鐘內有效。</p>";

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent: null, htmlContent);

            var response = await client.SendEmailAsync(msg);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Body.ReadAsStringAsync();
                throw new Exception($"SendGrid 寄信失敗: {response.StatusCode}, {body}");
            }
        }
    }
}
