using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Common.CacheService;
using Common.Common;
using Common.Constants;
using Common.Model;
using DocumentFormat.OpenXml.Office2010.Excel;
using Domain.Entities;
using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.CompilerServices;
using Serilog;
using ResponeDeleteModel = Common.Common.ResponeDeleteModel;

namespace Infrastructure.Persistence.Businesses.Product
{
    public class ProductHandler : IProductHandler
    {
        #region Message
        #endregion

        private const string CachePrefix = "Product";
        private const string SelectItemCacheSubfix = "list-select";
        private const string CodePrefix = "Product.";
        private const string CodePicturePrefix = "Pic.";
        private const string CodeTagPrefix = "PR_TAG.";
        private const string CodeCategoryPrefix = "PR_CATEGORY.";
        private const string CodeCategoryMetaPrefix = "PR_CATEMETA.";
        private readonly ApplicationDbContext _dataContext;
        private readonly ICacheService _cacheService;

        public ProductHandler(ApplicationDbContext dataContext, ICacheService cacheService)
        {
            _dataContext = dataContext;
            _cacheService = cacheService;
        }
        public async Task<Response> GetListCombobox(int count = 0, string textSearch = "")
        {
            try
            {
                var data = (from item in _dataContext.Products.Where(x => x.Status == true).OrderBy(x => x.Created).ThenBy(x => x.Name)
                            select new ListComboboxModel()
                            {
                                Id = item.Id,
                                Name = item.Name,
                                //Note = item.Code
                            }).AsEnumerable();

                if (!string.IsNullOrEmpty(textSearch))
                {
                    textSearch = textSearch.ToLower().Trim();
                    data = data.Where(x => x.Name.ToLower().Contains(textSearch) || x.Note.ToLower().Contains(textSearch)).ToList();
                }

                if (count > 0)
                {
                    data = data.Take(count).ToList();
                }

                return new ResponseObject<List<ListComboboxModel>>(data.ToList(), MessageConstants.GetDataSuccessMessage, Code.Success);
            }
            catch (Exception ex)
            {
                Log.Error(ex, MessageConstants.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{MessageConstants.GetDataErrorMessage} - {ex.Message}");
            }
        }
        public async Task<Response> Create(ProductBaseModel model)
        {
            try
            {
                #region Check is exist document Type
                /*var isExistEmail = _dataContext.Products.Any(c => c.Code == model.Code);
                if (isExistEmail)
                    return new ResponseError(Code.ServerError, "Mã sản phẩm đã tồn tại");
*/
                #endregion
                /*var isExistSerial = _dataContext.Products.Any(c => c.SerialNumber == model.SerialNumber);
                if (isExistSerial)
                    return new ResponseError(Code.ServerError, "Serial Number đã tồn tại");*/
                var entity = AutoMapperUtils.AutoMap<ProductBaseModel, Domain.Entities.Product>(model);
                entity.Id = Guid.NewGuid();
                entity.VisitCount = 0;
                await _dataContext.Products.AddAsync(entity);
                int dbSave = await _dataContext.SaveChangesAsync();
                if (model.Attachments != null && model.Attachments.Count > 0)
                {
                    foreach (var item in model.Attachments)
                    {
                        var pictureModel = new Attachment();
                        pictureModel.Id = Guid.NewGuid();
                        pictureModel.Status = true;
                        pictureModel.ObjectId = entity.Id;
                        pictureModel.FileName = item;
                        pictureModel.Url = item;
                        await _dataContext.Attachments.AddAsync(pictureModel);
                    }
                }
                if (model.Categories != null && model.Categories.Count > 0)
                {
                    foreach (var item in model.Categories)
                    {
                        var tagModel = new CategoryProduct();
                        tagModel.Id = Guid.NewGuid();
                        tagModel.Status = true;
                        tagModel.ProductId = entity.Id;
                        tagModel.CategoryId = item.Id;
                        await _dataContext.CategoryProducts.AddAsync(tagModel);
                    }
                }
                if (model.ProductColors != null && model.ProductColors.Count > 0)
                {
                    var quantity = 0;
                    foreach (var item in model.ProductColors)
                    {
                        var categoryModel = new ProductSize();

                        categoryModel.Id = Guid.NewGuid();
                        categoryModel.Status = true;
                        categoryModel.SizeId = item.SizeId;
                        categoryModel.ProductId = entity.Id;
                        categoryModel.Price = item.Price;
                        categoryModel.Discount = item.Discount;
                        categoryModel.Quantity = item.Quantity;
                        quantity += categoryModel.Quantity;
                        await _dataContext.ProductColors.AddAsync(categoryModel);
                    }
                    entity.Quantity = quantity;
                    _dataContext.Products.Update(entity);
                }
                if (model.ProductMetas != null && model.ProductMetas.Count > 0)
                {
                    foreach (var item in model.ProductMetas)
                    {
                        var categoryModel = new ProductMeta();

                        categoryModel.Id = Guid.NewGuid();
                        categoryModel.Status = true;
                        categoryModel.Content = item.Content;
                        categoryModel.Key = item.Key;
                        categoryModel.ProductId = entity.Id;
                        await _dataContext.ProductMetas.AddAsync(categoryModel);
                    }
                }

                await _dataContext.SaveChangesAsync();
                if (dbSave > 0)
                {
                    //Log.Information("Add success: " + JsonSerializer.Serialize(entity));

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


        public async Task<Response> Update(ProductBaseModel model)
        {
            try
            {
                #region Check is exist document Type
                /*var isExistEmail = _dataContext.Products.Any(c => c.Code == model.Code && c.Id != model.Id);
                if (isExistEmail)
                    return new ResponseError(Code.NotFound, "Mã Product đã tồn tại");

                var isExistSerial = _dataContext.Products.Any(c => c.SerialNumber == model.SerialNumber && c.Id != model.Id);
                if (isExistSerial)
                    return new ResponseError(Code.ServerError, "Serial Number đã tồn tại");*/
                #endregion
                var entity = await _dataContext.Products
                         .FirstOrDefaultAsync(x => x.Id == model.Id);
                entity = AutoMapperUtils.AutoMap<ProductBaseModel, Domain.Entities.Product>(model);
                _dataContext.Products.Update(entity);
                _dataContext.SaveChanges();
                var listPictureBefore = await _dataContext.Attachments.Where(x => x.ObjectId == entity.Id).ToListAsync();
                _dataContext.Attachments.RemoveRange(listPictureBefore);
                var listColorBefore = await _dataContext.ProductColors.Where(x => x.ProductId == entity.Id).ToListAsync();
                _dataContext.ProductColors.RemoveRange(listColorBefore);
                var listCategoryBefore = await _dataContext.CategoryProducts.Where(x => x.ProductId == entity.Id).ToListAsync();
                _dataContext.CategoryProducts.RemoveRange(listCategoryBefore);
                var listProductMetaBefore = await _dataContext.ProductMetas.Where(x => x.ProductId == entity.Id).ToListAsync();
                _dataContext.ProductMetas.RemoveRange(listProductMetaBefore);
                int db = _dataContext.SaveChanges();
                if (model.Attachments != null && model.Attachments.Count > 0)
                {
                    foreach (var item in model.Attachments)
                    {
                        var pictureModel = new Attachment();
                        pictureModel.Id = Guid.NewGuid();
                        pictureModel.Status = true;
                        pictureModel.ObjectId = entity.Id;
                        pictureModel.FileName = item;
                        pictureModel.Url = item;
                        await _dataContext.Attachments.AddAsync(pictureModel);
                    }
                }
                if (model.Categories != null && model.Categories.Count > 0)
                {
                    foreach (var item in model.Categories)
                    {
                        var tagModel = new CategoryProduct();
                        tagModel.Id = Guid.NewGuid();
                        tagModel.Status = true;
                        tagModel.ProductId = entity.Id;
                        tagModel.CategoryId = item.Id;
                        await _dataContext.CategoryProducts.AddAsync(tagModel);
                    }
                }
                if (model.ProductColors != null && model.ProductColors.Count > 0)
                {
                    var quantity = 0;
                    foreach (var item in model.ProductColors)
                    {
                        var categoryModel = new ProductSize();

                        categoryModel.Id = Guid.NewGuid();
                        categoryModel.Status = true;
                        categoryModel.SizeId = item.SizeId;
                        categoryModel.ProductId = entity.Id;
                        categoryModel.Discount = item.Discount;
                        categoryModel.Price = item.Price;
                        categoryModel.Quantity = item.Quantity;
                        quantity += categoryModel.Quantity;
                        await _dataContext.ProductColors.AddAsync(categoryModel);
                    }
                    entity.Quantity = quantity;
                    _dataContext.Products.Update(entity);
                }
                if (model.ProductMetas != null && model.ProductMetas.Count > 0)
                {
                    foreach (var item in model.ProductMetas)
                    {
                        var categoryModel = new ProductMeta();

                        categoryModel.Id = Guid.NewGuid();
                        categoryModel.Status = true;
                        categoryModel.Content = item.Content;
                        categoryModel.Key = item.Key;
                        categoryModel.ProductId = entity.Id;
                        await _dataContext.ProductMetas.AddAsync(categoryModel);
                    }
                }
                int dbSave = _dataContext.SaveChanges();
                if (dbSave > 0)
                {
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
                    var entity = await _dataContext.Products.FindAsync(item);

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
                        var categoryProduct =
                            _dataContext.CategoryProducts.Where(x => x.ProductId == entity.Id).ToList();
                        _dataContext.CategoryProducts.RemoveRange(categoryProduct);
                        _dataContext.Products.Remove(entity);
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
                return new ResponseObject<List<ResponeDeleteModel>>(listResult, MessageConstants.DeleteSuccessMessage, Code.Success);

            }
            catch (Exception ex)
            {
                Log.Error(ex, MessageConstants.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{MessageConstants.DeleteErrorMessage} - {ex.Message}");
            }
        }

        public async Task<ResponseObject<PaginationList<ProductBaseModel>>> Filter(ProductQueryFilter filter)
        {
            try
            {
                var data = (from c in _dataContext.Products
                            join sp in _dataContext.Suppliers on c.SupplierId equals sp.Id

                            select new ProductBaseModel()
                            {
                                Id = c.Id,
                                Name = c.Name,
                                //Code = c.Code,
                                Attachments = new List<string>(),
                                Title = c.Title,
                                CreatedDate = c.Created,
                                Status = c.Status,
                                //SerialNumber = c.SerialNumber,
                                SupplierName = sp.Name,
                                Summary = c.Summary,
                                //ThoiGianBaoHanh = c.ThoiGianBaoHanh,
                                VisitCount = c.VisitCount,
                                //LoaiBaoHanh = c.LoaiBaoHanh,
                                SupplierId = c.SupplierId,
                                Description = c.Description,
                                //Quantity = c.Quantity
                                Sold = c.Sold,
                                Quantity = _dataContext.ProductColors.Where(x => x.ProductId == c.Id).Select(x => x.Quantity).Sum()

                            });
                //Filter
                //var listFilter = data.Where(x => x.Code != null).ToList();
                var listFilter = data.ToList();
                if (!string.IsNullOrEmpty(filter.TextSearch))
                { string ts = filter.TextSearch.Trim().ToLower();
                    listFilter = listFilter.Where(x => x.Name.ToLower().Contains(ts) || x.Title.ToLower().Contains(ts) || x.Summary.ToLower().Contains(ts)).ToList();
                }
                if (filter.Status.HasValue)
                {
                    listFilter = listFilter.Where(x => x.Status == filter.Status).ToList();
                }

                if (filter.SupplierId.HasValue)
                {
                    listFilter = listFilter.Where(x => x.SupplierId == filter.SupplierId).ToList();
                }
                listFilter = listFilter.OrderByDescending(x => x.CreatedDate).ToList();
                if (filter.MinPrice.HasValue)
                {
                    listFilter = listFilter.Where(x => x.AmoutDefault >= filter.MinPrice).ToList();
                }
                if (filter.MaxPrice.HasValue)
                {
                    listFilter = listFilter.Where(x => x.AmoutDefault <= filter.MaxPrice).ToList();
                }
                int totalCount = listFilter.Count();
                foreach (var item in listFilter)
                {
                    var listRating = await _dataContext.ProductReviews.Where(x => x.ProductId == item.Id && x.IsRating).ToListAsync();
                    if (listRating != null && listRating.Count > 0)
                    {
                        item.Rating = listRating.Select(x => x.Rating).DefaultIfEmpty(0).Average();
                    }
                    item.AmoutDefault = _dataContext.ProductColors.FirstOrDefault(x => x.ProductId == item.Id)?.Price;
                    item.DiscountDefault = _dataContext.ProductColors.FirstOrDefault(x => x.ProductId == item.Id)?.Discount;
                    item.Categories = (from a in _dataContext.Categories
                                       join b in _dataContext.CategoryProducts on a.Id equals b.CategoryId
                                       where b.ProductId == item.Id
                                       select a).ToList();
                    item.ProductColors = (from a in _dataContext.ProductColors.Where(x => x.ProductId == item.Id).ToList()
                                          join b in _dataContext.Size on a.SizeId equals b.Id
                                          select new ProductSizeModel()
                                          {
                                              Name = b.Name,
                                              SizeId = a.SizeId,
                                              ProductId = a.ProductId,
                                              Price = a.Price,
                                              Discount = a.Discount,
                                              Quantity = a.Quantity
                                          }).ToList();
                    item.ProductMetas =
                        _dataContext.ProductMetas.Where(x => x.ProductId == item.Id).ToList();
                    item.Attachments =
                        _dataContext.Attachments.Where(x => x.ObjectId == item.Id).Select(x => x.FileName).ToList();
                    item.CategoryString = string.Join(",", item.Categories.Select(x => x.Name));
                }
                if (filter.CategoryId.HasValue)
                {
                    listFilter = listFilter.Where(x => x.Categories.Any(c => c.Id == filter.CategoryId)).ToList();
                }
                if (filter.SizeId.HasValue)
                {
                    listFilter = listFilter.Where(x => x.ProductColors.Any(c => c.SizeId == filter.SizeId)).ToList();
                }
                if (filter.SortType.HasValue)
                {
                    switch (filter.SortType)
                    {
                        case 0:
                            listFilter = listFilter.OrderByDescending(x => x.CreatedDate).ToList();
                            break;
                        case 1:
                            listFilter = listFilter.OrderBy(x => (x.AmoutDefault - x.DiscountDefault)).ToList();
                            break;
                        case 2:
                            listFilter = listFilter.OrderByDescending(x => (x.AmoutDefault - x.DiscountDefault)).ToList();
                            break;
                        case 3:
                            listFilter = listFilter.OrderByDescending(x => x.VisitCount).ToList();
                            break;
                        case 4:
                            listFilter = listFilter.OrderByDescending(x => x.Rating).ToList();
                            break;
                        case 5:
                            listFilter = listFilter.OrderBy(x => x.Name).ToList();
                            break;
                        case 6:
                            listFilter = listFilter.OrderByDescending(x => (x.DiscountDefault / x.AmoutDefault)).ToList();
                            break;
                        default:
                            listFilter = listFilter.OrderByDescending(x => x.CreatedDate).ToList();
                            break;
                    }
                }
                int dataCount = listFilter.Count();
                // Pagination
                if (!filter.IsGetAll.HasValue)
                {
                    if (filter.PageSize.HasValue && filter.PageNumber.HasValue)
                    {
                        //Calculate nunber of rows to skip on pagesize
                        int excludedRows = (filter.PageNumber.Value - 1) * (filter.PageSize.Value);
                        if (excludedRows <= 0)
                        {
                            excludedRows = 0;
                        }

                        // Query
                        listFilter = listFilter.Skip(excludedRows).Take(filter.PageSize.Value).ToList();
                    }
                }
                return new ResponseObject<PaginationList<ProductBaseModel>>(new PaginationList<ProductBaseModel>()
                {

                    DataCount = dataCount,
                    TotalCount = totalCount,
                    PageNumber = filter.PageNumber ?? 0,
                    PageSize = filter.PageSize ?? 0,
                    Data = listFilter.ToList()
                }, MessageConstants.GetDataSuccessMessage, Code.Success);
            }
            catch (Exception ex)
            {
                Log.Error(ex, MessageConstants.ErrorLogMessage);
                return new ResponseObject<PaginationList<ProductBaseModel>>(null, $"{MessageConstants.GetDataErrorMessage} - {ex.Message}");
            }
        }

        public async Task<ResponseObject<List<ProductBaseModel>>> GetListProductSimilar()
        {
            try
            {
                var data = (from c in _dataContext.Products
                            join sp in _dataContext.Suppliers on c.SupplierId equals sp.Id

                            select new ProductBaseModel()
                            {
                                Id = c.Id,
                                Name = c.Name,
                                //Code = c.Code,
                                Attachments = new List<string>(),
                                Title = c.Title,
                                CreatedDate = c.Created,
                                Status = c.Status,
                                //SerialNumber = c.SerialNumber,
                                SupplierName = sp.Name,
                                Summary = c.Summary,
                                //ThoiGianBaoHanh = c.ThoiGianBaoHanh,
                                VisitCount = c.VisitCount,
                                //LoaiBaoHanh = c.LoaiBaoHanh,
                                SupplierId = c.SupplierId,
                                Description = c.Description,
                                Sold=c.Sold,
                                Quantity = _dataContext.ProductColors.Where(x => x.ProductId == c.Id).Select(x => x.Quantity).Sum()
                            });
                //Filter
                //var listFilter = data.Where(x => x.Code != null).ToList();
                var listFilter = data.ToList();
                foreach (var item in listFilter)
                {
                    var listRating = await _dataContext.ProductReviews.Where(x => x.ProductId == item.Id && x.IsRating).ToListAsync();
                    if (listRating != null && listRating.Count > 0)
                    {
                        item.Rating = listRating.Select(x => x.Rating).DefaultIfEmpty(0).Average();
                    }
                    item.AmoutDefault = _dataContext.ProductColors.FirstOrDefault(x => x.ProductId == item.Id)?.Price;
                    item.DiscountDefault = _dataContext.ProductColors.FirstOrDefault(x => x.ProductId == item.Id)?.Discount;
                    item.Categories = (from a in _dataContext.Categories
                                       join b in _dataContext.CategoryProducts on a.Id equals b.CategoryId
                                       where b.ProductId == item.Id
                                       select a).ToList();
                    item.ProductColors = (from a in _dataContext.ProductColors.Where(x => x.ProductId == item.Id).ToList()
                                          join b in _dataContext.Size on a.SizeId equals b.Id
                                          select new ProductSizeModel()
                                          {
                                              Name = b.Name,
                                              SizeId = a.SizeId,
                                              ProductId = a.ProductId,
                                              Price = a.Price,
                                              Discount = a.Discount,
                                              Quantity = a.Quantity
                                          }).ToList();
                    item.ProductMetas =
                        _dataContext.ProductMetas.Where(x => x.ProductId == item.Id).ToList();
                    item.Attachments =
                        _dataContext.Attachments.Where(x => x.ObjectId == item.Id).Select(x => x.FileName).ToList();
                    item.CategoryString = string.Join(",", item.Categories.Select(x => x.Name));
                }
                return new ResponseObject<List<ProductBaseModel>>(listFilter, MessageConstants.GetDataSuccessMessage, Code.Success);
            }
            catch (Exception ex)
            {
                Log.Error(ex, MessageConstants.ErrorLogMessage);
                return new ResponseObject<List<ProductBaseModel>>(null, $"{MessageConstants.GetDataErrorMessage} - {ex.Message}");
            }
        }

        public async Task<Response> UpdateVisitCount(VisitCountModel model)
        {
            try
            {
                var prodModel = await _dataContext.Products.Where(x => x.Id == model.Id).FirstOrDefaultAsync();
                if (prodModel != null)
                {
                    if (prodModel.VisitCount == null || prodModel.VisitCount < 0)
                    {
                        prodModel.VisitCount = 0;
                    }
                    prodModel.VisitCount++;
                    _dataContext.Products.Update(prodModel);
                }
                int dbSave = await _dataContext.SaveChangesAsync();

                if (dbSave > 0)
                {
                    Log.Information("Update success:");

                    return new ResponseObject<int>(prodModel.VisitCount.Value, MessageConstants.CreateSuccessMessage, Code.Success);
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
        public async Task<Response> GetById(Guid id)
        {
            try
            {
                var vsModel = new VisitCountModel();
                vsModel.Id = id;
                var udRs = await UpdateVisitCount(vsModel);
                var data = (from c in _dataContext.Products
                            join sp in _dataContext.Suppliers on c.SupplierId equals sp.Id
                            where c.Id == id
                            select new ProductBaseModel()
                            {
                                Id = c.Id,
                                Name = c.Name,
                                //Code = c.Code,
                                Attachments = new List<string>(),
                                Title = c.Title,
                                CreatedDate = c.Created,
                                Status = c.Status,
                                //SerialNumber = c.SerialNumber,
                                Summary = c.Summary,
                                //ThoiGianBaoHanh = c.ThoiGianBaoHanh,
                                VisitCount = c.VisitCount,
                                //LoaiBaoHanh = c.LoaiBaoHanh,
                                SupplierId = c.SupplierId,
                                SupplierName = sp.Name,
                                Description = c.Description,
                                Sold = c.Sold,
                                Quantity = _dataContext.ProductColors.Where(x => x.ProductId == id).Select(x => x.Quantity).Sum()
                            }).FirstOrDefault();
                if (data != null)
                {
                    var listRating = await _dataContext.ProductReviews.Where(x => x.ProductId == data.Id && x.IsRating).ToListAsync();
                    if (listRating != null && listRating.Count > 0)
                    {
                        data.Rating = listRating.Select(x => x.Rating).DefaultIfEmpty(0).Average();
                    }
                    data.AmoutDefault = _dataContext.ProductColors.FirstOrDefault(x => x.ProductId == data.Id)?.Price;
                    data.DiscountDefault = _dataContext.ProductColors.FirstOrDefault(x => x.ProductId == data.Id)?.Discount;
                    data.Categories = (from a in _dataContext.Categories
                                       join b in _dataContext.CategoryProducts on a.Id equals b.CategoryId
                                       where b.ProductId == data.Id
                                       select a).ToList();
                    data.ProductColors = (from a in _dataContext.ProductColors.Where(x => x.ProductId == data.Id).ToList()
                                          join b in _dataContext.Size on a.SizeId equals b.Id
                                          select new ProductSizeModel()
                                          {
                                              Name = b.Name,
                                              SizeId = a.SizeId,
                                              ProductId = a.ProductId,
                                              Price = a.Price,
                                              Discount = a.Discount,
                                              Quantity = a.Quantity
                                          }).ToList();
                    data.ProductMetas =
                        _dataContext.ProductMetas.Where(x => x.ProductId == data.Id).ToList();
                    data.Attachments =
                        _dataContext.Attachments.Where(x => x.ObjectId == data.Id).Select(x => x.FileName).ToList();
                    data.CategoryString = string.Join(",", data.Categories.Select(x => x.Name));
                }
                return new ResponseObject<ProductBaseModel>(data, MessageConstants.GetDataSuccessMessage, Code.Success);
            }
            catch (Exception ex)
            {
                Log.Error(ex, MessageConstants.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{MessageConstants.GetDataErrorMessage} - {ex.Message}");
            }
        }
    }
}
