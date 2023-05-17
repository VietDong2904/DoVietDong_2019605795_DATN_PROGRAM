using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Common;
using Infrastructure.Persistence.Businesses.ProductReview;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{api-version:apiVersion}/productreview")]
    [ApiExplorerSettings(GroupName = "đánh giá sản phẩm")]
    public class ProductReviewController : ApiBaseController
    {
        private readonly IProductReviewHandler _handler;
        public ProductReviewController(IProductReviewHandler handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// Thêm mới đánh giá sản phẩm
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
        /// <param name="model">Thông tin đánh giá sản phẩm</param>
        /// <returns>Id đánh giá sản phẩm</returns> 
        /// <response code="200">Thành công</response>
        [Authorize, HttpPost, Route("")]
        [ProducesResponseType(typeof(ResponseObject<Guid>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create([FromBody] ProductReviewCreateModel model)
        {
            var result = await _handler.Create(model);

            return Ok(result);
        }

        /// <summary>
        /// Thêm mới đánh giá sản phẩm theo danh sách
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     [
        ///         {
        ///             "code": "Code",
        ///             "name": "Name",
        ///             "status": true,
        ///             "description": "Description",
        ///             "order": 1
        ///         }   
        ///     ]
        /// </remarks>
        /// <param name="list">Danh sách thông tin đánh giá sản phẩm</param>
        /// <returns>Danh sách kết quả thêm mới</returns> 
        /// <response code="200">Thành công</response>
        [Authorize, HttpPost, Route("create-many")]
        [ProducesResponseType(typeof(ResponseObject<Guid>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateMany([FromBody] List<ProductReviewCreateModel> list)
        {
            var result = await _handler.CreateMany(list);
            return Ok(result);
        }
        /// <summary>
        /// Cập nhật đánh giá sản phẩm
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
        /// <param name="model">Thông tin đánh giá sản phẩm cần cập nhật</param>
        /// <returns>Id đánh giá sản phẩm đã cập nhật thành công</returns> 
        /// <response code="200">Thành công</response>
        [Authorize, HttpPut, Route("")]
        [ProducesResponseType(typeof(ResponseObject<Guid>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update([FromBody] ProductReviewUpdateModel model)
        {
            var result = await _handler.Update(model);

            return Ok(result);
        }

        /// <summary>
        /// Lấy thông tin đánh giá sản phẩm theo id
        /// </summary> 
        /// <param name="id">Id đánh giá sản phẩm</param>
        /// <returns>Thông tin chi tiết đánh giá sản phẩm</returns> 
        /// <response code="200">Thành công</response>
        [AllowAnonymous, HttpGet, Route("")]
        [ProducesResponseType(typeof(ResponseObject<List<ProductReviewBaseModel>>), StatusCodes.Status200OK)]
        public async Task<Response> GetById(Guid id)
        {
            var result = await _handler.GetById(id);

            return result;
        }

        /// <summary>
        /// Lấy danh sách đánh giá sản phẩm theo điều kiện lọc
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
        /// <returns>Danh sách đánh giá sản phẩm</returns> 
        /// <response code="200">Thành công</response>
        [Authorize, HttpPost, Route("filter")]
        [ProducesResponseType(typeof(ResponseObject<List<ProductReviewBaseModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Filter([FromBody] ProductReviewQueryFilter filter)
        {
            var result = await _handler.Filter(filter);

            return Ok(result);
        }
        /// <summary>
        /// Lấy tất cả danh sách đánh giá sản phẩm
        /// </summary> 
        /// <param name="ts">Từ khóa tìm kiếm</param>
        /// <returns>Danh sách đánh giá sản phẩm</returns> 
        /// <response code="200">Thành công</response>
        [Authorize, HttpGet, Route("all")]
        [ProducesResponseType(typeof(ResponseObject<List<ProductReviewBaseModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(string ts = null)
        {
            ProductReviewQueryFilter filter = new ProductReviewQueryFilter()
            {
                TextSearch = ts,
                PageNumber = null,
                PageSize = null
            };
            var result = await _handler.Filter(filter);

            return Ok(result);
        }
    }
}
