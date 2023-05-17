using Domain.Common;

namespace Domain.Entities
{
    public class Size:BaseTableEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
