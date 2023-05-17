using System;
using Domain.Common;

namespace Domain.Entities
{
   public class User:BaseTableEntity
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool IsSocialSign { get; set; }
        public string? Provider { get; set; }
        public string? Avatar { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool Sex { get; set; }
        public string ListCartJson { get; set; }
    }
}
