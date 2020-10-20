using FollowUP.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Linq;

namespace FollowUP.Infrastructure.EF
{
    public class FollowUPContext : DbContext
    {
        private readonly SqlSettings _settings;
        public DbSet<AccountSettings> AccountSettings { get; set; }
        public DbSet<AccountStatistics> AccountStatistics { get; set; }
        public DbSet<ChildComment> ChildComments { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<DailyPromotionPercentage> DailyPromotionPercentages { get; set; }
        public DbSet<DayGroupConnection> DayGroupConnections { get; set; }
        public DbSet<ExplicitDaySchedule> ExplicitDaySchedules { get; set; }
        public DbSet<FollowedProfile> FollowedProfiles { get; set; }
        public DbSet<FollowPromotion> FollowPromotions { get; set; }
        public DbSet<HashtagPromotion> HashtagPromotions { get; set; }
        public DbSet<InstagramAccount> InstagramAccounts { get; set; }
        public DbSet<InstagramProxy> InstagramProxies { get; set; }
        public DbSet<LocationPromotion> LocationPromotions { get; set; }
        public DbSet<MonthlyGroupSchedule> MonthlyGroupSchedules { get; set; }
        public DbSet<ProfilePromotion> ProfilePromotions { get; set; }
        public DbSet<PromotionComment> PromotionComments { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<ScheduleGroup> ScheduleGroups { get; set; }
        public DbSet<SingleScheduleDay> SingleScheduleDays { get; set; }
        public DbSet<UnfollowPromotion> UnfollowPromotions { get; set; }
        public DbSet<User> Users { get; set; }

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
            // Set the default string length to 256 to avoid nvarchar(MAX)
            foreach (var property in modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(string)))
            {
                property.AsProperty().Builder
                    .HasMaxLength(256, ConfigurationSource.Convention);
            }


            #region AccountSettings

            var accountSettingsItemBuilder = modelBuilder.Entity<AccountSettings>();
            accountSettingsItemBuilder.HasKey(x => x.Id);
            accountSettingsItemBuilder.Property(p => p.InstagramAccountId).IsRequired();
            accountSettingsItemBuilder.Property(p => p.ActionsPerDay).IsRequired();
            accountSettingsItemBuilder.Property(p => p.LikesPerDay).IsRequired();
            accountSettingsItemBuilder.Property(p => p.FollowsPerDay).IsRequired();
            accountSettingsItemBuilder.Property(p => p.UnfollowsPerDay).IsRequired();

            #endregion

            #region AccountStatistics

            var accountStatisticsItemBuilder = modelBuilder.Entity<AccountStatistics>();
            accountStatisticsItemBuilder.HasKey(x => x.Id);
            accountStatisticsItemBuilder.Property(p => p.InstagramAccountId).IsRequired();
            accountStatisticsItemBuilder.Property(p => p.ActionsCount).IsRequired();
            accountStatisticsItemBuilder.Property(p => p.LikesCount).IsRequired();
            accountStatisticsItemBuilder.Property(p => p.FollowsCount).IsRequired();
            accountStatisticsItemBuilder.Property(p => p.UnfollowsCount).IsRequired();
            accountStatisticsItemBuilder.Property(p => p.CreatedAt).IsRequired();

            #endregion

            #region ChildComment

            var childCommentItemBuilder = modelBuilder.Entity<ChildComment>();
            childCommentItemBuilder.HasKey(x => x.Id);
            childCommentItemBuilder.Property(p => p.AuthorPk).IsRequired();
            childCommentItemBuilder.Property(p => p.Author).HasMaxLength(128).IsRequired();
            childCommentItemBuilder.Property(p => p.ProfilePictureUri).HasMaxLength(512);
            childCommentItemBuilder.Property(p => p.Content).HasMaxLength(512).IsRequired();
            childCommentItemBuilder.Property(p => p.LikesCount).IsRequired();
            childCommentItemBuilder.Property(p => p.CommentId).IsRequired();
            childCommentItemBuilder.Property(p => p.CreatedAt).IsRequired();

            #endregion

            #region Comment

            var commentItemBuilder = modelBuilder.Entity<Comment>();

            commentItemBuilder.HasMany(c => c.ChildComments)
                .WithOne(cc => cc.Comment)
                .OnDelete(DeleteBehavior.Cascade);

            commentItemBuilder.HasKey(x => x.Id);
            commentItemBuilder.Property(p => p.InstagramAccountId).IsRequired();
            commentItemBuilder.Property(p => p.AuthorPk).HasMaxLength(15).IsRequired();
            commentItemBuilder.Property(p => p.Author).HasMaxLength(128).IsRequired();
            commentItemBuilder.Property(p => p.ProfilePictureUri).HasMaxLength(512);
            commentItemBuilder.Property(p => p.Content).HasMaxLength(512).IsRequired();
            commentItemBuilder.Property(p => p.LikesCount).IsRequired();
            commentItemBuilder.Property(p => p.ParentMediaId).HasMaxLength(128).IsRequired();
            commentItemBuilder.Property(p => p.ParentImageUri).HasMaxLength(512).IsRequired();
            commentItemBuilder.Property(p => p.CreatedAt).IsRequired();

            #endregion

            #region DailyPromotionPercentage

            var dailyPromotionPercentagesItemBuilder = modelBuilder.Entity<DailyPromotionPercentage>();
            dailyPromotionPercentagesItemBuilder.HasKey(x => x.Id);
            dailyPromotionPercentagesItemBuilder.Property(p => p.SingleScheduleDayId).IsRequired();
            dailyPromotionPercentagesItemBuilder.Property(p => p.PromotionId).IsRequired();
            dailyPromotionPercentagesItemBuilder.Property(p => p.Percentage).IsRequired();

            #endregion

            #region DayGroupConnection

            var dayGroupConnectionsItemBuilder = modelBuilder.Entity<DayGroupConnection>();
            dayGroupConnectionsItemBuilder.HasKey(x => x.Id);
            dayGroupConnectionsItemBuilder.Property(p => p.SingleScheduleDayId).IsRequired();
            dayGroupConnectionsItemBuilder.Property(p => p.ScheduleGroupId).IsRequired();
            dayGroupConnectionsItemBuilder.Property(p => p.Order).IsRequired();

            #endregion

            #region ExplicitDaySchedule

            var explicitDaySchedulesItemBuilder = modelBuilder.Entity<ExplicitDaySchedule>();

            explicitDaySchedulesItemBuilder.HasOne(eds => eds.SingleScheduleDay)
                .WithMany(ssd => ssd.ExplicitDaySchedules)
                .OnDelete(DeleteBehavior.Cascade);

            explicitDaySchedulesItemBuilder.HasKey(x => x.Id);
            explicitDaySchedulesItemBuilder.Property(p => p.SingleScheduleDayId).IsRequired();
            explicitDaySchedulesItemBuilder.Property(p => p.Date).IsRequired();

            #endregion

            #region FollowedProfile

            var followedProfileItemBuilder = modelBuilder.Entity<FollowedProfile>();
            followedProfileItemBuilder.HasKey(x => x.Id);
            followedProfileItemBuilder.Property(p => p.InstagramAccountId).IsRequired();
            followedProfileItemBuilder.Property(p => p.ProfilePk).HasMaxLength(15).IsRequired();

            #endregion

            #region FollowPromotion

            var followPromotionItemBuilder = modelBuilder.Entity<FollowPromotion>();
            followPromotionItemBuilder.HasKey(x => x.Id);
            followPromotionItemBuilder.Property(p => p.Label).HasMaxLength(100).IsRequired();
            followPromotionItemBuilder.Property(p => p.LastMediaId).HasMaxLength(30);
            followPromotionItemBuilder.Property(p => p.NextMinId).HasMaxLength(30);
            followPromotionItemBuilder.Property(p => p.NextMinIdDate);

            #endregion

            #region InstagramAccount

            var accountItemBuilder = modelBuilder.Entity<InstagramAccount>();
            accountItemBuilder.HasMany(ia => ia.ExplicitDaySchedules)
                .WithOne(sg => sg.InstagramAccount)
                .OnDelete(DeleteBehavior.Restrict);
            accountItemBuilder.HasMany(ia => ia.MonthlyGroupSchedules)
                .WithOne(sg => sg.InstagramAccount)
                .OnDelete(DeleteBehavior.Restrict);
            accountItemBuilder.HasMany(ia => ia.ScheduleGroups)
                .WithOne(sg => sg.InstagramAccount)
                .OnDelete(DeleteBehavior.Restrict);
            accountItemBuilder.HasMany(ia => ia.SingleScheduleDays)
                .WithOne(sg => sg.InstagramAccount)
                .OnDelete(DeleteBehavior.Restrict);

            accountItemBuilder.HasMany(ia => ia.AccountStatistics)
                .WithOne(@as => @as.InstagramAccount)
                .OnDelete(DeleteBehavior.Cascade);
            accountItemBuilder.HasOne(ia => ia.AccountSettings)
                .WithOne(@as => @as.InstagramAccount)
                .OnDelete(DeleteBehavior.Cascade);
            accountItemBuilder.HasMany(ia => ia.Comments)
                .WithOne(c => c.InstagramAccount)
                .OnDelete(DeleteBehavior.Cascade);
            accountItemBuilder.HasMany(ia => ia.FollowedProfiles)
                .WithOne(fp => fp.InstagramAccount)
                .OnDelete(DeleteBehavior.Cascade);
            accountItemBuilder.HasMany(ia => ia.FollowPromotions)
                .WithOne(fp => fp.InstagramAccount)
                .OnDelete(DeleteBehavior.Cascade);
            accountItemBuilder.HasMany(ia => ia.PromotionComments)
                .WithOne(pc => pc.InstagramAccount)
                .OnDelete(DeleteBehavior.Cascade);

            accountItemBuilder.HasKey(x => x.Id);
            accountItemBuilder.Property(p => p.Pk).HasMaxLength(15).IsRequired();
            accountItemBuilder.Property(p => p.Username).HasMaxLength(30).IsRequired();
            accountItemBuilder.Property(p => p.Password).HasMaxLength(200).IsRequired();
            accountItemBuilder.Property(p => p.PhoneNumber).HasMaxLength(15);
            accountItemBuilder.Property(p => p.FilePath).HasMaxLength(100).IsRequired();
            accountItemBuilder.Property(p => p.AndroidDevice).HasMaxLength(100).IsRequired();
            accountItemBuilder.Property(p => p.CommentsModuleExpiry).IsRequired();
            accountItemBuilder.Property(p => p.PromotionsModuleExpiry).IsRequired();
            accountItemBuilder.Property(p => p.BannedUntil);
            accountItemBuilder.Property(p => p.ActionCooldown).IsRequired();
            accountItemBuilder.Property(p => p.PreviousCooldownMilliseconds).IsRequired();

            #endregion

            #region InstagramProxy

            var instagramProxyItemBuilder = modelBuilder.Entity<InstagramProxy>();
            instagramProxyItemBuilder.HasKey(x => x.Id);
            instagramProxyItemBuilder.Property(p => p.Ip).HasMaxLength(128).IsRequired();
            instagramProxyItemBuilder.Property(p => p.Port).HasMaxLength(10).IsRequired();
            instagramProxyItemBuilder.Property(p => p.Username).HasMaxLength(128);
            instagramProxyItemBuilder.Property(p => p.Password).HasMaxLength(128);
            instagramProxyItemBuilder.Property(p => p.ExpiryDate).IsRequired();
            instagramProxyItemBuilder.Property(p => p.IsTaken).IsRequired();

            #endregion

            #region MonthlyGroupSchedule

            var monthlyGroupSchedulesItemBuilder = modelBuilder.Entity<MonthlyGroupSchedule>();
            monthlyGroupSchedulesItemBuilder.HasKey(x => x.Id);
            monthlyGroupSchedulesItemBuilder.Property(p => p.ScheduleGroupId).IsRequired();
            monthlyGroupSchedulesItemBuilder.Property(p => p.BeginDate).IsRequired();
            monthlyGroupSchedulesItemBuilder.Property(p => p.EndDate).IsRequired();

            #endregion

            #region PromotionComment

            var promotionCommentItemBuilder = modelBuilder.Entity<PromotionComment>();
            promotionCommentItemBuilder.HasKey(x => x.Id);
            promotionCommentItemBuilder.Property(p => p.InstagramAccountId).IsRequired();
            promotionCommentItemBuilder.Property(p => p.Content).HasMaxLength(512).IsRequired();
            promotionCommentItemBuilder.Property(p => p.CreatedAt).IsRequired();

            #endregion

            #region RefreshToken

            var refreshTokensItemBuilder = modelBuilder.Entity<RefreshToken>();
            refreshTokensItemBuilder.HasKey(x => x.Id);
            refreshTokensItemBuilder.Property(p => p.UserId).IsRequired();
            refreshTokensItemBuilder.Property(p => p.Token).HasMaxLength(1024).IsRequired();
            refreshTokensItemBuilder.Property(p => p.UserAgent).HasMaxLength(1024).IsRequired();
            refreshTokensItemBuilder.Property(p => p.Revoked).IsRequired();

            #endregion

            #region ScheduleGroup

            var scheduleGroupsItemBuilder = modelBuilder.Entity<ScheduleGroup>();

            scheduleGroupsItemBuilder.HasMany(sg => sg.DayGroupConnections)
                .WithOne(dgc => dgc.ScheduleGroup)
                .OnDelete(DeleteBehavior.Cascade);
            scheduleGroupsItemBuilder.HasMany(sg => sg.MonthlyGroupSchedules)
                .WithOne(mgs => mgs.ScheduleGroup)
                .OnDelete(DeleteBehavior.Cascade);

            scheduleGroupsItemBuilder.HasKey(x => x.Id);
            scheduleGroupsItemBuilder.Property(p => p.InstagramAccountId).IsRequired();
            scheduleGroupsItemBuilder.Property(p => p.Name).HasMaxLength(30).IsRequired();
            scheduleGroupsItemBuilder.Property(p => p.Colour).IsRequired();

            #endregion

            #region SingleScheduleDay

            var singleScheduleDaysItemBuilder = modelBuilder.Entity<SingleScheduleDay>();

            singleScheduleDaysItemBuilder.HasMany(ssd => ssd.DayGroupConnections)
                .WithOne(dgc => dgc.SingleScheduleDay)
                .OnDelete(DeleteBehavior.Cascade);
            singleScheduleDaysItemBuilder.HasMany(ssd => ssd.DailyPromotionPercentages)
                .WithOne(dpp => dpp.SingleScheduleDay)
                .OnDelete(DeleteBehavior.Cascade);
            singleScheduleDaysItemBuilder.HasMany(ssd => ssd.ExplicitDaySchedules)
                .WithOne(eds => eds.SingleScheduleDay)
                .OnDelete(DeleteBehavior.Cascade);

            singleScheduleDaysItemBuilder.HasKey(x => x.Id);
            singleScheduleDaysItemBuilder.Property(p => p.InstagramAccountId).IsRequired();
            singleScheduleDaysItemBuilder.Property(p => p.Name).HasMaxLength(30).IsRequired();

            #endregion

            #region User

            var userItemBuilder = modelBuilder.Entity<User>();

            userItemBuilder.HasMany(u => u.InstagramAccounts)
                .WithOne(ia => ia.User)
                .OnDelete(DeleteBehavior.Cascade);
            userItemBuilder.HasMany(u => u.RefreshTokens)
               .WithOne(rt => rt.User)
               .OnDelete(DeleteBehavior.Cascade);

            userItemBuilder.HasKey(x => x.Id);
            // RFC 5322 standard length
            userItemBuilder.Property(p => p.Email).HasMaxLength(320).IsRequired();
            userItemBuilder.Property(p => p.Password).HasMaxLength(200).IsRequired();
            userItemBuilder.Property(p => p.Salt).HasMaxLength(200).IsRequired();
            userItemBuilder.Property(p => p.Username).HasMaxLength(35).IsRequired();
            userItemBuilder.Property(p => p.FullName).HasMaxLength(60);
            userItemBuilder.Property(p => p.Role).HasMaxLength(30).IsRequired();
            userItemBuilder.Property(p => p.Verified).IsRequired();
            userItemBuilder.Property(p => p.CreatedAt).IsRequired();
            userItemBuilder.Property(p => p.UpdatedAt);

            #endregion
        }
    }
}
