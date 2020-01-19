using Autofac;
using FollowUP.Core.Domain;
using FollowUP.Infrastructure.Services;
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

            builder.RegisterType<Encrypter>()
                   .As<IEncrypter>()
                   .SingleInstance();

            builder.RegisterType<JwtHandler>()
                   .As<IJwtHandler>()
                   .SingleInstance();

            builder.RegisterType<PasswordHasher<User>>()
                   .As<IPasswordHasher<User>>()
                   .SingleInstance();
        }
    }
}
