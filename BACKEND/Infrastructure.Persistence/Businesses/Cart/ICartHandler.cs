using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Common;

namespace Infrastructure.Persistence.Businesses.Cart
{
    /// <summary>
    /// Interface quản lý loại chứng chỉ
    /// </summary>
    public interface ICartHandler
    {
        /// <summary>
        /// Cập nhật giỏ hàng
        /// </summary>
        /// <param name="listProducts"></param>
        /// <returns>Id giỏ hàng</returns>
        Task<Response> Update(string listProducts);

        /// <summary>
        /// Lấy giỏ hàng theo Id
        /// </summary>
        /// <returns>Thông tin giỏ hàng</returns>
        Task<Response> GetById();
    }
}
