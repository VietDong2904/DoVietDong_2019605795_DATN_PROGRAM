using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Common;

namespace Infrastructure.Persistence.Businesses.Product
{
    /// <summary>
    /// Interface quản lý Product
    /// </summary>
    public interface IProductHandler
    {
        /// <summary>
        /// Thêm mới Product
        /// </summary>
        /// <param name="model">Model thêm mới Product</param>
        /// <returns>Id Product</returns>
        Task<Response> Create(ProductBaseModel model);
        /// <summary>
        /// Cập nhật Product
        /// </summary>
        /// <param name="model">Model cập nhật Product</param>
        /// <returns>Id Product</returns>
        Task<Response> Update(ProductBaseModel model);
        /// <summary>
        /// Xóa Product
        /// </summary>
        /// <param name="listId">Danh sách Id Product</param>
        /// <returns>Danh sách kết quả xóa</returns>
        Task<Response> Delete(List<Guid> listId);
        /// <summary>
        /// Lấy danh sách Product theo điều kiện lọc
        /// </summary>
        /// <param name="filter">Model điều kiện lọc</param>
        /// <returns>Danh sách Product</returns>
        Task<ResponseObject<PaginationList<ProductBaseModel>>> Filter(ProductQueryFilter filter);
        Task<ResponseObject<List<ProductBaseModel>>> GetListProductSimilar();
        /// <summary>
        /// Cập nhật lượt truy cập
        /// </summary>
        /// <param name="mode">Model điều kiện lọc</param>
        /// <returns>Danh sách Product</returns>
        Task<Response> UpdateVisitCount(VisitCountModel model);
        /// <summary>
        /// Lấy Product theo Code
        /// </summary>
        /// <param name="id">Code Product</param>
        /// <returns>Thông tin Product</returns>
        Task<Response> GetById(Guid id);

        Task<Response> GetListCombobox(int count = 0, string textSearch = "");
    }
}