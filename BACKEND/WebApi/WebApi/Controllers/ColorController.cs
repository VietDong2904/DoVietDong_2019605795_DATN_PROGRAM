using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Common;
using Infrastructure.Persistence.Businesses.Color;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    public class ColorController : ApiBaseController
    {
        private readonly IColorHandler _handler;
        public ColorController(IColorHandler handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// Thêm mới màu sắc
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
        /// <param name="model">Thông tin màu sắc</param>
        /// <returns>Id màu sắc</returns> 
        /// <response code="200">Thành công</response>
        [Authorize, HttpPost, Route("")]
        public async Task<IActionResult> Create([FromBody] ColorBaseModel model)
        {
            return Ok(await _handler.Create(model));
        }
        /// <summary>
        /// Cập nhật màu sắc
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
        /// <param name="model">Thông tin màu sắc cần cập nhật</param>
        /// <returns>Id màu sắc đã cập nhật thành công</returns> 
        /// <response code="200">Thành công</response>
        [Authorize, HttpPut, Route("")]
        public async Task<IActionResult> Update([FromBody] ColorBaseModel model)
        {
            return Ok(await _handler.Update(model));
        }

        /// <summary>
        /// Lấy thông tin màu sắc theo id
        /// </summary> 
        /// <param name="id">Id màu sắc</param>
        /// <returns>Thông tin chi tiết màu sắc</returns> 
        /// <response code="200">Thành công</response>
        [Authorize, HttpGet, Route("")]
        public async Task<IActionResult> GetById(Guid id)
        {
            return Ok(await _handler.GetById(id));
        }

        /// <summary>
        /// Lấy danh sách màu sắc theo điều kiện lọc
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
        /// <returns>Danh sách màu sắc</returns> 
        /// <response code="200">Thành công</response>
        [Authorize,HttpPost, Route("filter")]
        public async Task<IActionResult> Filter([FromBody] ColorFilterModel filter)
        {
            return Ok(await _handler.Filter(filter));
        }
        /// <summary>
        /// Xóa màu sắc
        /// </summary> 
        /// <remarks>
        /// Sample request:
        ///
        ///     [
        ///         "3fa85f64-5717-4562-b3fc-2c963f66afa6"
        ///     ]
        /// </remarks>
        /// <param name="listId">Danh sách Id màu sắc</param>
        /// <returns>Danh sách kết quả xóa</returns> 
        /// <response code="200">Thành công</response>
        [Authorize, HttpDelete, Route("")]
        public async Task<IActionResult> Delete([FromBody] List<Guid> listId)
        {
            return Ok(await _handler.Delete(listId));
        }
        /// <summary>
        /// Lấy danh sách màu sắc cho combobox
        /// </summary> 
        /// <param name="count">số bản ghi tối đa</param>
        /// <param name="ts">Từ khóa tìm kiếm</param>
        /// <returns>Danh sách màu sắc</returns> 
        /// <response code="200">Thành công</response>
        [AllowAnonymous, HttpGet, Route("for-combobox")]
        public async Task<Response> GetListCombobox(int count = 0, string ts = "")
        {
            var result = await _handler.GetListCombobox(count, ts);
            return result;
        }
    }
}
