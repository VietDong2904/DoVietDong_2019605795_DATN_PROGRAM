using Common.Common;
using Domain.Settings;
using Infrastructure.Persistence.Businesses.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Serilog;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    public class AccountController : ApiBaseController
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IAccountHandler _accountHandler;
        public AccountController(IOptions<JwtSettings> jwtSettings, IAccountHandler accountHandler)
        {
            _accountHandler = accountHandler;
            _jwtSettings = jwtSettings.Value;
        }
        /// <summary>
        /// Đăng nhập và trả về token
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel login)
        {
            try
            {
                var user = await _accountHandler.AuthenticateUser(login);

                if (user.Code == Code.Success && user is ResponseObject<UserModel> userData)
                {
                    var tokenString = GenerateJsonWebToken(userData.Data);
                    Log.Information($"Login successful: {JsonConvert.SerializeObject(login)}");
                    var result = new LoginResponse()
                    {
                        TokenString = tokenString,
                        UserModel = userData.Data
                    };
                    return Ok(new ResponseObject<LoginResponse>(result, userData.Message, Code.Success));
                }
                Log.Error($"Login failed: {JsonConvert.SerializeObject(login)}");
                return Ok(new ResponseObject<LoginResponse>(null, user.Message, Code.Unauthorized));
            }
            catch (Exception ex)
            {
                Log.Error($"Login failed: {JsonConvert.SerializeObject(ex)}");
                return Ok(new ResponseError(Code.ServerError, "Login failed"));
            }
        }
        /// <summary>
        /// Tạo mới tài khoản
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] UserModel model)
        {
            return Ok(await _accountHandler.Create(model));
        }
        [Authorize]
        [HttpPost]
        [Route("filter")]
        public async Task<IActionResult> Filter([FromBody] UserFilterModel model)
        {
            return Ok(await _accountHandler.Filter(model));
        }
        [Authorize]
        [HttpPut]
        [Route("")]
        public async Task<IActionResult> Update([FromBody] UserModel model)
        {
            return Ok(await _accountHandler.Update(model));
        }
        [Authorize]
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Detail([FromQuery] Guid id)
        {
            return Ok(await _accountHandler.GetById(id));
        }
        /// <summary>
        /// Thay đổi mật khẩu người dùng
        /// </summary> 
        /// <param name="model">Thông tin người dùng cần cập nhật mật khẩu</param>
        /// <returns>Id người dùng đã cập nhật thành công</returns> 
        /// <response code="200">Thành công</response>
        [Authorize, HttpPut, Route("update-password")]
        public async Task<IActionResult> UpdatePassword([FromBody] UserUpdatePasswordModel model)
        {
            var result = await _accountHandler.UpdatePassword(model);

            return Ok(result);
        }

        /// <summary>
        /// Lấy lại mật khẩu người dùng
        /// </summary> 
        /// <param name="email">Thông tin người dùng cần cập nhật mật khẩu</param>
        /// <returns>Id người dùng đã cập nhật thành công</returns> 
        /// <response code="200">Thành công</response>
        [AllowAnonymous, HttpGet, Route("forgot-password")]
        public async Task<Response> ForgotPassword([FromQuery] string email)
        {
            var result = await _accountHandler.FogotPassword(email);

            return result;
        }
        private string GenerateJsonWebToken(UserModel user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[] {
                new Claim("UserId", user.Id.ToString()),
                new Claim("Username", user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var token = new JwtSecurityToken(_jwtSettings.Issuer,
                _jwtSettings.Audience,
                claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
