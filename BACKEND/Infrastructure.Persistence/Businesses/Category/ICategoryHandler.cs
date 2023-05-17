using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Common;

namespace Infrastructure.Persistence.Businesses.Category
{
    /// <summary>
    /// Interface quản lý loại chứng chỉ
    /// </summary>
    public interface ICategoryHandler
    {
        /// <summary>
        /// Thêm mới loại sản phẩm
        /// </summary>
        /// <param name="model">Model thêm mới loại sản phẩm</param>
        /// <returns>Id loại sản phẩm</returns>
        Task<Response> Create(CategoryBaseModel model);

        /// <summary>
        /// Cập nhật loại sản phẩm
        /// </summary>
        /// <param name="model">Model cập nhật loại sản phẩm</param>
        /// <returns>Id loại sản phẩm</returns>
        Task<Response> Update(CategoryBaseModel model);

        /// <summary>
        /// Xóa loại sản phẩm
        /// </summary>
        /// <param name="listId">Danh sách Id loại sản phẩm</param>
        /// <returns>Danh sách kết quả xóa</returns>
        Task<Response> Delete(List<Guid> listId);

        /// <summary>
        /// Lấy danh sách loại sản phẩm theo điều kiện lọc
        /// </summary>
        /// <param name="filter">Model điều kiện lọc</param>
        /// <returns>Danh sách loại sản phẩm</returns>
        Task<Response> Filter(CategoryFilterModel filter);

        /// <summary>
        /// Lấy loại sản phẩm theo Id
        /// </summary>
        /// <param name="id">Id loại sản phẩm</param>
        /// <returns>Thông tin loại sản phẩm</returns>
        Task<Response> GetById(Guid id);
        /// <summary>
        /// Lấy danh sách loại sản phẩm cho combobox
        /// </summary>
        /// <param name="count">Số bản ghi tối đa</param>
        /// <param name="textSearch">Từ khóa tìm kiếm</param>
        /// <returns>Danh sách loại sản phẩm cho combobox</returns>
        Task<Response> GetListCombobox(int count = 0, string textSearch = "");
        Task<Response> GetListComboboxNoParent(int count = 0, string textSearch = "");

    }
}
