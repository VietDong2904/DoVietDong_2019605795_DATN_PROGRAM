using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Common;

namespace Infrastructure.Persistence.Businesses.Role
{
    /// <summary>
    /// Interface quản lý quyền
    /// </summary>
    public interface IRoleHandler
    {
        /// <summary>
        /// Thêm mới quyền
        /// </summary>
        /// <param name="model">Model thêm mới quyền</param>
        /// <returns>Id quyền</returns>
        Task<Response> Create(RoleBaseModel model);

        /// <summary>
        /// Cập nhật quyền
        /// </summary>
        /// <param name="model">Model cập nhật quyền</param>
        /// <returns>Id quyền</returns>
        Task<Response> Update(RoleBaseModel model);

        /// <summary>
        /// Xóa quyền
        /// </summary>
        /// <param name="listId">Danh sách Id quyền</param>
        /// <returns>Danh sách kết quả xóa</returns>
        Task<Response> Delete(List<Guid> listId);

        /// <summary>
        /// Lấy danh sách quyền theo điều kiện lọc
        /// </summary>
        /// <param name="filter">Model điều kiện lọc</param>
        /// <returns>Danh sách quyền</returns>
        Task<Response> Filter(RoleFilterModel filter);

        /// <summary>
        /// Lấy quyền theo Id
        /// </summary>
        /// <param name="id">Id quyền</param>
        /// <returns>Thông tin quyền</returns>
        Task<Response> GetById(Guid id);
        /// <summary>
        /// Lấy danh sách quyền cho combobox
        /// </summary>
        /// <param name="count">Số bản ghi tối đa</param>
        /// <param name="textSearch">Từ khóa tìm kiếm</param>
        /// <returns>Danh sách quyền cho combobox</returns>
        Task<Response> GetListCombobox(int count = 0, string textSearch = "");
    }
}
