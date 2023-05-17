using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Common;
using Domain.Entities;
using Infrastructure.Persistence.Businesses.BaseAddress;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    public class AddressController : ApiBaseController
    {
        private readonly IBaseAddressHandler _handler;
        public AddressController(IBaseAddressHandler handler)
        {
            _handler = handler;
        }
        /// <summary>
        /// lay DS thanh pho
        /// </summary>
        /// <returns>Model tài khoản</returns> 
        /// <response code="200">Thành công</response>
        [AllowAnonymous, HttpGet, Route("city")]
        [ProducesResponseType(typeof(ResponseObject<List<City>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCity()
        {

            var result = await _handler.GetCity();
            return Ok(result);
        }
        /// <summary>
        /// lay DS quan huyen
        /// </summary>
        /// <returns>Model tài khoản</returns> 
        /// <response code="200">Thành công</response>
        [AllowAnonymous, HttpGet, Route("district")]
        public async Task<IActionResult> GetDistrict(string matp)
        {
            var result = await _handler.GetDistrictByCity(matp);
            return Ok(result);
        }
        /// <summary>
        /// lay DS xa phuong
        /// </summary>
        /// <returns>Model tài khoản</returns> 
        /// <response code="200">Thành công</response>
        [AllowAnonymous, HttpGet, Route("commune")]
        [ProducesResponseType(typeof(ResponseObject<List<Commune>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCommune(string maqh)
        {
            var result = await _handler.GetCommuneByDistrict(maqh);
            return Ok(result);
        }
    }
}
