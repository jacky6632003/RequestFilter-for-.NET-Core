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
            // ��b�o��
            app.UseRequestFilter();

            // �p�G�n�ۤv���w IP �զW��M������|�A�i�H�ΥH�U���覡
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

            // �p�G�A���M�׬O��b Docker Container �̰���A���i��|�줣���ڪ��Τ�� IP
            // �N�n�[�J�H�U���]�w���e�A�H���o Headers �̪� X-Forwarded-For ���e�M X-Real-IP
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