using Domain.Common;
using System;

namespace Domain.Entities
{
    public class ProductReview : BaseTableEntity
    {
        public Guid ProductId { get; set; }
        public Guid UserId { get; set; }
        public Guid? ParentId { get; set; }
        public int Rating { get; set; }
        public bool IsRating { get; set; }
        public string Content { get; set; }
    }
}
