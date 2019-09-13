using Autofac;
using Autofac.Extensions.DependencyInjection;
using FollowUP.Infrastructure.IoC;
using FollowUP.Infrastructure.Services;
using FollowUP.Infrastructure.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Text;

namespace FollowUP
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }
        public IContainer ApplicationContainer { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddAuthorization(x => x.AddPolicy("admin", p => p.RequireRole("admin")));
            services.AddMemoryCache();
            services.AddMvc()
                    .AddJsonOptions(x => x.SerializerSettings.Formatting = Formatting.Indented)
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            var serviceProvider = services.BuildServiceProvider();
            var jwtSettings = serviceProvider.GetService<JwtSettings>();
            services.AddAuthentication().AddJwtBearer(options => {
                options.Audience = "http://localhost:5001/";
                options.Authority = "http://localhost:5000/";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
                };
            });

            var builder = new ContainerBuilder();
            builder.Populate(services);
            builder.RegisterModule(new ContainerModule(Configuration));
            ApplicationContainer = builder.Build();

            return new AutofacServiceProvider(ApplicationContainer);
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

            app.UseAuthentication();
            app.UseHttpsRedirection();

            var generalSettings = app.ApplicationServices.GetService<GeneralSettings>();
            if (generalSettings.SeedData)
            { 
                var dataInitializer = app.ApplicationServices.GetService<IDataInitializer>();
                dataInitializer.SeedAsync();
            }

            app.UseMvc();
        }
    }
}
