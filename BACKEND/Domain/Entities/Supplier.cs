using Domain.Common;

namespace Domain.Entities
{
    public class Supplier : BaseTableEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string? AvatarUrl { get; set; }
    }
}
