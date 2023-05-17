using Application.Interfaces;
using Common.Common;
using Common.Constants;
using Infrastructure.Persistence.Businesses.Product;
using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Businesses.Cart
{
    public class CartHandler : ICartHandler
    {
        private readonly IAuthenticatedUserService _authenticatedUserService;
        private readonly ApplicationDbContext _dataContext;

        public CartHandler(ApplicationDbContext dataContext, IAuthenticatedUserService authenticatedUserService)
        {
            _authenticatedUserService = authenticatedUserService;
            _dataContext = dataContext;
        }
        public async Task<Response> Update(string listProducts)
        {
            try
            {
                var entity = await _dataContext.Users
                         .FirstOrDefaultAsync(x => x.Id == _authenticatedUserService.UserId);
                //Log.Information("Before Update: " + JsonSerializer.Serialize(entity));

                entity.ListCartJson = listProducts;

                _dataContext.Users.Update(entity);

                int dbSave = await _dataContext.SaveChangesAsync();
                if (dbSave > 0)
                {
                    return new ResponseObject<Guid>(entity.Id, MessageConstants.UpdateSuccessMessage, Code.Success);
                }
                return new ResponseError(Code.ServerError, MessageConstants.UpdateErrorMessage);
            }
            catch (Exception ex)
            {
                Log.Error(ex, MessageConstants.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{MessageConstants.GetDataErrorMessage} - {ex.Message}");
            }
        }
        public async Task<Response> GetById()
        {
            try
            {
                List<ProductBaseModel> listCart = new List<ProductBaseModel>();
                var entity = await _dataContext.Users
                    .FirstOrDefaultAsync(x => x.Id == _authenticatedUserService.UserId);
                if (entity == null)
                    return new ResponseObject<List<ProductBaseModel>>(null, MessageConstants.GetDataErrorMessage,
                        Code.ServerError);
                if (!string.IsNullOrEmpty(entity.ListCartJson))
                {
                    listCart = JsonConvert.DeserializeObject<List<ProductBaseModel>>(entity.ListCartJson);
                }
                return new ResponseObject<List<ProductBaseModel>>(listCart, MessageConstants.GetDataSuccessMessage, Code.Success);
            }
            catch (Exception ex)
            {
                Log.Error(ex, MessageConstants.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{MessageConstants.GetDataErrorMessage} - {ex.Message}");
            }
        }

    }
}