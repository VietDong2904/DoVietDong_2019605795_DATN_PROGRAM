using Domain.Common;
using System;

namespace Domain.Entities
{
    public class Category : BaseTableEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsDisplay { get; set; }
        public Guid? ParentId { get; set; }
    }
}
