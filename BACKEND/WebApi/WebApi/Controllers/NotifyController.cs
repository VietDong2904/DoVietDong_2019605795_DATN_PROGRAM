using Common.Common;
using Common.Constants;
using Domain.Entities;
using Infrastructure.Persistence.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Controllers;

namespace DigitalID.API
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{api-version:apiVersion}/notification")]
    [ApiExplorerSettings(GroupName = "Thông báo")]
    public class NotifyController : ApiBaseController
    {
        private readonly ApplicationDbContext _dataContext;
        public NotifyController(ApplicationDbContext dataContext)
        {
            _dataContext = dataContext;
        }
        /// <summary>
        /// Lấy danh sách thông báo
        /// </summary> 
        /// <param name="count">số bản ghi tối đa</param>
        /// <param name="ts">Từ khóa tìm kiếm</param>
        /// <returns>Danh sách loại sản phẩm</returns> 
        /// <response code="200">Thành công</response>
        [Authorize, HttpGet, Route("")]
        [ProducesResponseType(typeof(ResponseObject<List<Notification>>), StatusCodes.Status200OK)]
        public async Task<Response> GetListNotify(int count = 0, string ts = "")
        {
            try
            {
                var listNotify = from noti in _dataContext.Notifications
                    select (new Notification()
                    {
                        Id = noti.Id,
                        CreatedDate = noti.CreatedDate,
                        IsRead = noti.IsRead,
                        Description = noti.Description,
                        Title = noti.Title,
                        UserId = noti.UserId
                    });
                return new ResponseObject<List<Notification>> (listNotify.OrderByDescending(x => x.CreatedDate).ToList(), MessageConstants.CreateSuccessMessage, Code.Success);
            }
            catch (Exception e)
            {
                return new ResponseObject<List<Notification>>(null, MessageConstants.GetDataErrorMessage, Code.ServerError);
            }
        }

        /// <summary>
        /// Cập nhật trạng thái thông báo
        /// </summary> 
        /// <param name="count">số bản ghi tối đa</param>
        /// <param name="ts">Từ khóa tìm kiếm</param>
        /// <returns>Danh sách loại sản phẩm</returns> 
        /// <response code="200">Thành công</response>
        [Authorize, HttpPost, Route("update")]
        [ProducesResponseType(typeof(ResponseObject<bool>), StatusCodes.Status200OK)]
        public async Task<Response> UpdateStatus(Notification model)
        {
            try
            {
                var notify = await _dataContext.Notifications.Where(x => x.Id == model.Id).FirstOrDefaultAsync();
                notify.IsRead = true;
                _dataContext.Notifications.Update(notify);
                var dbSave = await _dataContext.SaveChangesAsync();
                if (dbSave > 0)
                {
                    return new ResponseObject<bool>(true, MessageConstants.CreateSuccessMessage, Code.Success);
                }
                return new ResponseObject<bool>(false, MessageConstants.GetDataErrorMessage, Code.ServerError);

            }
            catch (Exception e)
            {
                return new ResponseObject<bool>(false, MessageConstants.GetDataErrorMessage, Code.ServerError);
            }
        }
    }
}
