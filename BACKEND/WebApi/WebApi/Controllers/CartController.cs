using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Common;
using Infrastructure.Persistence.Businesses.Cart;
using Infrastructure.Persistence.Businesses.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    public class CartController : ApiBaseController
    {
        private readonly ICartHandler _handler;
        public CartController(ICartHandler handler)
        {
            _handler = handler;
        }
        /// <summary>
        /// Cập nhật giỏ hàng
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize, HttpPut, Route("")]
        public async Task<IActionResult> Update([FromBody] CartModel model)
        {
            return Ok(await _handler.Update(model.ListProducts));
        }

        /// <summary>
        /// Lấy thông tin sản phẩm giỏ hàng theo id
        /// </summary> 
        /// <returns>Thông tin chi tiết quyền</returns> 
        /// <response code="200">Thành công</response>
        [Authorize, HttpGet, Route("get-by-user-id")]
        public async Task<IActionResult> GetById()
        {
            return Ok(await _handler.GetById());
        }
    }
}
