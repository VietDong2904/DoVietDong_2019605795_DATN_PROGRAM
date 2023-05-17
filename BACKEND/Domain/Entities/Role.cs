using Domain.Common;

namespace Domain.Entities
{
    public class Role:BaseTableEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
