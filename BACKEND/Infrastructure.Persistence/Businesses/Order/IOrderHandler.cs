using Common.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Businesses.Order
{
    /// <summary>
    /// Interface quản lý loại chứng chỉ
    /// </summary>
    public interface IOrderHandler
    {
        /// <summary>
        /// Thêm mới đơn đặt
        /// </summary>
        /// <param name="model">Model thêm mới đơn đặt</param>
        /// <returns>Id đơn đặt</returns>
        Task<Response> Create(OrderCreateModel model);

        /// <summary>
        /// Cập nhật đơn đặt
        /// </summary>
        /// <param name="model">Model cập nhật đơn đặt</param>
        /// <returns>Id đơn đặt</returns>
        Task<Response> Update(OrderUpdateModel model);

        /// <summary>
        /// Cập nhật trạng thái đơn đặt
        /// </summary>
        /// <param name="model">Model cập nhật đơn đặt</param>
        /// <returns>Id đơn đặt</returns>
        Task<Response> UpdateStatusOrder(OrderUpdateModel model);

        /// <summary>
        /// Xóa đơn đặt
        /// </summary>
        /// <param name="listId">Danh sách Id đơn đặt</param>
        /// <returns>Danh sách kết quả xóa</returns>
        Task<Response> Delete(List<Guid> listId);

        /// <summary>
        /// Lấy danh sách đơn đặt theo điều kiện lọc
        /// </summary>
        /// <param name="filter">Model điều kiện lọc</param>
        /// <returns>Danh sách đơn đặt</returns>
        Task<Response> Filter(OrderQueryFilter filter);

        /// <summary>
        /// Lấy đơn đặt theo Id
        /// </summary>
        /// <param name="id">Id đơn đặt</param>
        /// <returns>Thông tin đơn đặt</returns>
        Task<Response> GetById(Guid id,string ts);
        /// <summary>
        /// Kiểm tra thông tin baaro hành
        /// </summary>
        /// <param name="serial">Serial</param>
        /// <returns>Thông tin đơn đặt</returns>
        /*Task<Response> CheckInsurance(string serial);*/
    }
}
