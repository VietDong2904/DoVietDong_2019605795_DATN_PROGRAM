using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Common;
using Common.Constants;
using Domain.Entities;
using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Infrastructure.Persistence.Businesses.BaseAddress
{
   public class BaseAddressHandler : IBaseAddressHandler
    {
        private readonly ApplicationDbContext _dataContext;

        public BaseAddressHandler(ApplicationDbContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<Response> GetCity()
        {
            try
            {
                var cities = await _dataContext.Cities.ToListAsync();
                if (cities.Count > 0)
                {
                    return new ResponseObject<List<City>>(cities, MessageConstants.UpdateSuccessMessage, Code.Success);
                }
                else
                {
                    return new ResponseError(Code.ServerError, MessageConstants.UpdateErrorMessage);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, MessageConstants.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{MessageConstants.GetDataErrorMessage} - {ex.Message}");
            }
        }
        public async Task<Response> GetDistrictByCity(string matp)
        {
            try
            {
                var districts = await _dataContext.Districts.Where(x=> x.matp == matp).ToListAsync();
                if (districts.Count > 0)
                {
                    return new ResponseObject<List<District>>(districts, MessageConstants.UpdateSuccessMessage, Code.Success);
                }
                else
                {
                    return new ResponseError(Code.ServerError, MessageConstants.GetDataErrorMessage);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, MessageConstants.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{MessageConstants.GetDataErrorMessage} - {ex.Message}");
            }
        }
        public async Task<Response> GetCommuneByDistrict(string maqh)
        {
            try
            {
                var communes = await _dataContext.Communes.Where(x=>x.maqh == maqh).ToListAsync();
                if (communes.Count > 0)
                {
                    return new ResponseObject<List<Commune>>(communes, MessageConstants.UpdateSuccessMessage, Code.Success);
                }
                else
                {
                    return new ResponseError(Code.Success, "Không có dữ liệu");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, MessageConstants.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{MessageConstants.GetDataErrorMessage} - {ex.Message}");
            }
        }
    }
}
