using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreProfiler.Web;
using GST.Library.Middleware.HttpOverrides;
using GST.Library.Middleware.HttpOverrides.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RequestFilter.Infrastructure;

namespace RequestFilter
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            // IHttpContextAccessor
            services.AddHttpContextAccessor();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // 放在這裡
            app.UseRequestFilter();

            // 如果要自己指定 IP 白名單和限制路徑，可以用以下的方式
            // app.UseRequestFilter(new[] { "127.0.0.1" }, new[] { "Home" });

            app.UseCoreProfiler(true);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseGSTForwardedHeaders(new GST.Library.Middleware.HttpOverrides.Builder.ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XRealIp,
                ForceXForxardedOrRealIp = true,
            });

            // 如果你的專案是放在 Docker Container 裡執行，有可能會抓不到實際的用戶端 IP
            // 就要加入以下的設定內容，以取得 Headers 裡的 X-Forwarded-For 內容和 X-Real-IP
            app.UseGSTForwardedHeaders(new GST.Library.Middleware.HttpOverrides.Builder.ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XRealIp,
                ForceXForxardedOrRealIp = true,
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}