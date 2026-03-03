using Lab.API.DI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lab.API.DI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        public interface IEmailNotifier
        {
            Task SendEmailAsync(string to, string title, string body);
        }

        public interface ISmsNotifier
        {
            Task SendSmsAsync(string phonenumber, string username);
        }

        public interface IMessageNotifier
        {
            Task SendMessageAsync(int userId, string username, string body);
        }

        private readonly IEmailNotifier _emailNotifier;

        private readonly IMessageNotifier _messageNotifier;

        public EmailController(IEmailNotifier notifier, IMessageNotifier messageNotifier)
        {
            _emailNotifier = notifier;
            _messageNotifier = messageNotifier;
        }

        public async Task ConfrimOrderAsync(Order order)
        {
            await _emailNotifier.SendEmailAsync(
                order.CustomerEmail,
                "訂單確認",
                $"訂購的商品{order.Name}已到達"
            );
        }
    }
}
