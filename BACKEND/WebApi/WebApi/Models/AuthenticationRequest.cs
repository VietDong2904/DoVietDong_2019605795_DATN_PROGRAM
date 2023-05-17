namespace WebApi.Models
{
    public class AuthenticationUserCommonRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string AppCode { get { return "DTC"; } }
    }
}
