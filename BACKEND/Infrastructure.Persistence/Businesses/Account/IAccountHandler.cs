using System;
using Common.Common;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Businesses.Account
{
    public interface IAccountHandler
    {
        Task<Response> Create(UserModel model);
        Task<Response> Update(UserModel model);
        Task<Response> AuthenticateUser(LoginModel model);
        Task<Response> Filter(UserFilterModel model);
        /// <summary>
        /// Lấy User theo Id
        /// </summary>
        /// <param name="id">Id User</param>
        /// <returns>Thông tin User</returns>
        Task<Response> GetById(Guid id);
        /// <summary>
        /// Cập nhật mật khẩu
        /// </summary>
        /// <param name="model">Model cập nhật mật khẩu</param>
        /// <returns>Id người dùng</returns>
        Task<Response> UpdatePassword(UserUpdatePasswordModel model);

        /// <summary>
        /// Quên mật khẩu
        /// </summary>
        /// <param name="email"></param>
        /// <returns>Password User</returns>
        Task<Response> FogotPassword(string email);
    }
}
