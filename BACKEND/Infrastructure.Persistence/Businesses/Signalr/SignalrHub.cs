using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Persistence.Businesses.Signalr
{
    public class SignalrHub: Hub<IHubClient>
    {
        public async Task BroadcastMessage(MessageInstance msg)
        {
            await Clients.All.BroadcastMessage(msg);
        }
    }
}
