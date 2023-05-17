using System;
using Domain.Common;

namespace Domain.Entities
{
    public class OrderProduct : BaseTableEntity
    {
        public Guid ProductId { get; set; }
        public Guid OrderId { get; set; }
        public double Price { get; set; }
        public double Discount { get; set; }
        public int Quantity { get; set; }
    }
}
