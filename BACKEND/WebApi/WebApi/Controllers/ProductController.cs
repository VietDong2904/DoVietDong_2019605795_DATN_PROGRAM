using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Common;
using Infrastructure.Persistence.Businesses.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    public class ProductController : ApiBaseController
    {
        private readonly IProductHandler _handler;

        public ProductController(IProductHandler handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// Thêm mới sản phẩm
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
        /// <param name="model">Thông tin sản phẩm</param>
        /// <returns>Id sản phẩm</returns> 
        /// <response code="200">Thành công</response>
        [Authorize, HttpPost, Route("")]
        public async Task<IActionResult> Create([FromBody] ProductBaseModel model)
        {
            var result = await _handler.Create(model);
            return Ok(result);
        }

        /// <summary>
        /// Cập nhật sản phẩm
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
        /// <param name="model">Thông tin sản phẩm cần cập nhật</param>
        /// <returns>Id sản phẩm đã cập nhật thành công</returns> 
        /// <response code="200">Thành công</response>
        [Authorize, HttpPut, Route("")]
        public async Task<IActionResult> Update([FromBody] ProductBaseModel model)
        {
            var result = await _handler.Update(model);

            return Ok(result);
        }

        /// <summary>
        /// Lấy thông tin sản phẩm theo id
        /// </summary> 
        /// <returns>Thông tin chi tiết sản phẩm</returns> 
        /// <response code="200">Thành công</response>
        [AllowAnonymous, HttpGet, Route("")]
        public async Task<Response> GetByCode(Guid id)
        {
            var result = await _handler.GetById(id);

            return result;
        }

        /// <summary>
        /// Lấy danh sách sản phẩm theo điều kiện lọc
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
        /// <returns>Danh sách sản phẩm</returns> 
        /// <response code="200">Thành công</response>
        [AllowAnonymous, HttpPost, Route("filter")]
        public async Task<Response> Filter([FromBody] ProductQueryFilter filter)
        {
            var result = await _handler.Filter(filter);

            return result;
        }

        [AllowAnonymous, HttpGet, Route("similar")]
        public async Task<Response> Smilar(string id)
        {
            var result = await _handler.GetListProductSimilar();

            return result;
        }

        [AllowAnonymous, HttpPost, Route("update-visit-count")]
        [ProducesResponseType(typeof(ResponseObject<List<ProductBaseModel>>), StatusCodes.Status200OK)]
        public async Task<Response> UpdateVisitCount(VisitCountModel model)
        {
            var result = await _handler.UpdateVisitCount(model);

            return result;
        }

        /// <summary>
        /// Xóa sản phẩm
        /// </summary> 
        /// <remarks>
        /// Sample request:
        ///
        ///     [
        ///         "3fa85f64-5717-4562-b3fc-2c963f66afa6"
        ///     ]
        /// </remarks>
        /// <param name="listId">Danh sách Id sản phẩm</param>
        /// <returns>Danh sách kết quả xóa</returns> 
        /// <response code="200">Thành công</response>
        [AllowAnonymous, HttpDelete, Route("")]
        public async Task<IActionResult> Delete([FromBody] List<Guid> listId)
        {
            var result = await _handler.Delete(listId);

            return Ok(result);
        }
        /// <summary>
        /// Lấy tất cả danh sách sản phẩm
        /// </summary> 
        /// <param name="ts">Từ khóa tìm kiếm</param>
        /// <returns>Danh sách sản phẩm</returns> 
        /// <response code="200">Thành công</response>
        [AllowAnonymous, HttpGet, Route("all")]
        public async Task<Response> GetAll(string ts = null)
        {
            ProductQueryFilter filter = new ProductQueryFilter()
            {
                TextSearch = ts,
                PageNumber = null,
                PageSize = null,
                IsGetAll = true
            };
            var result = await _handler.Filter(filter);
            return result;
        }
        /// <summary>
        /// Lấy danh sách sản phẩm cho combobox
        /// </summary> 
        /// <param name="count">số bản ghi tối đa</param>
        /// <param name="ts">Từ khóa tìm kiếm</param>
        /// <returns>Danh sách sản phẩm</returns> 
        /// <response code="200">Thành công</response>
        [AllowAnonymous, HttpGet, Route("for-combobox")]
        public async Task<IActionResult> GetListCombobox(int count = 0, string ts = "")
        {
            var result = await _handler.GetListCombobox(count, ts);
            return Ok(result);
        }
    }
}