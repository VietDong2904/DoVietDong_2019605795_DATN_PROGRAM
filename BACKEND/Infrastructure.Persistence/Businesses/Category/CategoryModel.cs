using System;
using System.Collections.Generic;
using Common.Common;
using Common.Model;
using Infrastructure.Persistence.Businesses.Product;

namespace Infrastructure.Persistence.Businesses.Category
{
    public class CategoryBaseModel
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsDisplay { get; set; }
        public Guid? ParentId { get; set; }
        public string ParentName { get; set; }
    }

    public class CategoryFilterModel : RequestParameter
    {
        public string TextSearch { get; set; }
        public Guid? ParentId { get; set; }
        public bool? IsGetAll { get; set; }
    }

    public class ComboboxCateogryModel
    {
        public string Code { get; set; }
        public string ColorCode { get; set; }
        public Guid Id { get; set; }
        public Guid Key { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public bool IsLeaf { get; set; }
        public int Count { get; set; }
        public Guid? ParentId { get; set; }
        public List<ProductBaseModel> Products { get; set; }
        public List<ComboboxCateogryModel> Children { get; set; }
    }
}