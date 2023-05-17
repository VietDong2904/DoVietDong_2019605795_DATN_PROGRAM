using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Common;
using Domain.Entities;
using Infrastructure.Persistence.Businesses.Product;
using Infrastructure.Persistence.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace WebApi.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{api-version:apiVersion}/dashboard")]
    [ApiExplorerSettings(GroupName = "Quản lý")]
    public class DashboardController : ApiBaseController
    {
        private readonly IProductHandler _handler;
        private ApplicationDbContext _dataContext;
        public DashboardController(IProductHandler handler, ApplicationDbContext dataContext)
        {
            _dataContext = dataContext;
            _handler = handler;
        }

        [AllowAnonymous, HttpGet, Route("")]
        [ProducesResponseType(typeof(ResponseObject<DashboardModel>), StatusCodes.Status200OK)]
        public async Task<DashboardModel> GetDataDashboard(string year = "")
        {

            var listOrder = await _dataContext.Orders.ToListAsync();
            var listOrdProd = new List<OrderProduct>();
            foreach (var item in listOrder)
            {
                var products = JsonConvert.DeserializeObject<List<ProductBaseModel>>(item.ListProducts) ?? new List<ProductBaseModel>();
                listOrdProd.AddRange(products.Select(x=> new OrderProduct() {
                    Id = Guid.NewGuid(),
                    ProductId = x.Id,
                    OrderId = item.Id,
                    Price = x.AmoutDefault.Value,
                    Discount = x.DiscountDefault.Value,
                    Quantity = x.Count
                }));
            }
            var listProd = listOrdProd.GroupBy(x => x.ProductId).Select(x => new ProductOrderModel()
            {
                ProductId = x.First().ProductId,
                Quantity = x.Count()
            }).ToList();
            var listRpYear = listOrder.Where(x => x.Status == 3 || x.Status == 5).GroupBy(x => x.Created.Year)
                .Select(od => new ReportYearModel()
                {
                    Year = od.First().Created.Year,
                    Total = od.Sum(c => c.GrandTotal.Value)
                }).ToList();
            if (string.IsNullOrEmpty(year))
            {
                year = DateTime.Now.Year.ToString();
            }
            var orderRp = new OrderReport()
            {
                OrderReceived = listOrder.Count,
                OrderDelivered = listOrder.Where(x => x.Status == 3 || x.Status == 5).ToList().Count,
                OrderCancelled = listOrder.Where(x => x.Status == -1).ToList().Count,
                OrderReceivedMonth = listOrder.Where(x =>
                        x.Created.Month == DateTime.Now.Month && x.Created.Year == DateTime.Now.Year).ToList()
                    .Count,
                OrderReceivedToday = listOrder.Where(x =>
                    x.Created.Day == DateTime.Now.Day && x.Created.Year == DateTime.Now.Year &&
                    x.Created.Month == DateTime.Now.Month).ToList().Count,
                OrderNotProcess = listOrder.Where(x => x.Status == 0).ToList().Count,
                OrderNotProcessToday = listOrder.Where(x =>
                    x.Status == 0 && x.Created.Day == DateTime.Now.Day && x.Created.Year == DateTime.Now.Year &&
                    x.Created.Month == DateTime.Now.Month).ToList().Count,
                GrandTotal = listOrder.Where(x => x.Status != -1).ToList().Select(x => x.GrandTotal.Value).Sum(),
                GrandTotalDelivered = listOrder.Where(x => x.Status == 3 || x.Status == 5).ToList().Select(x => x.GrandTotal.Value).Sum(),
            };
            var model = new DashboardModel()
            {
                ReportYearModel = listRpYear.Where(x => x.Year >= (DateTime.Now.Year - 5)).ToList(),
                OrderReport = orderRp,
                ProductOrderModels = listProd.OrderByDescending(x=>x.Quantity).ToList()
            };
            return model;
        }

        [AllowAnonymous, HttpGet, Route("report-month-in-year")]
        [ProducesResponseType(typeof(ResponseObject<DashboardModel>), StatusCodes.Status200OK)]
        public async Task<DashboardModel> GetReportYear(string year = "")
        {
            var listOrder = await _dataContext.Orders.ToListAsync();
            if (string.IsNullOrEmpty(year))
            {
                year = DateTime.Now.Year.ToString();
            }
            var listRpMonth = listOrder.Where(x => x.Status == 3 || x.Status == 5).GroupBy(x => new { x.Created.Year, x.Created.Month }).Select(od => new ReportMonthModel()
            {
                Year = od.First().Created.Year,
                Month = od.First().Created.Month,
                Total = od.Sum(c => c.GrandTotal.Value)
            }).ToList();
            var model = new DashboardModel()
            {
                ReportMonthModel = listRpMonth.Where(x => x.Year.ToString() == year).ToList()
            };
            return model;
        }
    }
    public class DashboardModel
    {
        public OrderReport OrderReport { get; set; }
        public List<ProductOrderModel> ProductOrderModels { get; set; }
        public List<ReportMonthModel> ReportMonthModel { get; set; }
        public List<ReportYearModel> ReportYearModel { get; set; }
    }

    public class OrderReport
    {
        public int OrderReceived { get; set; }
        public int OrderDelivered { get; set; }
        public int OrderCancelled { get; set; }
        public int OrderReceivedMonth { get; set; }
        public int OrderReceivedToday { get; set; }
        public int OrderNotProcess { get; set; }
        public int OrderNotProcessToday { get; set; }
        public double GrandTotal { get; set; }
        public double GrandTotalDelivered { get; set; }
    }
    public class VisitWebsiteResponse
    {
        public int VisitWebsiteToday { get; set; }
        public int VisitWebsite { get; set; }
    }
    public class ReportYearModel
    {
        public int Year { get; set; }
        public double Total { get; set; }
    }
    public class ProductOrderModel
    {
        public int Quantity { get; set; }
        public Guid ProductId { get; set; }
    }
    public class ReportMonthModel
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public double Total { get; set; }
    }
}
