using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Common;

namespace Infrastructure.Persistence.Businesses.Supplier
{
    /// <summary>
    /// Interface quản lý nhà cung cấp
    /// </summary>
    public interface ISupplierHandler
    {
        /// <summary>
        /// Thêm mới nhà cung cấp
        /// </summary>
        /// <param name="model">Model thêm mới nhà cung cấp</param>
        /// <returns>Id nhà cung cấp</returns>
        Task<Response> Create(SupplierBaseModel model);

        /// <summary>
        /// Cập nhật nhà cung cấp
        /// </summary>
        /// <param name="model">Model cập nhật nhà cung cấp</param>
        /// <returns>Id nhà cung cấp</returns>
        Task<Response> Update(SupplierBaseModel model);

        /// <summary>
        /// Xóa nhà cung cấp
        /// </summary>
        /// <param name="listId">Danh sách Id nhà cung cấp</param>
        /// <returns>Danh sách kết quả xóa</returns>
        Task<Response> Delete(List<Guid> listId);

        /// <summary>
        /// Lấy danh sách nhà cung cấp theo điều kiện lọc
        /// </summary>
        /// <param name="filter">Model điều kiện lọc</param>
        /// <returns>Danh sách nhà cung cấp</returns>
        Task<Response> Filter(SupplierFilterModel filter);

        /// <summary>
        /// Lấy nhà cung cấp theo Id
        /// </summary>
        /// <param name="id">Id nhà cung cấp</param>
        /// <returns>Thông tin nhà cung cấp</returns>
        Task<Response> GetById(Guid id);
        /// <summary>
        /// Lấy danh sách nhà cung cấp cho combobox
        /// </summary>
        /// <param name="count">Số bản ghi tối đa</param>
        /// <param name="textSearch">Từ khóa tìm kiếm</param>
        /// <returns>Danh sách nhà cung cấp cho combobox</returns>
        Task<Response> GetListCombobox(int count = 0, string textSearch = "");
    }
}
