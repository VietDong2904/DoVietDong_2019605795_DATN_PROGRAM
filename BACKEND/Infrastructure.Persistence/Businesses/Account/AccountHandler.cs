using Application.DTOs.Email;
using Common.Common;
using Common.Constants;
using Common.Helpers;
using Domain.Entities;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Shared.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmailRequest = Infrastructure.Shared.Services.EmailRequest;

namespace Infrastructure.Persistence.Businesses.Account
{
    public class AccountHandler : IAccountHandler
    {
        private readonly string _frontendUrl = Helpers.GetConfig("Url:Frontend");
        private readonly ApplicationDbContext _dataContext;
        private readonly Random _random = new Random();
        private readonly IEmailService _emailService;

        public AccountHandler(ApplicationDbContext dataContext, IEmailService emailService)
        {
            _emailService = emailService;
            _dataContext = dataContext;
        }

        public async Task<Response> Update(UserModel model)
        {
            try
            {
                var oldEntity = _dataContext.Users.FirstOrDefault(x => x.Id == model.Id);
                if (oldEntity == null)
                {
                    return new ResponseError(Code.ServerError, MessageConstants.UpdateErrorMessage);
                }
                oldEntity.Email = model.Email;
                oldEntity.Name = model.Name;
                oldEntity.Avatar = model.Avatar;
                oldEntity.Phone = model.Phone;
                oldEntity.DateOfBirth = model.DateOfBirth;
                oldEntity.Status = model.Status;
                #region Check Email Unique
                var ckMail = await _dataContext.Users.AnyAsync(x => x.Email.Equals(oldEntity.Email) && x.Id != oldEntity.Id);
                if (ckMail)
                {
                    return new ResponseError(Code.BadRequest, "Email đã tồn tại");
                }
                #endregion

                _dataContext.Users.Update(oldEntity);
                var dbSave = await _dataContext.SaveChangesAsync();
                //Remove Old Role
                _dataContext.UserRoles.RemoveRange(_dataContext.UserRoles.Where(x => x.UserId == oldEntity.Id));
                await _dataContext.SaveChangesAsync();
                if (model.ListRoleIds != null && model.ListRoleIds.Count > 0)
                {
                    var listUserRole = new List<UserRole>();
                    foreach (var item in model.ListRoleIds)
                    {
                        var role = new UserRole()
                        {
                            Id = Guid.NewGuid(),
                            UserId = oldEntity.Id,
                            RoleId = item
                        };
                        listUserRole.Add(role);
                    }
                    await _dataContext.UserRoles.AddRangeAsync(listUserRole);
                }
                await _dataContext.SaveChangesAsync();

                if (dbSave > 0)
                {
                    return new ResponseObject<UserModel>(model, MessageConstants.UpdateSuccessMessage, Code.Success);
                }
                return new ResponseError(Code.ServerError, MessageConstants.UpdateErrorMessage);
            }
            catch (Exception ex)
            {
                Log.Error(ex, MessageConstants.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{MessageConstants.UpdateErrorMessage} - {ex.Message}");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public async Task<Response> AuthenticateUser(LoginModel login)
        {
            var user = await _dataContext.Users.FirstOrDefaultAsync(x => x.Username.Trim().Equals(login.Username.Trim()));
            if (user != null)
            {
                var userModel = AutoMapperUtils.AutoMap<User, UserModel>(user);
                var passhash = Helpers.PasswordGenerateHmac(login.Password, user.PasswordSalt);
                if (passhash == user.Password)
                {
                    userModel.ListRoles = (from a in _dataContext.Roles join b in _dataContext.UserRoles on a.Id equals b.RoleId where b.UserId == userModel.Id select a.Code).ToList();
                    return new ResponseObject<UserModel>(userModel, "Đăng nhập thành công", Code.Success);
                }
                return new ResponseError(Code.BadRequest, "Sai mật khẩu");
            }

            return new ResponseError(Code.BadRequest, "Không tìm thấy tài khoản");
        }

        public async Task<Response> Filter(UserFilterModel filter)
        {
            try
            {
                var data = (from c in _dataContext.Users

                            select new UserModel()
                            {
                                Id = c.Id,
                                Name = c.Name,
                                Username = c.Username,
                                Password = c.Password,
                                PasswordSalt = c.PasswordSalt,
                                Email = c.Email,
                                ListRoles = (from a in _dataContext.UserRoles join b in _dataContext.Roles on a.RoleId equals b.Id where a.UserId == c.Id select b.Code).ToList(),
                                ListRoleIds = (from a in _dataContext.UserRoles join b in _dataContext.Roles on a.RoleId equals b.Id where a.UserId == c.Id select b.Id).ToList(),
                                Phone = c.Phone,
                                Status = c.Status,
                                Avatar = c.Avatar,
                                DateOfBirth = c.DateOfBirth,
                                Sex = c.Sex
                            });

                if (!string.IsNullOrEmpty(filter.TextSearch))
                {
                    string ts = filter.TextSearch.Trim().ToLower();
                    data = data.Where(x => x.Name.ToLower().Contains(ts) || x.Username.ToLower().Contains(ts) || x.Email.ToLower().Contains(ts) || x.Phone.ToLower().Contains(ts));
                }
                if (filter.Status.HasValue)
                {
                    data = data.Where(x => x.Status == filter.Status);
                }
                int totalCount = data.Count();
                // Pagination
                if (filter.IsGetAll.HasValue && !filter.IsGetAll.Value)
                {
                    //Calculate nunber of rows to skip on pagesize
                    int excludedRows = (filter.PageNumber - 1) * (filter.PageSize);
                    if (excludedRows <= 0)
                    {
                        excludedRows = 0;
                    }
                    // Query
                    data = data.Skip(excludedRows).Take(filter.PageSize);
                }
                int dataCount = data.Count();

                var listResult = await data.ToListAsync();
                return new ResponseObject<PaginationList<UserModel>>(new PaginationList<UserModel>()
                {
                    DataCount = dataCount,
                    TotalCount = totalCount,
                    PageNumber = filter.PageNumber,
                    PageSize = filter.PageSize,
                    Data = listResult
                }, MessageConstants.GetDataSuccessMessage, Code.Success);
            }
            catch (Exception ex)
            {
                Log.Error(ex, MessageConstants.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{MessageConstants.GetDataErrorMessage} - {ex.Message}");
            }
        }

        public async Task<Response> GetById(Guid id)
        {
            try
            {
                var entity = await _dataContext.Users
                    .FirstOrDefaultAsync(x => x.Id == id);
                var rs = AutoMapperUtils.AutoMap<User, UserModel>(entity);
                rs.ListRoles = (from a in _dataContext.UserRoles
                    join b in _dataContext.Roles on a.RoleId equals b.Id
                    where a.UserId == rs.Id
                    select b.Code).ToList();                
                rs.ListRoleIds = (from a in _dataContext.UserRoles
                    join b in _dataContext.Roles on a.RoleId equals b.Id
                    where a.UserId == rs.Id
                    select b.Id).ToList();
                return new ResponseObject<UserModel>(rs, MessageConstants.GetDataSuccessMessage, Code.Success);
            }
            catch (Exception ex)
            {
                Log.Error(ex, MessageConstants.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{MessageConstants.GetDataErrorMessage} - {ex.Message}");
            }
        }

        public async Task<Response> UpdatePassword(UserUpdatePasswordModel model)
        {
            try
            {
                var entity = await _dataContext.Users
                    .FirstOrDefaultAsync(x => x.Username == model.Username);
                //Log.Information("Before Update Password: " + JsonSerializer.Serialize(entity));

                if (!entity.Password.Equals(Helpers.PasswordGenerateHmac(model.OldPassword, entity.PasswordSalt)))
                {
                    return new ResponseObject<Guid>(Guid.Empty, "Mật khẩu cũ không chính xác", Code.ServerError); ;
                }

                entity.PasswordSalt = Helpers.PassowrdCreateSalt512();
                entity.Password = Helpers.PasswordGenerateHmac(model.NewPassword, entity.PasswordSalt);
                _dataContext.Users.Update(entity);

                int dbSave = await _dataContext.SaveChangesAsync();
                if (dbSave > 0)
                {
                    return new ResponseObject<string>(model.Username, MessageConstants.UpdateSuccessMessage, Code.Success);
                }
                return new ResponseError(Code.ServerError, MessageConstants.UpdateErrorMessage);
            }
            catch (Exception ex)
            {
                Log.Error(ex, MessageConstants.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{MessageConstants.GetDataErrorMessage} - {ex.Message}");
            }
        }

        public async Task<Response> FogotPassword(string email)
        {
            try
            {
                var userModel = _dataContext.Users.FirstOrDefault(x => x.Email.Equals(email));
                if (userModel == null)
                {
                    return new ResponseObject<string>(null, "Email không tồn tại trong hệ thống!", Code.ServerError);
                }
                userModel.PasswordSalt = Helpers.PassowrdCreateSalt512();
                var newPassword = RandomPassword();
                userModel.Password = Helpers.PasswordGenerateHmac(newPassword, userModel.PasswordSalt);
                _dataContext.Users.Update(userModel);
                var dbSave = await _dataContext.SaveChangesAsync();
                if (dbSave >= 1)
                {
                    #region Gửi email thông báo đến khách hàng

                    string title = "[Molla - Mobile] - Lấy lại mật khẩu thành công";


                    StringBuilder htmlBody = new StringBuilder();
                    htmlBody.Append("<html><body>");
                    htmlBody.Append("<p>Xin chào <b>" + userModel.Name + "</b>,</p>");
                    htmlBody.Append("<p>Bạn đã lấy lại mật khẩu thành công. Bạn hãy truy cập vào trang web để thay đổi mật khẩu.</p>");
                    htmlBody.Append("<p>Mật khẩu mới của bạn là: <b>" + newPassword + "</b></p>");
                    htmlBody.Append("<p><a href='" + _frontendUrl + "'><span class='fas fa - laptop'></span> <p>Molla<strong> Mobile</strong> <span>Thế giới máy tính<span></p> </a> </p>");
                    htmlBody.Append("</body></html>");

                    var emailModel = new EmailRequest()
                    {
                        To = userModel.Email,
                        Subject = title,
                        Body = htmlBody.ToString()
                    };
                    try
                    {
                        _ = Task.Factory.StartNew(() => { _emailService.SendAsync(emailModel); });
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                    #endregion
                    return new ResponseObject<string>(null, "Lấy mật khẩu thành công!", Code.Success);
                }
                return new ResponseObject<string>(null, "Lấy mật khẩu thất bại!", Code.ServerError);
            }
            catch (Exception ex)
            {
                Log.Error(ex, MessageConstants.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{MessageConstants.GetDataErrorMessage} - {ex.Message}");
            }
        }

        public async Task<Response> Create(UserModel model)
        {
            try
            {
                var entity = AutoMapperUtils.AutoMap<UserModel, User>(model);

                #region Check Username Unique
                var ckUserName = await _dataContext.Users.AnyAsync(x => x.Username.Equals(entity.Username));
                if (ckUserName)
                {
                    return new ResponseError(Code.BadRequest, "Tài khoản đã tồn tại");
                }
                #endregion
                #region Check Email Unique
                var ckMail = await _dataContext.Users.AnyAsync(x => x.Email.Equals(entity.Email));
                if (ckMail)
                {
                    return new ResponseError(Code.BadRequest, "Email đã tồn tại");
                }
                #endregion
                entity.Id = Guid.NewGuid();
                entity.PasswordSalt = Helpers.PassowrdCreateSalt512();
                entity.Password = Helpers.PasswordGenerateHmac(entity.Password, entity.PasswordSalt);
                await _dataContext.Users.AddAsync(entity);
                var role = _dataContext.Roles.FirstOrDefault(x => x.Code == "USER");
                var roleUser = new UserRole()
                {
                    Id = Guid.NewGuid(),
                    RoleId = role?.Id ?? Guid.Empty,
                    UserId = entity.Id
                };
                await _dataContext.UserRoles.AddAsync(roleUser);
                int dbSave = await _dataContext.SaveChangesAsync();

                if (dbSave > 0)
                {
                    #region Gửi email thông báo đến khách hàng

                    string title = "[Molla - Mobile] - Đăng ký tài khoản thành công";


                    StringBuilder htmlBody = new StringBuilder();
                    htmlBody.Append("<html><body>");
                    htmlBody.Append("<p>Xin chào <b>" + entity.Name + "</b>,</p>");
                    htmlBody.Append("<p>Bạn đã đăng ký tài khoản thành công. Hãy tiếp tục ghé thăm Molla Mobile để chọn lựa cho mình những sản phẩm ưu đãi nhất.</p>");
                    htmlBody.Append("<p><a href='" + _frontendUrl + "'><span class='fas fa - laptop'></span> <p>Molla<strong> Mobile</strong> <span>Thế giới điện tử<span></p> </a> </p>");
                    htmlBody.Append("</body></html>");

                    var emailModel = new EmailRequest()
                    {
                        To = model.Email,
                        Subject = title,
                        Body = htmlBody.ToString()
                    };
                    try
                    {
                        _ = Task.Factory.StartNew(() => { _emailService.SendAsync(emailModel); });
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                    #endregion
                    //Log.Information("Add success: " + JsonSerializer.Serialize(entity));
                    return new ResponseObject<UserModel>(model, MessageConstants.CreateSuccessMessage, Code.Success);
                }
                else
                {
                    return new ResponseError(Code.ServerError, MessageConstants.CreateErrorMessage);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, MessageConstants.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{MessageConstants.CreateErrorMessage} - {ex.Message}");
            }
        }
        // Generates a random password.  
        // 4-LowerCase + 4-Digits + 2-UpperCase  
        public string RandomPassword()
        {
            var passwordBuilder = new StringBuilder();

            // 4-Letters lower case   
            passwordBuilder.Append(RandomString(4, true));

            // 4-Digits between 1000 and 9999  
            passwordBuilder.Append(RandomNumber(1000, 9999));

            // 2-Letters upper case  
            passwordBuilder.Append(RandomString(2));
            return passwordBuilder.ToString();
        }
        // Generates a random string with a given size.    
        public string RandomString(int size, bool lowerCase = false)
        {
            var builder = new StringBuilder(size);

            // Unicode/ASCII Letters are divided into two blocks
            // (Letters 65–90 / 97–122):
            // The first group containing the uppercase letters and
            // the second group containing the lowercase.  

            // char is a single Unicode character  
            char offset = lowerCase ? 'a' : 'A';
            const int lettersOffset = 26; // A...Z or a..z: length=26  

            for (var i = 0; i < size; i++)
            {
                var @char = (char)_random.Next(offset, offset + lettersOffset);
                builder.Append(@char);
            }

            return lowerCase ? builder.ToString().ToLower() : builder.ToString();
        }
        // Generates a random number within a range.      
        public int RandomNumber(int min, int max)
        {
            return _random.Next(min, max);
        }
    }
}