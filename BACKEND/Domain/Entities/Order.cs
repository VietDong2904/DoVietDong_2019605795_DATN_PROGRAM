using Domain.Common;
using System;

namespace Domain.Entities
{
    public class Order : BaseTableEntity
    {
        public string Code { get; set; }
        public string? VoucherCode { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
        public new int Status { get; set; }
        public Guid? UserId { get; set; }
        public string Email { get; set; }
        public string CityId { get; set; }
        public string Note { get; set; }
        public string Description { get; set; }
        public string DistrictId { get; set; }
        public string CommuneId { get; set; }
        /// <summary>
        /// Giảm giá
        /// </summary>
        public double? Discount { get; set; }
        /// <summary>
        /// Giá ship
        /// </summary>
        public double? Shipping { get; set; }
        /// <summary>
        /// Thuế VAT
        /// </summary>
        public double? Vat { get; set; }
        /// <summary>
        /// 0. Trực tiếp, 1. Online
        /// </summary>
        public int PhuongThucThanhToan { get; set; }
        public string AddressDetail { get; set; }
        /// <summary>
        /// Đã xác nhận
        /// </summary>
        public DateTime? ConfirmedDate { get; set; }
        /// <summary>
        /// Đang giao
        /// </summary>
        public DateTime? DeliverDate { get; set; }
        /// <summary>
        /// Đã nhận
        /// </summary>
        public DateTime? RecivedDate { get; set; }
        /// <summary>
        /// Đã hủy
        /// </summary>
        public DateTime? CanceledDate { get; set; }
        /// <summary>
        /// Danh sách sản phẩm trong hóa đơn dạng JSON
        /// </summary>
        public string ListProducts { get; set; }

        public double? GrandTotal { get; set; }
    }
}
