using Autofac;
using FollowUP.Core.Domain;
using FollowUP.Infrastructure.IoC.Modules;
using FollowUP.Infrastructure.Mappers;
using Microsoft.Extensions.Configuration;

namespace FollowUP.Infrastructure.IoC
{
    public class ContainerModule : Autofac.Module
    {
        private readonly IConfiguration _configuration;

        public ContainerModule(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(AutoMapperConfig.Initialize())
                .SingleInstance();
            builder.RegisterModule<CommandModule>();
            builder.RegisterModule<RepositoryModule>();
            builder.RegisterModule<ServiceModule>();
            builder.RegisterModule<PromotionModule>();
            builder.RegisterModule<SqlModule>();
            builder.RegisterModule(new SettingsModule(_configuration));
        }
    }
}
