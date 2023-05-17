using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Common;

namespace Infrastructure.Persistence.Businesses.ProductReview
{
    /// <summary>
    /// Interface quản lý loại chứng chỉ
    /// </summary>
    public interface IProductReviewHandler
    {
        /// <summary>
        /// Thêm mới đánh giá sản phẩm
        /// </summary>
        /// <param name="model">Model thêm mới đánh giá sản phẩm</param>
        /// <returns>Id đánh giá sản phẩm</returns>
        Task<Response> Create(ProductReviewCreateModel model);

        /// <summary>
        /// Thêm mới đánh giá sản phẩm theo danh sách
        /// </summary>
        /// <param name="list">Danh sách thông tin đánh giá sản phẩm</param>
        /// <returns>Danh sách kết quả thêm mới</returns> 
        Task<Response> CreateMany(List<ProductReviewCreateModel> list);

        /// <summary>
        /// Cập nhật đánh giá sản phẩm
        /// </summary>
        /// <param name="model">Model cập nhật đánh giá sản phẩm</param>
        /// <returns>Id đánh giá sản phẩm</returns>
        Task<Response> Update(ProductReviewUpdateModel model);

        /// <summary>
        /// Xóa đánh giá sản phẩm
        /// </summary>
        /// <param name="listId">Danh sách Id đánh giá sản phẩm</param>
        /// <returns>Danh sách kết quả xóa</returns>
        Task<Response> Delete(List<Guid> listId);

        /// <summary>
        /// Lấy danh sách đánh giá sản phẩm theo điều kiện lọc
        /// </summary>
        /// <param name="filter">Model điều kiện lọc</param>
        /// <returns>Danh sách đánh giá sản phẩm</returns>
        Task<Response> Filter(ProductReviewQueryFilter filter);

        /// <summary>
        /// Lấy đánh giá sản phẩm theo Id
        /// </summary>
        /// <param name="id">Id đánh giá sản phẩm</param>
        /// <returns>Thông tin đánh giá sản phẩm</returns>
        Task<Response> GetById(Guid id);
    }
}
