using System.Threading.Tasks;

namespace Infrastructure.Shared.Services
{
    public interface IEmailService
    {
        Task SendAsync(EmailRequest request);
    }
}
