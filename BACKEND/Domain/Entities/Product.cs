using Domain.Common;
using System;

namespace Domain.Entities
{
    public class Product : BaseTableEntity
    {
        //public string Code { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        //public string SerialNumber { get; set; }
        //public string LoaiBaoHanh { get; set; }
        public int? VisitCount { get; set; }
        //public int ThoiGianBaoHanh { get; set; }
        public Guid SupplierId { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public int Sold { get; set; } = 0;
    }
}
