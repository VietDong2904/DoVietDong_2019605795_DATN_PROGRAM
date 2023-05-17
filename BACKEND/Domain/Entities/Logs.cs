using System;
using Domain.Common;

namespace Domain.Entities
{
    /// <summary>
    /// Bảng ghi log
    /// </summary>
    public class Logs
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string Level { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Exception { get; set; }
        public string Properties { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Action { get; set; }
        public string Module { get; set; }
        public string IpAddress { get; set; }
    }
}
