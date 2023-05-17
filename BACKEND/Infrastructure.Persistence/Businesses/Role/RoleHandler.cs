using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Common.Common;
using Common.Constants;
using Common.Model;
using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ResponeDeleteModel = Common.Common.ResponeDeleteModel;

namespace Infrastructure.Persistence.Businesses.Role
{
    public class RoleHandler : IRoleHandler
    {
        private readonly ApplicationDbContext _dataContext;

        public RoleHandler(ApplicationDbContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<Response> Create(RoleBaseModel model)
        {
            try
            {
                #region Check is exist
                var isExist = _dataContext.Roles.Any(c => c.Code == model.Code);
                if (isExist)
                    return new ResponseError(Code.NotFound, "Mã quyền đã tồn tại");
                #endregion

                var entity = AutoMapperUtils.AutoMap<RoleBaseModel, Domain.Entities.Role>(model);
                entity.Id = Guid.NewGuid();
                await _dataContext.Roles.AddAsync(entity);

                int dbSave = await _dataContext.SaveChangesAsync();

                if (dbSave > 0)
                {
                    return new ResponseObject<Guid>(entity.Id, "Thêm mới thành công", Code.Success);
                }
                return new ResponseError(Code.ServerError, "Thêm mới thất bại");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return new ResponseError(Code.ServerError, ex.Message);
            }
        }


        public async Task<Response> Update(RoleBaseModel model)
        {
            try
            {
                #region Check is exist
                var isExist = _dataContext.Roles.Any(c => c.Code == model.Code && c.Id != model.Id);
                if (isExist)
                    return new ResponseError(Code.NotFound, "Mã quyền đã tồn tại");
                #endregion

                var entity = AutoMapperUtils.AutoMap<RoleBaseModel, Domain.Entities.Role>(model);
                _dataContext.Roles.Update(entity);

                int dbSave = await _dataContext.SaveChangesAsync();

                if (dbSave > 0)
                {
                    return new ResponseObject<Guid>(entity.Id, "Cập nhật thành công", Code.Success);
                }
                return new ResponseError(Code.ServerError, "Cập nhật thất bại");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return new ResponseError(Code.ServerError, ex.Message);
            }
        }

        public async Task<Response> Delete(List<Guid> listId)
        {
            try
            {
                var listResult = new List<ResponeDeleteModel>();
                var name = "";
                Log.Information("List Delete: " + JsonSerializer.Serialize(listId));
                foreach (var item in listId)
                {
                    name = "";
                    var entity = await _dataContext.Roles.FindAsync(item);

                    if (entity == null)
                    {
                        listResult.Add(new ResponeDeleteModel()
                        {
                            Id = item,
                            Name = name,
                            Result = false,
                            Message = MessageConstants.DeleteItemNotFoundMessage
                        });
                    }
                    else
                    {
                        name = entity.Name;
                        _dataContext.Roles.Remove(entity);
                        try
                        {
                            int dbSave = await _dataContext.SaveChangesAsync();
                            if (dbSave > 0)
                            {
                                listResult.Add(new ResponeDeleteModel()
                                {
                                    Id = item,
                                    Name = name,
                                    Result = true,
                                    Message = MessageConstants.DeleteItemSuccessMessage
                                });
                            }
                            else
                            {
                                listResult.Add(new ResponeDeleteModel()
                                {
                                    Id = item,
                                    Name = name,
                                    Result = false,
                                    Message = MessageConstants.DeleteItemErrorMessage
                                });
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, MessageConstants.ErrorLogMessage);
                            listResult.Add(new ResponeDeleteModel()
                            {
                                Id = item,
                                Name = name,
                                Result = false,
                                Message = ex.Message
                            });
                        }
                    }
                }
                Log.Information("List Result Delete: " + JsonSerializer.Serialize(listResult));
                return new ResponseObject<List<ResponeDeleteModel>>(listResult, "Xóa thành công", Code.Success);

            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return new ResponseError(Code.ServerError, ex.Message);
            }
        }

        public async Task<Response> Filter(RoleFilterModel filter)
        {
            try
            {

                var data = (from c in _dataContext.Roles
                            select new RoleBaseModel()
                            {
                                Id = c.Id,
                                Code = c.Code,
                                Name = c.Name,
                                Status = c.Status
                            }).ToList();
                if (!string.IsNullOrEmpty(filter.TextSearch))
                {
                    string ts = filter.TextSearch.Trim().ToLower();
                    data = data.Where(x => x.Name.ToLower().Contains(ts) || x.Code.ToLower().Contains(ts)).ToList();
                }

                if (filter.Status.HasValue)
                {
                    data = data.Where(x => x.Status == filter.Status).ToList();
                }
                int totalCount = data.Count();

                // Pagination
                //Calculate nunber of rows to skip on pagesize
                int excludedRows = (filter.PageNumber - 1) * (filter.PageSize);
                if (excludedRows <= 0)
                {
                    excludedRows = 0;
                }

                // Query
                if (!filter.IsGetAll.HasValue)
                {
                    data = data.Skip(excludedRows).Take(filter.PageSize).ToList();
                }
                int dataCount = data.Count();
                var listResult = data;
                return new ResponseObject<PaginationList<RoleBaseModel>>(new PaginationList<RoleBaseModel>()
                {
                    DataCount = dataCount,
                    TotalCount = totalCount,
                    PageNumber = filter.PageNumber,
                    PageSize = filter.PageSize,
                    Data = listResult
                }, MessageConstants.GetDataSuccessMessage, Code.Success);
            }
            catch (Exception ex)
            {
                Log.Error(ex, MessageConstants.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{MessageConstants.GetDataErrorMessage} - {ex.Message}");
            }
        }

        public async Task<Response> GetById(Guid id)
        {
            try
            {
                var entity = await (from c in _dataContext.Roles
                                    where c.Id == id
                                    select new RoleBaseModel()
                                    {
                                        Id = c.Id,
                                        Code = c.Code,
                                        Name = c.Name,
                                        Status = c.Status
                                    }).FirstOrDefaultAsync();
                return new ResponseObject<RoleBaseModel>(entity, MessageConstants.GetDataSuccessMessage, Code.Success);
            }
            catch (Exception ex)
            {
                Log.Error(ex, MessageConstants.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{MessageConstants.GetDataErrorMessage} - {ex.Message}");
            }
        }

        public async Task<Response> GetListCombobox(int count = 0, string textSearch = "")
        {
            try
            {
                var data = await (from item in _dataContext.Roles.OrderBy(x => x.Created).ThenBy(x => x.Name)
                                  select new ListComboboxModel()
                                  {
                                      Id = item.Id,
                                      Name = item.Name,
                                      Code = item.Code
                                  }).ToListAsync();

                if (!string.IsNullOrEmpty(textSearch))
                {
                    textSearch = textSearch.ToLower().Trim();
                    data = data.Where(x => x.Name.ToLower().Contains(textSearch) || x.Code.ToLower().Contains(textSearch)).ToList();
                }

                if (count > 0)
                {
                    data = data.Take(count).ToList();
                }

                return new ResponseObject<List<ListComboboxModel>>(data, MessageConstants.GetDataSuccessMessage, Code.Success);
            }
            catch (Exception ex)
            {
                Log.Error(ex, MessageConstants.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{MessageConstants.GetDataErrorMessage} - {ex.Message}");
            }
        }
    }
}