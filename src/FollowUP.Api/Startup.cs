using Autofac;
using FollowUP.Api.Framework;
using FollowUP.Infrastructure.EF;
using FollowUP.Infrastructure.IoC;
using FollowUP.Infrastructure.Services;
using FollowUP.Infrastructure.Services.Background;
using FollowUP.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Globalization;
using System.Text;

namespace FollowUP
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            Environment = env;
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.secrets.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }
        public IContainer ApplicationContainer { get; private set; }
        public IWebHostEnvironment Environment { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add SendGrid email sender
            services.AddSendGridEmailSender();

            // Add general email template sender
            services.AddEmailTemplateSender();

            // Add middleware to manage JWT
            services.AddTransient<TokenManagerMiddleware>();

            // Add middleware to handle exceptions in requests
            services.AddTransient<ExceptionHandlerMiddleware>();

            services.AddAuthorization(x => x.AddPolicy("admin", p => p.RequireRole("admin")));
            services.AddMemoryCache();
            services.AddDistributedRedisCache(r =>
            { 
                r.Configuration = Configuration["redis:connectionString"];
            });

            services.AddCors(o => o.AddPolicy("FollowUPCorsPolicy", corsBuilder =>
            {
                corsBuilder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
            }));

            if (Configuration["promotion:updateComments"] == "True")
            {
                services.AddHostedService<CommentsUpdater>();
            }

            if (Configuration["promotion:enabled"] == "True")
            {
                services.AddHostedService<PromotionBotSpawner>();
            }

            services.AddMvc()
                    .AddNewtonsoftJson(x =>
                    {
                        var y = x.SerializerSettings;
                        y.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                        y.Culture = new CultureInfo("pl-PL");
                        y.Formatting = Formatting.Indented;
                        y.ContractResolver = new CamelCasePropertyNamesContractResolver();
                        y.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    })
                    .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddAuthentication(options => 
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options => 
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = !Environment.IsDevelopment();
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(Configuration["jwt:key"])
                    ),
                    ValidateIssuer = true,
                    ValidIssuer = Configuration["jwt:issuer"],
                    ValidateAudience = false,
                    RequireExpirationTime = true,
                    ValidateLifetime = true
                };
            });

            services.AddDbContext<FollowUPContext>();

            services.AddControllers();
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new ContainerModule(Configuration));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

            app.UseRouting();
            app.UseCors("FollowUPCorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHttpsRedirection();

            var generalSettings = app.ApplicationServices.GetService<GeneralSettings>();
            if (generalSettings.SeedData)
            { 
                var dataInitializer = app.ApplicationServices.GetService<IDataInitializer>();
                dataInitializer.SeedAsync();
            }

            app.UseCustomExceptionHandler();
            app.UseCustomTokenManager();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
