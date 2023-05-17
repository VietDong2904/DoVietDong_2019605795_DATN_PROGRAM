using Common.Model;
using System;
using System.Collections.Generic;

namespace Infrastructure.Persistence.Businesses.Account
{
    public class LoginModel
    {
        /// <summary>
        /// 
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool RememberMe { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// 
        /// </summary>
        public string TokenString { get; set; }

        public UserModel UserModel { get; set; }
    }

    public class UserModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Avatar { get; set; }
        public DateTime DateOfBirth { get; set; }
        public bool Sex { get; set; }
        public bool Status { get; set; }
        public string ListCartJson { get; set; }
        public List<string> ListRoles { get; set; }
        public List<Guid> ListRoleIds { get; set; }
    }

    public class UserFilterModel:RequestParameter
    {
        public string TextSearch { get; set; }
        public bool? Status { get; set; }
        public bool? IsGetAll { get; set; }
    }
    public class UserUpdatePasswordModel
    {
        public string Username { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
