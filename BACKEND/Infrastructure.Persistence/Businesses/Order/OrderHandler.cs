using Common.CacheService;
using Common.Common;
using Common.Constants;
using Common.Helpers;
using Infrastructure.Persistence.Businesses.Product;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Shared.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Persistence.Businesses.Signalr;
using Microsoft.AspNetCore.SignalR;
using EmailRequest = Infrastructure.Shared.Services.EmailRequest;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Infrastructure.Persistence.Businesses.Order
{
    public class OrderHandler : IOrderHandler
    {
        #region Message
        #endregion
        private readonly IEmailService _emailHandler;
        private readonly string _frontendUrl = Helpers.GetConfig("Url:FrontendUser");
        private const string CachePrefix = "Order";
        private const string SelectItemCacheSubfix = "list-select";
        private const string CodePrefix = "Order.";
        private readonly ApplicationDbContext _dataContext;
        private readonly IHubContext<SignalrHub, IHubClient> _signalrHub;
        private readonly ICacheService _cacheService;
        private readonly IAuthenticatedUserService _authenticatedUserService;
        public OrderHandler(ApplicationDbContext dataContext, ICacheService cacheService, IEmailService emailHandler, IAuthenticatedUserService authenticatedUserService, IHubContext<SignalrHub, IHubClient> signalrHub)
        {
            _emailHandler = emailHandler;
            _dataContext = dataContext;
            _signalrHub = signalrHub;
            _cacheService = cacheService;
            _authenticatedUserService = authenticatedUserService;
        }

        public async Task<Response> Create(OrderCreateModel model)
        {
            try
            {
                #region Check is exist document Type
                var isExistEmail = _dataContext.Orders.Any(c => c.Code == model.Code);
                if (isExistEmail)
                    return new ResponseError(Code.NotFound, "Mã đơn đặt đã tồn tại");

                #endregion
                DateTimeOffset now = DateTime.UtcNow;
                var entity = AutoMapperUtils.AutoMap<OrderCreateModel, Domain.Entities.Order>(model);
                entity.UserId = _authenticatedUserService.UserId;
                entity.Code = Helpers.GenerateAutoCode(CodePrefix, now.ToUnixTimeSeconds());
                var products = JsonConvert.DeserializeObject<List<ProductBaseModel>>(model.ListProducts) ?? new List<ProductBaseModel>();
                entity.Id = Guid.NewGuid();
                await _dataContext.Orders.AddAsync(entity);

                List<ProductSize> productSizes = new List<ProductSize>();
                foreach (var item in products)
                {
                    var productSizeSql = await _dataContext.ProductColors
                        .Where(pc => pc.ProductId == item.ProductId && pc.SizeId == item.SizeId).FirstOrDefaultAsync();
                    var product = await _dataContext.Products.Where(c => c.Id == item.ProductId).FirstOrDefaultAsync();
                    productSizeSql.Quantity -= item.Amount;
                    product.Quantity -= item.Amount;
                    product.Sold += item.Amount;
                    _dataContext.Products.Update(product);
                    productSizes.Add(productSizeSql);
                }
                _dataContext.ProductColors.UpdateRange(productSizes);

                var user = _dataContext.Users.FirstOrDefault(x => x.Id == _authenticatedUserService.UserId);
                if (user != null)
                {
                    user.ListCartJson = string.Empty;
                    _dataContext.Users.Update(user);
                    await _dataContext.SaveChangesAsync();
                }
                #region Gửi email thông báo đến khách hàng

                string title = "[CARA FASHION SHOP] - Order Success";


                StringBuilder htmlBody = new StringBuilder();
                htmlBody.Append("<html><body>");
                htmlBody.Append("<p>Hello <b>" + entity.Name + "</b>,</p>");
                htmlBody.Append("<p>Your Order " + entity.Code + "was successfull!</p>");
                htmlBody.Append(" <table border=1 style='border-collapse: collapse'><tr><th style='padding: 5px'> No </th ><th style='padding: 5px'> Product's name </th ><th style='padding: 5px'> Size </th ><th style='padding: 5px' > Quantity </th ><th style='padding: 5px' > Price </th ></tr >");
                for (int i = 0; i < products.Count; i++)
                {
                    htmlBody.Append("<tr ><td style='padding: 5px'> " + (i + 1) + " </td ><td style='padding: 5px'> " + products[i].Name + " </td ><td style='padding: 5px'> " + products[i].ProductColors[0].Name + " </td ><td style='padding: 5px' >" + products[i].Count + " </td ><td style='padding: 5px'> " +
                                    $"{products[i].AmoutDefault - products[i].DiscountDefault:#,##0.00đ;(#,##0.00đ);Zero}" + " </td ></tr >");
                }
                htmlBody.Append("</table >");
                htmlBody.Append("Total Amount: " + $"{model.GrandTotal:#,##0.00đ;(#,##0.00đ);Zero}");
                htmlBody.Append(
                    "<br>Continue shopping on CARA FASHION SHOP to get more discount products now!");
                htmlBody.Append("<p><a href='" + _frontendUrl + "'><span class='fas fa - phone'></span> <p>CARA<strong> FRASHION SHOP</strong> <span>More beauty than you think!<span></p> </a> </p>");
                htmlBody.Append("</body></html>");

                var emailModel = new EmailRequest()
                {
                    To = model.Email,
                    Subject = title,
                    Body = htmlBody.ToString()
                };
                try
                {
                    _ = Task.Factory.StartNew(() => { _emailHandler.SendAsync(emailModel); });
                }
                catch (Exception)
                {
                    // ignored
                }
                var notify = new Notification()
                {
                    Id = Guid.NewGuid(),
                    CreatedDate = DateTime.Now,
                    IsRead = false,
                    Description = entity.Name + " đã đặt một đơn hàng với giá: " + String.Format("{0:#,##0.00đ;(#,##0.00đ);Zero}", entity.GrandTotal),
                    Title = "Thông báo đơn đặt hàng mới",
                    UserId = model.UserId ?? Guid.Empty
                };
                await _dataContext.Notifications.AddAsync(notify);

                int dbSave = await _dataContext.SaveChangesAsync();
                if (dbSave > 0)
                {
                    var listNotify = from noti in _dataContext.Notifications
                                     where noti.IsRead == false
                                     select (new Notification()
                                     {
                                         Id = noti.Id,
                                         CreatedDate = noti.CreatedDate,
                                         IsRead = noti.IsRead,
                                         Description = noti.Description,
                                         Title = noti.Title,
                                         UserId = noti.UserId
                                     });
                    MessageInstance msg = new MessageInstance()
                    {
                        Timestamp = DateTime.Now.ToString("yyyy-MM-dd"),
                        From = entity.Name,
                        ListNotifications = listNotify.ToList()
                    };
                    await _signalrHub.Clients.All.BroadcastMessage(msg);
                }


                #endregion
                return new ResponseObject<string>(entity.Code, MessageConstants.CreateSuccessMessage, Code.Success);
            }
            catch (Exception ex)
            {
                Log.Error(ex, MessageConstants.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{MessageConstants.CreateErrorMessage} - {ex.Message}");
            }
        }

        public async Task<Response> Update(OrderUpdateModel model)
        {
            try
            {
                #region Check is exist document Type
                var isExistEmail = _dataContext.Orders.Any(c => c.Code == model.Code && c.Id != model.Id);
                if (isExistEmail)
                    return new ResponseError(Code.NotFound, "Mã đơn đặt đã tồn tại");

                #endregion

                var entity = await _dataContext.Orders
                         .FirstOrDefaultAsync(x => x.Id == model.Id);
                //Log.Information("Before Update: " + JsonSerializer.Serialize(entity));

                if (entity != null)
                {
                    var entityUpdate = AutoMapperUtils.AutoMap<OrderUpdateModel, Domain.Entities.Order>(model);
                    _dataContext.Orders.Update(entityUpdate);
                    int dbSave = await _dataContext.SaveChangesAsync();
                    if (dbSave > 0)
                    {

                        return new ResponseObject<Guid>(model.Id, MessageConstants.UpdateSuccessMessage, Code.Success);
                    }
                    return new ResponseError(Code.ServerError, MessageConstants.UpdateErrorMessage);
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

        public async Task<Response> UpdateStatusOrder(OrderUpdateModel model)
        {
            try
            {
                var orderModel = await _dataContext.Orders.Where(x => x.Id == model.Id).FirstOrDefaultAsync();
                if (orderModel != null)
                {
                    orderModel.Status = model.Status;
                    switch (model.Status)
                    {
                        case -1:
                            var products = JsonConvert.DeserializeObject<List<ProductBaseModel>>(orderModel.ListProducts) ?? new List<ProductBaseModel>();
                            List<ProductSize> productSizes = new List<ProductSize>();
                           
                            foreach (var item in products)
                            {
                                var productSizeSql = await _dataContext.ProductColors
                                    .Where(pc => pc.ProductId == item.ProductId && pc.SizeId == item.SizeId).FirstOrDefaultAsync();
                                var product = await _dataContext.Products.Where(c => c.Id == item.ProductId).FirstOrDefaultAsync();
                                productSizeSql.Quantity += item.Amount;
                                product.Quantity += item.Amount;
                                product.Sold -= item.Amount;
                                _dataContext.Products.Update(product);
                                productSizes.Add(productSizeSql);
                            }
                            _dataContext.ProductColors.UpdateRange(productSizes);
                            orderModel.CanceledDate = DateTime.Now;
                            break;
                        case 1:
                            orderModel.ConfirmedDate = DateTime.Now;
                            break;
                        case 2:
                            orderModel.DeliverDate = DateTime.Now;
                            break;
                        case 3:
                            orderModel.RecivedDate = DateTime.Now;
                            break;
                        default:
                            break;
                    }
                    _dataContext.Orders.Update(orderModel);
                    var dbSave = await _dataContext.SaveChangesAsync();
                    var orderBaseModel = AutoMapperUtils.AutoMap<Domain.Entities.Order, OrderModel>(orderModel);
                    if (dbSave > 0)
                    {
                        return new ResponseObject<OrderModel>(orderBaseModel, MessageConstants.GetDataSuccessMessage, Code.Success);
                    }
                    return new ResponseObject<OrderModel>(null, MessageConstants.GetDataErrorMessage, Code.Success);

                }
                return new ResponseObject<OrderModel>(null, MessageConstants.GetDataErrorMessage, Code.Success);
            }
            catch (Exception ex)
            {
                Log.Error(ex, MessageConstants.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{MessageConstants.DeleteErrorMessage} - {ex.Message}");
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
                    var entity = await _dataContext.Orders.FindAsync(item);

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
                        _dataContext.Orders.Remove(entity);
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

        public async Task<Response> Filter(OrderQueryFilter filter)
        {
            try
            {

                var data = (from od in _dataContext.Orders.OrderByDescending(x=>x.LastModified)
                            join city in _dataContext.Cities on od.CityId equals city.matp
                            join dis in _dataContext.Districts on od.DistrictId equals dis.maqh
                            join com in _dataContext.Communes on od.CommuneId equals com.xaid

                            select new OrderBaseModel()
                            {
                                Id = od.Id,
                                AddressDetail = od.AddressDetail,
                                City = city.name,
                                CityId = od.CityId,
                                Code = od.Code,
                                Commune = com.name,
                                CommuneId = od.CommuneId,
                                GrandTotal = od.GrandTotal,
                                CreatedDate = od.Created,
                                CanceledDate = od.CanceledDate,
                                ConfirmedDate = od.ConfirmedDate,
                                DeliverDate = od.DeliverDate,
                                RecivedDate = od.RecivedDate,
                                Description = od.Description,
                                Discount = od.Discount,
                                Status = od.Status,
                                District = dis.name,
                                DistrictId = od.DistrictId,
                                Shipping = od.Shipping,
                                ListProducts = od.ListProducts,
                                UserId = od.UserId,
                                Name = od.Name,
                                PhoneNumber = od.PhoneNumber,
                                Email = od.Email,
                                PhuongThucThanhToan = od.PhuongThucThanhToan,
                            });
                if (!string.IsNullOrEmpty(filter.TextSearch))
                {
                    string ts = filter.TextSearch.Trim().ToLower();
                    data = data.Where(x => x.Name.ToLower().Contains(ts) || x.Code.ToLower().Contains(ts));
                }

                if (filter.Status.HasValue)
                {
                    data = data.Where(x => x.Status == filter.Status);
                }

                if (filter.UserId.HasValue)
                {
                    data = data.Where(x => x.UserId == filter.UserId);
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
                data = data.Skip(excludedRows).Take(filter.PageSize);
                int dataCount = data.Count();

                var listResult = await data.ToListAsync();
                return new ResponseObject<PaginationList<OrderBaseModel>>(new PaginationList<OrderBaseModel>()
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

        public async Task<Response> GetById(Guid id, string ts)
        {
            try
            {
                var rs = await (from od in _dataContext.Orders
                                join ct in _dataContext.Cities on od.CityId equals ct.matp
                                join dt in _dataContext.Districts on od.DistrictId equals dt.maqh
                                join cm in _dataContext.Communes on od.CommuneId equals cm.xaid
                                where od.UserId == id
                                select new OrderBaseModel()
                                {
                                    Id = od.Id,
                                    AddressDetail = od.AddressDetail,
                                    City = ct.name,
                                    CityId = od.CityId,
                                    Code = od.Code,
                                    Commune = cm.name,
                                    CommuneId = od.CommuneId,
                                    CreatedDate = od.Created,
                                    CanceledDate = od.CanceledDate,
                                    ConfirmedDate = od.ConfirmedDate,
                                    DeliverDate = od.DeliverDate,
                                    RecivedDate = od.RecivedDate,
                                    Description = od.Description,
                                    GrandTotal = od.GrandTotal,
                                    Discount = od.Discount,
                                    Status = od.Status,
                                    District = dt.name,
                                    DistrictId = od.DistrictId,
                                    Shipping = od.Shipping,
                                    ListProducts = od.ListProducts,
                                    UserId = od.UserId,
                                    Name = od.Name,
                                    PhoneNumber = od.PhoneNumber,
                                    Email = od.Email,
                                    PhuongThucThanhToan = od.PhuongThucThanhToan,

                                }).OrderByDescending(x => x.CreatedDate).ToListAsync();
                //var entity = await _dataContext.Orders
                //   .Where(x => x.UserId.ToString().ToLower().Trim() == id.ToString().ToLower().Trim()).OrderByDescending(x=>x.CreatedDate).ToListAsync();
                if (!string.IsNullOrEmpty(ts))
                {
                    ts = ts.ToLower().Trim();
                    rs = rs.Where(x => x.Code.ToString().Trim().Contains(ts)).ToList();
                }
                return new ResponseObject<List<OrderBaseModel>>(rs, MessageConstants.GetDataSuccessMessage, Code.Success);
            }
            catch (Exception ex)
            {
                Log.Error(ex, MessageConstants.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{MessageConstants.GetDataErrorMessage} - {ex.Message}");
            }
        }
        /*public async Task<Response> CheckInsurance(string serial)
        {
            try
            {
                var orders = _dataContext.Orders.ToList();
                var orderModel = new Domain.Entities.Order();
                var prodModel = new ProductBaseModel();
                var flag = false;
                if (orders.Count > 0)
                {
                    foreach (var order in orders)
                    {
                        var listProd = JsonConvert.DeserializeObject<List<ProductBaseModel>>(order.ListProducts) ?? new List<ProductBaseModel>();
                        var prod = listProd.FirstOrDefault(x => x.SerialNumber == serial);
                        if (prod != null)
                        {
                            flag = true;
                            orderModel = order;
                            prodModel = prod;
                            break;
                        }
                    }
                }
                if (flag)
                {
                    var orderResult = new InsuranceFilterModel()
                    {
                        CustomerName = orderModel.Name,
                        OrderDate = orderModel.Created,
                        SerialNumber = serial,
                        ProductName = prodModel.Name,
                        InsuranceTime = prodModel.ThoiGianBaoHanh + " " + prodModel.LoaiBaoHanh
                    };
                    return new ResponseObject<InsuranceFilterModel>(orderResult, MessageConstants.GetDataSuccessMessage, Code.Success);
                }
                return new ResponseObject<InsuranceFilterModel>(null, MessageConstants.GetDataSuccessMessage, Code.Success);
            }
            catch (Exception ex)
            {
                Log.Error(ex, MessageConstants.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{MessageConstants.GetDataErrorMessage} - {ex.Message}");
            }
        }*/
    }
}