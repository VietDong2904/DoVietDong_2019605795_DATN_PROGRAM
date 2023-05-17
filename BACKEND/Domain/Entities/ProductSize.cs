using System;
using Domain.Common;

namespace Domain.Entities
{
    public class ProductSize:BaseTableEntity
    {
        public Guid ProductId { get; set; }
        public Guid SizeId { get; set; }
        public int Quantity { get; set; }
        public double? Price { get; set; }
        public double? Discount { get; set; }
    }
}
