using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Common;
using Infrastructure.Persistence.Businesses.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    public class OrderController : ApiBaseController
    {
        private readonly IOrderHandler _handler;
        public OrderController(IOrderHandler handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// Thêm mới đơn đặt
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     {
        ///         "code": "Code",
        ///         "name": "Name",
        ///         "status": true,
        ///         "description": "Description",
        ///         "order": 1
        ///     }
        /// </remarks>
        /// <param name="model">Thông tin đơn đặt</param>
        /// <returns>Id đơn đặt</returns> 
        /// <response code="200">Thành công</response>
        [Authorize, HttpPost, Route("")]
        [ProducesResponseType(typeof(ResponseObject<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create([FromBody] OrderCreateModel model)
        {
            var result = await _handler.Create(model);
            return Ok(result);
        }
        /// <summary>
        /// Cập nhật đơn đặt
        /// </summary> 
        /// <remarks>
        /// Sample request:
        ///
        ///     {
        ///         "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///         "code": "Code",
        ///         "name": "Name",
        ///         "status": true,
        ///         "description": "Description",
        ///         "order": 1
        ///     }   
        /// </remarks>
        /// <param name="model">Thông tin đơn đặt cần cập nhật</param>
        /// <returns>Id đơn đặt đã cập nhật thành công</returns> 
        /// <response code="200">Thành công</response>
        [Authorize, HttpPut, Route("")]
        [ProducesResponseType(typeof(ResponseObject<Guid>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromBody] OrderUpdateModel model)
        {
            var result = await _handler.Update(model);
            return Ok(result);
        }

        [Authorize, HttpPut, Route("update-status")]
        [ProducesResponseType(typeof(ResponseObject<Guid>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateStatusOrder([FromBody] OrderUpdateModel model)
        {
            var result = await _handler.UpdateStatusOrder(model);
            return Ok(result);
        }
        [AllowAnonymous, HttpGet, Route("check-insurance")]
        /*[ProducesResponseType(typeof(ResponseObject<InsuranceFilterModel>), StatusCodes.Status200OK)]
        public async Task<Response> CheckInsurance([FromQuery] string serial)
        {
            var result = await _handler.CheckInsurance(serial);

            return result;
        }*/
        /// <summary>
        /// Lấy thông tin đơn đặt theo id
        /// </summary> 
        /// <param name="id">Id đơn đặt</param>
        /// <returns>Thông tin chi tiết đơn đặt</returns> 
        /// <response code="200">Thành công</response>
        [Authorize, HttpGet, Route("")]
        [ProducesResponseType(typeof(ResponseObject<List<OrderBaseModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(Guid id, string ts)
        {
            var result = await _handler.GetById(id,ts);
            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách đơn đặt theo điều kiện lọc
        /// </summary> 
        /// <remarks>
        /// Sample request:
        ///
        ///     {
        ///         "textSearch": "",
        ///         "pageSize": 20,
        ///         "pageNumber": 1
        ///     }
        /// </remarks>
        /// <param name="filter">Điều kiện lọc</param>
        /// <returns>Danh sách đơn đặt</returns> 
        /// <response code="200">Thành công</response>
        [Authorize, HttpPost, Route("filter")]
        [ProducesResponseType(typeof(ResponseObject<List<OrderBaseModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Filter([FromBody] OrderQueryFilter filter)
        {
            var result = await _handler.Filter(filter);
            return Ok(result);
        }
        /// <summary>
        /// Xóa đơn đặt
        /// </summary> 
        /// <remarks>
        /// Sample request:
        ///
        ///     [
        ///         "3fa85f64-5717-4562-b3fc-2c963f66afa6"
        ///     ]
        /// </remarks>
        /// <param name="listId">Danh sách Id đơn đặt</param>
        /// <returns>Danh sách kết quả xóa</returns> 
        /// <response code="200">Thành công</response>
        [Authorize, HttpDelete, Route("")]
        [ProducesResponseType(typeof(ResponseObject<List<ResponeDeleteModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Delete([FromBody] List<Guid> listId)
        {
            var result = await _handler.Delete(listId);

            return Ok(result);
        }
    }
}
