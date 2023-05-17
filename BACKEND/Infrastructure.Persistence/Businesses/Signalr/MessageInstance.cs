using System.Collections.Generic;
using Domain.Entities;

namespace Infrastructure.Persistence.Businesses.Signalr
{
    public class MessageInstance
    {
        public string Timestamp { get; set; }
        public string From { get; set; }
        public List<Notification> ListNotifications { get; set; }
    }
}
