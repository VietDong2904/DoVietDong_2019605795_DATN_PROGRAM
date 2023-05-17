using Domain.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Message : BaseTableEntity
    {
        public Guid ReceiverId { get; set; }
        public Guid DocumentId { get; set; }
        /// <summary>
        /// Loại Message SMS, NOTI
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Người nhận
        /// </summary>
        [MaxLength(255)]
        public string ReceiverName { get; set; }
        [MaxLength(255)]
        public string Email { get; set; }
        [MaxLength(255)]
        public string PhoneNumber { get; set; }
        public DateTime? SentDate { get; set; }
        /// <summary>
        /// Đã gửi
        /// </summary>
        public bool IsSent { get; set; }
        /// <summary>
        /// Đã nhận
        /// </summary>
        public bool IsReceived { get; set; }
        /// <summary>
        /// Đã xem
        /// </summary>
        public bool IsViewed { get; set; }

        public Guid SendBy { get; set; }
        public DateTime? ViewedDate { get; set; }
        public string ContentMessage { get; set; }
        public bool IsViewAll { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }

    }
}
