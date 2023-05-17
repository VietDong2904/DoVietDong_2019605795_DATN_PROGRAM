using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Common;

namespace Infrastructure.Persistence.Businesses.Color
{
    /// <summary>
    /// Interface quản lý màu sắc
    /// </summary>
    public interface IColorHandler
    {
        /// <summary>
        /// Thêm mới màu sắc
        /// </summary>
        /// <param name="model">Model thêm mới màu sắc</param>
        /// <returns>Id màu sắc</returns>
        Task<Response> Create(ColorBaseModel model);

        /// <summary>
        /// Cập nhật màu sắc
        /// </summary>
        /// <param name="model">Model cập nhật màu sắc</param>
        /// <returns>Id màu sắc</returns>
        Task<Response> Update(ColorBaseModel model);

        /// <summary>
        /// Xóa màu sắc
        /// </summary>
        /// <param name="listId">Danh sách Id màu sắc</param>
        /// <returns>Danh sách kết quả xóa</returns>
        Task<Response> Delete(List<Guid> listId);

        /// <summary>
        /// Lấy danh sách màu sắc theo điều kiện lọc
        /// </summary>
        /// <param name="filter">Model điều kiện lọc</param>
        /// <returns>Danh sách màu sắc</returns>
        Task<Response> Filter(ColorFilterModel filter);

        /// <summary>
        /// Lấy màu sắc theo Id
        /// </summary>
        /// <param name="id">Id màu sắc</param>
        /// <returns>Thông tin màu sắc</returns>
        Task<Response> GetById(Guid id);
        /// <summary>
        /// Lấy danh sách màu sắc cho combobox
        /// </summary>
        /// <param name="count">Số bản ghi tối đa</param>
        /// <param name="textSearch">Từ khóa tìm kiếm</param>
        /// <returns>Danh sách màu sắc cho combobox</returns>
        Task<Response> GetListCombobox(int count = 0, string textSearch = "");
    }
}
