using Common.Model;
using System;

namespace Infrastructure.Persistence.Businesses.Color
{
    public class ColorBaseModel
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string ColorCode { get; set; }
        public bool Status { get; set; }
    }

    public class ColorFilterModel:RequestParameter
    {
        public string TextSearch { get; set; }
        public bool? IsGetAll { get; set; }
        public bool? Status { get; set; }
    }
}