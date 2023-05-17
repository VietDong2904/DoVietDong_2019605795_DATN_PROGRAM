using System;
using System.Collections.Generic;

namespace Common.Model
{
    public class BaseModel
    {
    }

    public class ListComboboxModel
    {
        public string Code { get; set; }
        public Guid Id { get; set; }
        public Guid Key { get; set; }
        public string Title { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public bool IsLeaf { get; set; }
        public int Count { get; set; }
        public Guid? ParentId { get; set; }
        public List<ListComboboxModel> Children { get; set; }
    }
    public class ResponeDeleteModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool Result { get; set; }
        public string Message { get; set; }
    }
    public class SysParamModel
    {
        public Guid Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class RequestParameter
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public RequestParameter()
        {
            this.PageNumber = 1;
            this.PageSize = 10;
        }
        public RequestParameter(int pageNumber, int pageSize)
        {
            this.PageNumber = pageNumber < 1 ? 1 : pageNumber;
            this.PageSize = pageSize > 10 ? 10 : pageSize;
        }

    }
}
