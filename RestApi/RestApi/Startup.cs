using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestApi.BusinessLayers;
using RestApi.BusinessLayers.Interfaces;
using RestApi.Helpers;
using RestApi.Helpers.Interfaces;
using RestApi.Repositories;
using RestApi.Repositories.Interfaces;
using System.Net.Http;
using HttpClientHandler = RestApi.Helpers.HttpClientHandler;

namespace RestApi
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
            services.AddTransient<HttpClient, HttpClient>();
            services.AddTransient<IHttpClientHandler, HttpClientHandler>();
            services.AddTransient<IHackerNewsRepository, HackerNewsRepository>();
            services.AddSingleton<IHackerNewsBusinessLayer, HackerNewsBusinessLayer>();
            services.AddSingleton<IApiAccessHelper, ApiAccessHelper>();

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins("http://localhost:4200");
                });
            });

            //adds the services Razor Pages and MVC require.
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1); 
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCors();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
