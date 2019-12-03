using FollowUP.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace FollowUP.Infrastructure.EF
{
    public class FollowUPContext : DbContext
    {
        private readonly SqlSettings _settings;
        public DbSet<User> Users { get; set; }
        public DbSet<InstagramAccount> InstagramAccounts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<ChildComment> ChildComments { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<PromotionComment> PromotionComments { get; set; }
        public DbSet<CompletedMedia> MediaBlacklist { get; set; }
        public DbSet<InstagramProxy> InstagramProxies { get; set; }
        public DbSet<AccountProxy> AccountProxies { get; set; }
        public DbSet<AccountSettings> AccountSettings { get; set; }
        public DbSet<FollowedProfile> FollowedProfiles { get; set; }

        public FollowUPContext(DbContextOptions<FollowUPContext> options, SqlSettings settings) : base(options)
        {
            _settings = settings;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_settings.InMemory)
            {
                optionsBuilder.UseInMemoryDatabase(_settings.ConnectionString);

                return;
            }
            optionsBuilder.UseSqlServer(_settings.ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var userItemBuilder = modelBuilder.Entity<User>();
            userItemBuilder.HasKey(x => x.Id);

            var accountItemBuilder = modelBuilder.Entity<InstagramAccount>();
            accountItemBuilder.HasKey(x => x.Id);

            var commentItemBuilder = modelBuilder.Entity<Comment>();
            commentItemBuilder.HasKey(x => x.Id);

            var childCommentItemBuilder = modelBuilder.Entity<ChildComment>();
            childCommentItemBuilder.HasKey(x => x.Id);

            var promotionItemBuilder = modelBuilder.Entity<Promotion>();
            promotionItemBuilder.HasKey(x => x.Id);

            var promotionCommentItemBuilder = modelBuilder.Entity<PromotionComment>();
            promotionCommentItemBuilder.HasKey(x => x.Id);

            var completedMediaItemBuilder = modelBuilder.Entity<CompletedMedia>();
            completedMediaItemBuilder.HasKey(x => x.Id);

            var instagramProxyItemBuilder = modelBuilder.Entity<InstagramProxy>();
            instagramProxyItemBuilder.HasKey(x => x.Id);

            var accountProxyItemBuilder = modelBuilder.Entity<AccountProxy>();
            accountProxyItemBuilder.HasKey(x => x.Id);

            var accountSettingsItemBuilder = modelBuilder.Entity<AccountSettings>();
            accountSettingsItemBuilder.HasKey(x => x.Id);

            var followedProfileItemBuilder = modelBuilder.Entity<FollowedProfile>();
            followedProfileItemBuilder.HasKey(x => x.Id);
        }
    }
}
