using Application.Interfaces;
using cm.BackendApi.Helpers;
using Common.Common;
using Domain.Settings;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Businesses.Signalr;
using Infrastructure.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Serilog;
using System;
using System.IO;
using System.Net.Mime;
using System.Text;
using WebApi.Extensions;
using WebApi.Helpers;
using WebApi.Middlewares;
using WebApi.Services;

namespace WebApi
{
    public class Startup
    {
        private readonly string KspSpecificOrigins = "KspSpecificOrigins";
        public IConfiguration Config { get; }
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Config = configuration;
            var builder = StartupHelpers.CreateDefaultConfigurationBuilder(env);

            if (env.IsDevelopment())
            {
                //builder.AddUserSecrets<Startup>();
            }

            Config = builder.Build();
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddCors(options =>
            {
                options.AddPolicy(KspSpecificOrigins,
                builder =>
                {
                    builder.WithOrigins(Config["AllowOrigins"])
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });
            services.AddPersistenceInfrastructure(Config);
            services.AddSharedInfrastructure(Config);
            services.AddSwaggerExtension();
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                           .AddJwtBearer(o =>
                           {
                               o.RequireHttpsMetadata = false;
                               o.SaveToken = true;
                               o.TokenValidationParameters = new TokenValidationParameters
                               {
                                   ValidateIssuerSigningKey = true,
                                   ValidateIssuer = false,
                                   ValidateAudience = false,
                                   ValidateLifetime = false,
                                   ClockSkew = TimeSpan.Zero,
                                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Config["JWTSettings:Key"]))
                               };
                               o.Events = new JwtBearerEvents()
                               {
                                   OnAuthenticationFailed = c =>
                                   {
                                       c.NoResult();
                                       c.Response.StatusCode = 500;
                                       c.Response.ContentType = "text/plain";
                                       return c.Response.WriteAsync(c.Exception.ToString());
                                   },
                                   OnChallenge = context =>
                                   {
                                       context.HandleResponse();
                                       context.Response.StatusCode = 401;
                                       context.Response.ContentType = "application/json";
                                       var result = JsonConvert.SerializeObject(new Response(Code.Unauthorized, "Chưa đăng nhập hoặc không có quyền truy cập"));
                                       return context.Response.WriteAsync(result);
                                   },
                                   OnForbidden = context =>
                                   {
                                       context.Response.StatusCode = 403;
                                       context.Response.ContentType = "application/json";
                                       var result = JsonConvert.SerializeObject(new Response(Code.Forbidden, "Chưa đăng nhập hoặc không có quyền truy cập"));
                                       return context.Response.WriteAsync(result);
                                   },
                               };
                           });
            services.AddAuthorization();
            //services.AddControllers();
            services.AddControllers().ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var result = new ValidationFailedResult(context.ModelState);

                    // TODO: add `using System.Net.Mime;` to resolve MediaTypeNames
                    result.ContentTypes.Add(MediaTypeNames.Application.Json);
                    result.ContentTypes.Add(MediaTypeNames.Application.Xml);
                    return result;
                };
            });
            services.Configure<JwtSettings>(Config.GetSection("JWTSettings"));
            services.AddSignalR(o =>
            {
                o.EnableDetailedErrors = true;
            });
            services.AddApiVersioningExtension();
            services.AddHealthChecks();
            services.AddMemoryCache();
            services.AddTransient<ITokenHelper, TokenHelper>();
            services.AddTransient<IAuthenticatedUserService, AuthenticatedUserService>(); services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
            app.UseCors(x => x
              .SetIsOriginAllowed(origin => true)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseMiddleware<WebSocketsMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Images")),
                RequestPath = new PathString("/Images")
            });
            app.UseSwaggerExtension();
            app.UseErrorHandlingMiddleware();
            app.UseHealthChecks("/health");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<SignalrHub>("/signalr");
            });
            app.UseSerilogRequestLogging();


        }
    }
}
