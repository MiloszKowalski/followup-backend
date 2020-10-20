using Autofac;
using InstagramApiSharp.API;

namespace FollowUP.Infrastructure.IoC.Modules
{
    public class PromotionModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<InstaApi>()
                   .As<IInstaApi>();
        }
    }
}
