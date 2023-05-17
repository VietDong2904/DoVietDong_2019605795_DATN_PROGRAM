using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Common.CacheService;
using Common.Common;
using Common.Constants;
using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.CompilerServices;
using Serilog;

namespace Infrastructure.Persistence.Businesses.ProductReview
{
    public class ProductReviewHandler : IProductReviewHandler
    {
        #region Message
        #endregion

        private const string CachePrefix = "ProductReview";
        private const string SelectItemCacheSubfix = "list-select";
        private const string CodePrefix = "ProductReview.";
        private readonly ApplicationDbContext _dataContext;
        private readonly ICacheService _cacheService;

        public ProductReviewHandler(ApplicationDbContext dataContext, ICacheService cacheService)
        {
            _dataContext = dataContext;
            _cacheService = cacheService;
        }

        public async Task<Response> Create(ProductReviewCreateModel model)
        {
            try
            {
                var entity = AutoMapperUtils.AutoMap<ProductReviewCreateModel, Domain.Entities.ProductReview>(model);
                entity.Id = Guid.NewGuid();
                await _dataContext.ProductReviews.AddAsync(entity);

                int dbSave = await _dataContext.SaveChangesAsync();

                if (dbSave > 0)
                {
                    //Log.Information("Add success: " + JsonSerializer.Serialize(entity));
                    InvalidCache();

                    return new ResponseObject<Guid>(entity.Id, MessageConstants.CreateSuccessMessage, Code.Success);
                }
                else
                {
                    return new ResponseError(Code.ServerError, MessageConstants.CreateErrorMessage);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, MessageConstants.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{MessageConstants.CreateErrorMessage} - {ex.Message}");
            }
        }

        public async Task<Response> CreateMany(List<ProductReviewCreateModel> list)
        {
            try
            {

                var listId = new List<Guid>();
                var listRS = new List<Domain.Entities.ProductReview>();
                foreach (var item in list)
                {
                    var entity = AutoMapperUtils.AutoMap<ProductReviewCreateModel, Domain.Entities.ProductReview>(item);
                    entity.Id = Guid.NewGuid();
                    await _dataContext.ProductReviews.AddAsync(entity);
                    listId.Add(entity.Id);
                    listRS.Add(entity);
                }

                int dbSave = await _dataContext.SaveChangesAsync();

                if (dbSave > 0)
                {
                    Log.Information("Add success: " + JsonSerializer.Serialize(listRS));
                    InvalidCache();

                    return new ResponseObject<List<Guid>>(listId, MessageConstants.CreateSuccessMessage, Code.Success);
                }
                else
                {
                    return new ResponseError(Code.ServerError, MessageConstants.CreateErrorMessage);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, MessageConstants.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{MessageConstants.CreateErrorMessage} - {ex.Message}");
            }
        }

        public async Task<Response> Update(ProductReviewUpdateModel model)
        {
            try
            {


                var entity = await _dataContext.ProductReviews
                         .FirstOrDefaultAsync(x => x.Id == model.Id);
                //Log.Information("Before Update: " + JsonSerializer.Serialize(entity));

                model.UpdateToEntity(entity);

                _dataContext.ProductReviews.Update(entity);

                int dbSave = await _dataContext.SaveChangesAsync();
                if (dbSave > 0)
                {
                    //Log.Information("After Update: " + JsonSerializer.Serialize(entity));
                    InvalidCache(model.Id.ToString());

                    return new ResponseObject<Guid>(model.Id, MessageConstants.UpdateSuccessMessage, Code.Success);
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
                    var entity = await _dataContext.ProductReviews.FindAsync(item);

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
                        name = entity.Content;
                        _dataContext.ProductReviews.Remove(entity);
                        try
                        {
                            int dbSave = await _dataContext.SaveChangesAsync();
                            if (dbSave > 0)
                            {
                                InvalidCache(item.ToString());

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
                return new ResponseObject<List<ResponeDeleteModel>>(listResult, MessageConstants.DeleteSuccessMessage, Code.Success);

            }
            catch (Exception ex)
            {
                Log.Error(ex, MessageConstants.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{MessageConstants.DeleteErrorMessage} - {ex.Message}");
            }
        }

        public async Task<Response> Filter(ProductReviewQueryFilter filter)
        {
            try
            {
                
                var data = (from c in _dataContext.ProductReviews
                    join pr in _dataContext.Products on c.ProductId equals pr.Id

                    select new ProductReviewBaseModel()
                            {
                                Id = c.Id,
                                ProductId = c.ProductId,
                                ProductName = pr.Name,
                                Rating = c.Rating,
                                IsRating = c.IsRating,
                                Content = c.Content,
                                ParentId = c.ParentId,
                                Status = c.Status,
                                CreatedDate = c.Created
                            });
                var listDefault = data.ToList();
                foreach (var item in listDefault)
                {
                    item.ParentCode = listDefault.Where(x => x.Id == item.ParentId).Select(x => x.Code)
                        .FirstOrDefault();
                }
                if (!string.IsNullOrEmpty(filter.TextSearch))
                {
                    string ts = filter.TextSearch.Trim().ToLower();
                    data = data.Where(x => x.Code.ToLower().Contains(ts) || x.Code.ToLower().Contains(ts));
                }

                if (filter.Status.HasValue)
                {
                    data = data.Where(x => x.Status == filter.Status);
                }

                if (filter.ParentId.HasValue)
                {
                    data = data.Where(x => x.ParentId == filter.ParentId);
                }
                data = data.OrderByDescending(x=>x.CreatedDate);

                int totalCount = data.Count();

                // Pagination
                if (filter.PageSize.HasValue && filter.PageNumber.HasValue)
                {
                    if (filter.PageSize <= 0)
                    {
                        filter.PageSize = 1;
                    }

                    //Calculate nunber of rows to skip on pagesize
                    int excludedRows = (filter.PageNumber.Value - 1) * (filter.PageSize.Value);
                    if (excludedRows <= 0)
                    {
                        excludedRows = 0;
                    }

                    // Query
                    data = data.Skip(excludedRows).Take(filter.PageSize.Value);
                }
                int dataCount = data.Count();

                var listResult = await data.ToListAsync();
                foreach (var item in listResult)
                {
                    foreach (var item1 in listDefault)
                    {
                        if (item.ParentId != Guid.Empty)
                        {
                            if (item.ParentId == item1.Id)
                            {
                                item.ParentCode = item1.Code;
                            }
                        }
                    }
                }
                return new ResponseObject<PaginationList<ProductReviewBaseModel>>(new PaginationList<ProductReviewBaseModel>()
                {
                    DataCount = dataCount,
                    TotalCount = totalCount,
                    PageNumber = filter.PageNumber ?? 0,
                    PageSize = filter.PageSize ?? 0,
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
                var entity = from pr in _dataContext.ProductReviews
                    join user in _dataContext.Users on pr.UserId equals user.Id
                    where pr.ProductId == id
                    select new ProductReviewBaseModel()
                    {
                        Id = pr.Id,
                        Content = pr.Content,
                        CreatedDate = pr.Created,
                        ModifiedDate = pr.LastModified.Value,
                        Rating = pr.Rating,
                        Status = pr.Status,
                        UserId = pr.UserId,
                        ParentId = pr.ParentId,
                        ProductId = pr.ProductId,
                        UserName = user.Name
                    };
                var listResult = await entity.OrderByDescending(x=>x.CreatedDate).ToListAsync();
                return new ResponseObject<List<ProductReviewBaseModel>>(listResult, MessageConstants.GetDataSuccessMessage, Code.Success);
            }
            catch (Exception ex)
            {
                Log.Error(ex, MessageConstants.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{MessageConstants.GetDataErrorMessage} - {ex.Message}");
            }
        }

        private void InvalidCache(string id = "")
        {
            if (!string.IsNullOrEmpty(id))
            {
                string cacheKey = BuildCacheKey(id);
                _cacheService.Remove(cacheKey);
            }

            string selectItemCacheKey = BuildCacheKey(SelectItemCacheSubfix);
            _cacheService.Remove(selectItemCacheKey);
        }

        private string BuildCacheKey(string id)
        {
            return $"{CachePrefix}-{id}";
        }
    }
}