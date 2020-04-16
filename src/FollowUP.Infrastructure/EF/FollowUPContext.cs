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
        public DbSet<InstagramProxy> InstagramProxies { get; set; }
        public DbSet<AccountProxy> AccountProxies { get; set; }
        public DbSet<AccountSettings> AccountSettings { get; set; }
        public DbSet<FollowedProfile> FollowedProfiles { get; set; }
        public DbSet<AccountStatistics> AccountStatistics { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<ScheduleDay> ScheduleDays { get; set; }
        public DbSet<ScheduleBatch> ScheduleBatches { get; set; }
        public DbSet<MonthlyDaySchedule> MonthlyDaySchedules { get; set; }
        public DbSet<MonthlyBatchSchedule> MonthlyBatchSchedules { get; set; }
        public DbSet<DayBatch> DayBatches { get; set; }
        public DbSet<DailyPromotionSchedule> DailyPromotionSchedules { get; set; }

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

            var instagramProxyItemBuilder = modelBuilder.Entity<InstagramProxy>();
            instagramProxyItemBuilder.HasKey(x => x.Id);

            var accountProxyItemBuilder = modelBuilder.Entity<AccountProxy>();
            accountProxyItemBuilder.HasKey(x => x.Id);

            var accountSettingsItemBuilder = modelBuilder.Entity<AccountSettings>();
            accountSettingsItemBuilder.HasKey(x => x.Id);

            var followedProfileItemBuilder = modelBuilder.Entity<FollowedProfile>();
            followedProfileItemBuilder.HasKey(x => x.Id);

            var accountStatisticsItemBuilder = modelBuilder.Entity<AccountStatistics>();
            accountStatisticsItemBuilder.HasKey(x => x.Id);

            var refreshTokensItemBuilder = modelBuilder.Entity<RefreshToken>();
            refreshTokensItemBuilder.HasKey(x => x.Id);
            
            var scheduleDaysItemBuilder = modelBuilder.Entity<ScheduleDay>();
            scheduleDaysItemBuilder.HasKey(x => x.Id);

            var scheduleBatchesItemBuilder = modelBuilder.Entity<ScheduleBatch>();
            scheduleBatchesItemBuilder.HasKey(x => x.Id);

            var monthlyDaySchedulesItemBuilder = modelBuilder.Entity<MonthlyDaySchedule>();
            monthlyDaySchedulesItemBuilder.HasKey(x => x.Id);

            var monthlyBatchSchedulesItemBuilder = modelBuilder.Entity<MonthlyBatchSchedule>();
            monthlyDaySchedulesItemBuilder.HasKey(x => x.Id);

            var dayBatchesSchedulesItemBuilder = modelBuilder.Entity<DayBatch>();
            dayBatchesSchedulesItemBuilder.HasKey(x => x.Id);

            var dailyPromotionSchedulesItemBuilder = modelBuilder.Entity<DailyPromotionSchedule>();
            dailyPromotionSchedulesItemBuilder.HasKey(x => x.Id);
        }
    }
}
