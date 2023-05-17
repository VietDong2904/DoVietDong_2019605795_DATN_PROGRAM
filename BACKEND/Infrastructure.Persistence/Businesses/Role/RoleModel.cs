using Common.Model;
using System;

namespace Infrastructure.Persistence.Businesses.Role
{
    public class RoleBaseModel
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public bool Status { get; set; }
    }

    public class RoleFilterModel:RequestParameter
    {
        public string TextSearch { get; set; }
        public bool? Status { get; set; }
        public bool? IsGetAll { get; set; }
    }
}