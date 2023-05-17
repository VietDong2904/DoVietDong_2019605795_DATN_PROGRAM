using System;
using Domain.Common;

namespace Domain.Entities
{
    public class CategoryProduct:BaseTableEntity
    {
        public Guid ProductId { get; set; }
        public Guid CategoryId { get; set; }
    }
}
