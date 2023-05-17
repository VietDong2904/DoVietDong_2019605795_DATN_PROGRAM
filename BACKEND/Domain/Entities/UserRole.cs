using System;
using Domain.Common;

namespace Domain.Entities
{
   public class UserRole :BaseTableEntity
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
    }
}
