using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Common;
using Infrastructure.Persistence.Businesses.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    public class CategoryController : ApiBaseController
    {
        private readonly ICategoryHandler _handler;
        public CategoryController(ICategoryHandler handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// Thêm mới loại sản phẩm
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
        /// <param name="model">Thông tin loại sản phẩm</param>
        /// <returns>Id loại sản phẩm</returns> 
        /// <response code="200">Thành công</response>
        [Authorize, HttpPost, Route("")]
        public async Task<IActionResult> Create([FromBody] CategoryBaseModel model)
        {
            return Ok(await _handler.Create(model));
        }
        /// <summary>
        /// Cập nhật loại sản phẩm
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
        /// <param name="model">Thông tin loại sản phẩm cần cập nhật</param>
        /// <returns>Id loại sản phẩm đã cập nhật thành công</returns> 
        /// <response code="200">Thành công</response>
        [Authorize, HttpPut, Route("")]
        public async Task<IActionResult> Update([FromBody] CategoryBaseModel model)
        {
            return Ok(await _handler.Update(model));
        }

        /// <summary>
        /// Lấy thông tin loại sản phẩm theo id
        /// </summary> 
        /// <param name="id">Id loại sản phẩm</param>
        /// <returns>Thông tin chi tiết loại sản phẩm</returns> 
        /// <response code="200">Thành công</response>
        [Authorize, HttpGet, Route("")]
        public async Task<IActionResult> GetById(Guid id)
        {
            return Ok(await _handler.GetById(id));
        }

        /// <summary>
        /// Lấy danh sách loại sản phẩm theo điều kiện lọc
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
        /// <returns>Danh sách loại sản phẩm</returns> 
        /// <response code="200">Thành công</response>
        [Authorize,HttpPost, Route("filter")]
        public async Task<IActionResult> Filter([FromBody] CategoryFilterModel filter)
        {
            return Ok(await _handler.Filter(filter));
        }
        /// <summary>
        /// Xóa loại sản phẩm
        /// </summary> 
        /// <remarks>
        /// Sample request:
        ///
        ///     [
        ///         "3fa85f64-5717-4562-b3fc-2c963f66afa6"
        ///     ]
        /// </remarks>
        /// <param name="listId">Danh sách Id loại sản phẩm</param>
        /// <returns>Danh sách kết quả xóa</returns> 
        /// <response code="200">Thành công</response>
        [Authorize, HttpDelete, Route("")]
        public async Task<IActionResult> Delete([FromBody] List<Guid> listId)
        {
            return Ok(await _handler.Delete(listId));
        }
        /// <summary>
        /// Lấy danh sách loại sản phẩm cho combobox
        /// </summary> 
        /// <param name="count">số bản ghi tối đa</param>
        /// <param name="ts">Từ khóa tìm kiếm</param>
        /// <returns>Danh sách loại sản phẩm</returns> 
        /// <response code="200">Thành công</response>
        [AllowAnonymous, HttpGet, Route("for-combobox")]
        public async Task<Response> GetListCombobox(int count = 0, string ts = "")
        {
            var result = await _handler.GetListCombobox(count, ts);
            return result;
        }
        [AllowAnonymous, HttpGet, Route("for-combobox-noparent")]
        public async Task<Response> GetListComboboxNoParent(int count = 0, string ts = "")
        {
            var result = await _handler.GetListComboboxNoParent(count, ts);
            return result;
        }
    }
}
