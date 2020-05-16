using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;
using OnlineShoppingBackend.Utils;

namespace OnlineShoppingBackend
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(GlobalExceptionFilter)); // 发生异常时返回自定义结果并记录日志
            }).AddJsonOptions(options =>
            {
                // 设置默认序列化规则
                options.SerializerSettings.ContractResolver = new SnakeCaseNamingContractResolver();
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);


            // redis
            services.AddDistributedRedisCache(options =>
            {
                options.InstanceName = Configuration["AppSettings:Redis:InstanceName"];
                options.Configuration = Configuration["AppSettings:Redis:ConnectionString"];
            });

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromDays(30); // session活期时间
                options.Cookie.HttpOnly = true; // 设为httponly
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder =>
                        builder.AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowCredentials()
                                .SetIsOriginAllowed(arg => true)
                );
            });

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true; // 禁用模型验证自动返回 400 错误
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSession();
            app.UseCors("AllowSpecificOrigin");
            app.UseMvc();
        }
    }
}
