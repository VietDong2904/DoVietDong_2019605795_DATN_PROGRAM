using Common.CacheService;
using Infrastructure.Persistence.Businesses.Account;
using Infrastructure.Persistence.Businesses.BaseAddress;
using Infrastructure.Persistence.Businesses.Cart;
using Infrastructure.Persistence.Businesses.Category;
using Infrastructure.Persistence.Businesses.Color;
using Infrastructure.Persistence.Businesses.Order;
using Infrastructure.Persistence.Businesses.Product;
using Infrastructure.Persistence.Businesses.ProductReview;
using Infrastructure.Persistence.Businesses.Role;
using Infrastructure.Persistence.Businesses.Supplier;
using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence
{
    public static class ServiceRegistration
    {
        public static void AddPersistenceInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseInMemoryDatabase("ApplicationDb"));
            }
            else
            {
                services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlServer(
                   configuration.GetConnectionString("DefaultConnection"),
                   b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
            }
            #region Repositories
            services.AddTransient<ICategoryHandler, CategoryHandler>();
            services.AddTransient<IAccountHandler, AccountHandler>();
            services.AddTransient<IColorHandler, ColorHandler>();
            services.AddTransient<ISupplierHandler, SupplierHandler>();
            services.AddTransient<IRoleHandler, RoleHandler>();
            services.AddTransient<IProductHandler, ProductHandler>();
            services.AddTransient<IOrderHandler, OrderHandler>();
            services.AddTransient<IProductReviewHandler, ProductReviewHandler>();
            services.AddTransient<ICartHandler, CartHandler>();
            services.AddTransient<IBaseAddressHandler, BaseAddressHandler>();
            #endregion

            #region Services
            services.AddTransient<ICacheService, InMemoryCacheService>();
            #endregion
        }
    }
}
