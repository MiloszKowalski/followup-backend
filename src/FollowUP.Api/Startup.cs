using Autofac;
using Autofac.Extensions.DependencyInjection;
using FollowUP.Infrastructure.EF;
using FollowUP.Infrastructure.IoC;
using FollowUP.Infrastructure.Services;
using FollowUP.Infrastructure.Services.Background;
using FollowUP.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Logging;
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
            Environment = env;
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }
        public IContainer ApplicationContainer { get; private set; }
        public IHostingEnvironment Environment { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddAuthorization(x => x.AddPolicy("admin", p => p.RequireRole("admin")));
            services.AddMemoryCache();

            services.AddCors(o => o.AddPolicy("FollowUPCorsPolicy", corsBuilder =>
            {
                corsBuilder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
            }));

            services.AddHostedService<CommentsUpdater>();
            services.AddHostedService<Promoter>();

            services.AddMvc()
                    .AddJsonOptions(x => x.SerializerSettings.Formatting = Formatting.Indented)
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new CorsAuthorizationFilterFactory("FollowUPCorsPolicy"));
            });


            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options => {
                options.RequireHttpsMetadata = !Environment.IsDevelopment();
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["jwt:issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["jwt:key"]))
                };
            });

            services.AddEntityFrameworkSqlServer()
                    .AddEntityFrameworkInMemoryDatabase()
                    .AddDbContext<FollowUPContext>();

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
                IdentityModelEventSource.ShowPII = true;
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

            app.UseCors("FollowUPCorsPolicy");

            app.UseMvc();
        }
    }
}
