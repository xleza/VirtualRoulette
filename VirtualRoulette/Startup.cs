using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using VirtualRoulette.Common;
using VirtualRoulette.Filters;
using VirtualRoulette.Persistence;
using VirtualRoulette.Security;
using VirtualRoulette.Services;

namespace VirtualRoulette
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var authConfig = new AuthConfig();
            _configuration.GetSection("AuthConfig").Bind(authConfig);

            services.AddTransient<IDbHelper, MySqlDbHelper>();
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<ISecurityService, SecurityService>();
            services.AddTransient<UsersCommandService>();
            services.AddHostedService<ConsumeScopedServiceHostedService>();
            services.AddScoped<IScopedProcessingService, JackpotIncrementerService>();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = Constants.CookiesKey;
            }).AddCookie(Constants.CookiesKey, options =>
            {
                options.Cookie.Name = "auth_cookie";
                options.Cookie.SameSite = SameSiteMode.None;
                options.Cookie.Expiration = TimeSpan.FromMinutes(authConfig.ExpirationInMinutes);
                options.ExpireTimeSpan = TimeSpan.FromMinutes(authConfig.IdleTimeoutInMinutes);
                options.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = redirectContext =>
                    {
                        redirectContext.HttpContext.Response.StatusCode = 401;
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddMvc(options => options.Filters.Add(typeof(CustomExceptionFilterAttribute)))
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "VirtualRoulette",
                    Description = "Test Project",
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();
            app.UseMvc();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "VirtualRoulette V1");
            });
        }
    }
}
