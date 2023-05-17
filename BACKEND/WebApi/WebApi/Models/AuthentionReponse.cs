using System.Collections.Generic;

namespace WebApi.Models
{
    public class AuthResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
    public class PermissionResponse
    {
        public string Name { get; set; }
        public string Policy { get { return Name; } }
    }
    public class AuthenticationResponse
    {
        public Data Data { get; set; }
        public string StatusCode { get; set; }
        public bool Succeeded { get; set; }
        public string Code { get; set; }
        public string Message { get; set; }
    }
    public class Data
    {
        public string UserName { get; set; }
        public string UserId { get; set; }

        public UserInformation UserInformation { get; set; }
        public List<Permission> ListPermissions { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
    public class UserInformation
    {
        public string Id { get; set; }
        public string HoTen { get; set; }
        public string AnhDaiDien_FilePath { get; set; }
        public string ChucVu { get; set; }
        public string DonVi { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string UserCanBoDepartmentId { get; set; }
        public DonViTrucThuoc DonViGoc { get; set; }
        public DonViTrucThuoc donViTrucThuoc { get; set; }

    }
    public class DonViTrucThuoc
    {
        public string Id { get; set; }
        public string TenDonVi { get; set; }
    }
    public class Permission
    {
        public string Name { get; set; }
        public string Policy { get { return Name; } }
    }
    public class DataResponse
    {
        public UserInformation UserInformation { get; set; }
        public List<Permission> Permissions { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string StatusCode { get; set; }
    }
}
