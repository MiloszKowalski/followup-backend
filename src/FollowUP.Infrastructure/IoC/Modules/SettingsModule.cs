using Autofac;
using FollowUP.Infrastructure.EF;
using FollowUP.Infrastructure.Extensions;
using FollowUP.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;

namespace FollowUP.Infrastructure.IoC.Modules
{
    public class SettingsModule : Autofac.Module
    {
        private readonly IConfiguration _configuration;

        public SettingsModule(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_configuration.GetSettings<GeneralSettings>())
                   .SingleInstance();
            builder.RegisterInstance(_configuration.GetSettings<JwtSettings>())
                   .SingleInstance();
            builder.RegisterInstance(_configuration.GetSettings<SqlSettings>())
                   .SingleInstance();
            builder.RegisterInstance(_configuration.GetSettings<PromotionSettings>())
                   .SingleInstance();
        }
    }
}
