using Common.Model;
using System;

namespace Infrastructure.Persistence.Businesses.Supplier
{
    public class SupplierBaseModel
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool Status { get; set; }
        public string AvatarUrl { get; set; }
    }

    public class SupplierFilterModel:RequestParameter
    {
        public string TextSearch { get; set; }
        public bool? IsGetAll { get; set; }
        public bool? Status { get; set; }
    }
}