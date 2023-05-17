using Domain.Common;
using System;

namespace Domain.Entities
{
    public class ProductMeta:BaseTableEntity
    {
        public string Key { get; set; }
        public Guid ProductId { get; set; }
        public string Content { get; set; }
    }
}
