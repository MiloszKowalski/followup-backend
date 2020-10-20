using Autofac;
using FollowUP.Core.Domain;
using FollowUP.Infrastructure.Services;
using FollowUP.Infrastructure.Services.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Reflection;

namespace FollowUP.Infrastructure.IoC.Modules
{
    public class ServiceModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var assembly = typeof(ServiceModule)
                .GetTypeInfo()
                .Assembly;

            builder.RegisterAssemblyTypes(assembly)
                   .Where(x => x.IsAssignableTo<IService>())
                   .AsImplementedInterfaces()
                   .InstancePerLifetimeScope();

            builder.RegisterType<AesEncryptor>()
                   .As<IAesEncryptor>()
                   .SingleInstance();

            builder.RegisterType<Encryptor>()
                   .As<IEncryptor>()
                   .SingleInstance();

            builder.RegisterType<HttpContextAccessor>()
                   .As<IHttpContextAccessor>()
                   .SingleInstance();

            builder.RegisterType<JwtHandler>()
                   .As<IJwtHandler>()
                   .SingleInstance();

            builder.RegisterType<PasswordHasher<User>>()
                   .As<IPasswordHasher<User>>()
                   .SingleInstance();

            builder.RegisterType<InstaActionLogger>()
                   .As<IInstaActionLogger>()
                   .SingleInstance();
        }
    }
}
