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
        }
    }
}
