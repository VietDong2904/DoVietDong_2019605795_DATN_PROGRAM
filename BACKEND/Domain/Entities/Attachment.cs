using System;
using Domain.Common;

namespace Domain.Entities
{
    public class Attachment : BaseTableEntity
    {
        public Guid ObjectId { get; set; }
        public string FileName { get; set; }
        public string Url { get; set; }
    }
}
