using System.Threading.Tasks;

namespace Infrastructure.Persistence.Businesses.Signalr
{
   public interface IHubClient
    {
        Task BroadcastMessage(MessageInstance msg);
    }
}
