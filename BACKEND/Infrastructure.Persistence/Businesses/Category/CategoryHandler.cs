using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Common.CacheService;
using Common.Common;
using Common.Constants;
using Common.Model;
using Infrastructure.Persistence.Contexts;
using Serilog;
using Domain.Entities;
using Infrastructure.Persistence.Businesses.Product;
using Microsoft.EntityFrameworkCore;
using ResponeDeleteModel = Common.Common.ResponeDeleteModel;

namespace Infrastructure.Persistence.Businesses.Category
{
    public class CategoryHandler : ICategoryHandler
    {
        private readonly ApplicationDbContext _dataContext;
        private readonly ICacheService _cacheService;
        private readonly IProductHandler _productHandler;

        public CategoryHandler(ApplicationDbContext dataContext, ICacheService cacheService, IProductHandler productHandler)
        {
            _productHandler = productHandler;
            _dataContext = dataContext;
            _cacheService = cacheService;
        }

        public async Task<Response> Create(CategoryBaseModel model)
        {
            try
            {
                #region Check is exist
                var isExist = _dataContext.Categories.Any(c => c.Code == model.Code);
                if (isExist)
                    return new ResponseError(Code.NotFound, "Mã loại sản phẩm đã tồn tại");
                #endregion

                var entity = AutoMapperUtils.AutoMap<CategoryBaseModel, Domain.Entities.Category>(model);
                entity.Id = Guid.NewGuid();
                await _dataContext.Categories.AddAsync(entity);

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


        public async Task<Response> Update(CategoryBaseModel model)
        {
            try
            {
                #region Check is exist
                var isExist = _dataContext.Categories.Any(c => c.Code == model.Code && c.Id != model.Id);
                if (isExist)
                    return new ResponseError(Code.NotFound, "Mã loại sản phẩm đã tồn tại");
                #endregion

                var entity = AutoMapperUtils.AutoMap<CategoryBaseModel, Domain.Entities.Category>(model);
                _dataContext.Categories.Update(entity);

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
                    var entity = await _dataContext.Categories.FindAsync(item);

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
                        _dataContext.Categories.Remove(entity);
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

        public async Task<Response> Filter(CategoryFilterModel filter)
        {
            try
            {

                var data = (from c in _dataContext.Categories
                            select new CategoryBaseModel()
                            {
                                Id = c.Id,
                                Code = c.Code,
                                Name = c.Name,
                                ParentId = c.ParentId,
                                IsDisplay = c.IsDisplay
                            }).ToList();
                var listDefault = data;
                if (!string.IsNullOrEmpty(filter.TextSearch))
                {
                    string ts = filter.TextSearch.Trim().ToLower();
                    data = data.Where(x => x.Name.ToLower().Contains(ts) || x.Code.ToLower().Contains(ts)).ToList();
                }
                if (filter.ParentId.HasValue)
                {
                    data = data.Where(x => x.ParentId == filter.ParentId).ToList();
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
                foreach (var item in listResult)
                {
                    foreach (var item1 in listDefault)
                    {
                        if (item.ParentId != Guid.Empty)
                        {
                            if (item.ParentId == item1.Id)
                            {
                                item.ParentName = item1.Name;
                            }
                        }
                    }
                }
                return new ResponseObject<PaginationList<CategoryBaseModel>>(new PaginationList<CategoryBaseModel>()
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
                var entity = await (from c in _dataContext.Categories
                                    where c.Id == id
                                    select new CategoryBaseModel()
                                    {
                                        Id = c.Id,
                                        Code = c.Code,
                                        Name = c.Name,
                                        ParentId = c.ParentId
                                    }).FirstOrDefaultAsync();
                var parent = _dataContext.Categories.FirstOrDefault(x => x.Id == entity.ParentId);
                entity.ParentName = parent?.Name;
                return new ResponseObject<CategoryBaseModel>(entity, MessageConstants.GetDataSuccessMessage, Code.Success);
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
                var data = (from item in _dataContext.Categories.OrderBy(x => x.Created).ThenBy(x => x.Name)
                            where item.IsDisplay
                            select new ComboboxCateogryModel()
                            {
                                Id = item.Id,
                                Title = item.Name,
                                Name = item.Name,
                                Code = item.Code,
                                ParentId = item.ParentId
                            }).ToList();
                foreach (var cate in data)
                {
                    var listProduct = await _productHandler.Filter(new ProductQueryFilter()
                    {
                        CategoryId = cate.Id,
                        IsGetAll = true
                    });
                    cate.Products = listProduct.Data.Data;
                    if (data.FirstOrDefault(x => x.ParentId == cate.Id) == null && cate.ParentId != null)
                    {
                        cate.IsLeaf = true;
                    }
                }
                data = data.Select(c => new ComboboxCateogryModel()
                {
                    Id = c.Id,
                    Products = c.Products,
                    Key = c.Id,
                    Code = c.Code,
                    Name = c.Name,
                    IsLeaf = c.IsLeaf,
                    Count = _dataContext.CategoryProducts.Count(x => x.CategoryId == c.Id),
                    Title = c.Title,
                    ParentId = c.ParentId,
                    Children = GetChildren(data, c.Id)
                }).ToList();
                return new ResponseObject<List<ComboboxCateogryModel>>(data, MessageConstants.GetDataSuccessMessage, Code.Success);
            }
            catch (Exception ex)
            {
                Log.Error(ex, MessageConstants.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{MessageConstants.GetDataErrorMessage} - {ex.Message}");
            }
        }
        public async Task<Response> GetListComboboxNoParent(int count = 0, string textSearch = "")
        {
            try
            {
                var data = (from item in _dataContext.Categories.OrderBy(x => x.Created).ThenBy(x => x.Name)
                            where item.IsDisplay
                            select new ComboboxCateogryModel()
                            {
                                Id = item.Id,
                                Title = item.Name,
                                Name = item.Name,
                                Code = item.Code,
                                ParentId = item.ParentId
                            }).ToList();
                foreach (var cate in data)
                {
                    var listProduct = await _productHandler.Filter(new ProductQueryFilter()
                    {
                        CategoryId = cate.Id,
                        IsGetAll = true
                    });
                    cate.Products = listProduct.Data.Data;
                    if (data.FirstOrDefault(x => x.ParentId == cate.Id) == null && cate.ParentId != null)
                    {
                        cate.IsLeaf = true;
                    }
                }
                data = data.Where(x => x.ParentId == null).Select(c => new ComboboxCateogryModel()
                {
                    Id = c.Id,
                    Products = c.Products,
                    Key = c.Id,
                    Code = c.Code,
                    Name = c.Name,
                    IsLeaf = c.IsLeaf,
                    Count = _dataContext.CategoryProducts.Count(x => x.CategoryId == c.Id),
                    Title = c.Title,
                    ParentId = c.ParentId,
                    Children = GetChildren(data, c.Id)
                }).ToList();
                return new ResponseObject<List<ComboboxCateogryModel>>(data, MessageConstants.GetDataSuccessMessage, Code.Success);
            }
            catch (Exception ex)
            {
                Log.Error(ex, MessageConstants.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{MessageConstants.GetDataErrorMessage} - {ex.Message}");
            }
        }
        public List<ComboboxCateogryModel> GetChildren(List<ComboboxCateogryModel> list, Guid parentId)
        {
            return list
                .Where(t => t.ParentId == parentId)
                .Select(c => new ComboboxCateogryModel
                {
                    Id = c.Id,
                    Key = c.Id,
                    Products = c.Products,
                    Count = _dataContext.CategoryProducts.Count(x => x.CategoryId == c.Id),
                    Code = c.Code,
                    IsLeaf = c.IsLeaf,
                    Name = c.Name,
                    Title = c.Title,
                    ParentId = c.ParentId,
                    Children = GetChildren(list, c.Id)
                })
                .ToList();
        }
    }
}