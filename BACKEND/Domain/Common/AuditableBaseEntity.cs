using System;

namespace Domain.Common
{
    public abstract class BaseTableEntity : IBaseTableEntity
    {
        public Guid Id { get; set; }
        public bool Status { get; set; }
        public bool IsDeleted { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime Created { get; set; }
        public Guid LastModifiedBy { get; set; }
        public DateTime? LastModified { get; set; }
    }
    public interface IBaseTableEntity
    {
        Guid Id { get; set; }
        Guid CreatedBy { get; set; }
        DateTime Created { get; set; }
        Guid LastModifiedBy { get; set; }
        DateTime? LastModified { get; set; }
        public bool IsDeleted { get; set; }
    }
}
