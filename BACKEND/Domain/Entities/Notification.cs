using System;

namespace Domain.Entities
{
   public class Notification
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public Guid UserId { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Description { get; set; }
    }
}
