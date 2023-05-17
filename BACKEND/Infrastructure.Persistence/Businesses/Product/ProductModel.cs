using System;
using System.Collections.Generic;
using Domain.Entities;

namespace Infrastructure.Persistence.Businesses.Product
{
    public class ProductBaseModel
    {
        public Guid Id { get; set; }
        //public string Code { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        //public string SerialNumber { get; set; }
        public string CategoryString { get; set; }
        //public string LoaiBaoHanh { get; set; }
        public double? AmoutDefault { get; set; }
        public double? DiscountDefault { get; set; }
        public double Rating { get; set; }
        public int Count { get; set; }
        public int? VisitCount { get; set; }
        public bool Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        //public int ThoiGianBaoHanh { get; set; }
        public Guid SupplierId { get; set; }
        public Guid ProductId { get; set; }
        public Guid SizeId { get; set; }
        public int Amount { get; set; }
        public string SupplierName { get; set; }
        public List<Domain.Entities.Category> Categories { get; set; }
        public List<ProductSizeModel> ProductColors { get; set; }
        public List<string> Attachments { get; set; }
        public List<ProductMeta> ProductMetas { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public int Sold { get; set; } 
    }

    public class ProductModel : ProductBaseModel
    {
        public int Order { get; set; } = 0;

    }

    public class ProductSizeModel:ProductSize
    {
        public string Name { get; set; }
    }
    public class GetByCode
    {
        public string Code { get; set; }
    }
    public class VisitCountModel
    {
        public Guid Id { get; set; }
    }
    public class ProductQueryFilter
    {
        public string TextSearch { get; set; }
        public Guid? SupplierId { get; set; }
        public bool? IsGetAll { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? ColorId { get; set; }
        public int? PageSize { get; set; }
        public int? PageNumber { get; set; }
        public string CategoryCode { get; set; }
        public string ProductCode { get; set; }
        public double? MaxPrice { get; set; }
        public double? MinPrice { get; set; }
        public int? SortType { get; set; }
        public bool? Status { get; set; }
        public Guid? SizeId { get; set; }
    }
}