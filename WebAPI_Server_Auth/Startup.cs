using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using WebAPI_Server.DB;
using WebAPI_Server.MiddleWares;

namespace WebAPI_Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            
            DBManager.Init(configuration);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseRouting();

            // app.UseCheckUserTokenMiddleWare(); --> MiddlewareExtentions 모든 미들웨어를 보는것이 편하다면 이 방법을 사용
            //                                    --> 이 방법에서는 미들웨어 통과 여부 검사를 Startup이 아닌 각자의 Middleware 스크립트에서 정의
            
            
            app.UseWhen(
                context => context.Request.Path != "/CreateAccount" &&
                                   context.Request.Path != "/Login",
                appbuilder =>
                {
                    appbuilder.UseMiddleware<CheckJwtTokenMiddleWare>();
                }
            );

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}