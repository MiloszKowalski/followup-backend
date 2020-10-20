using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FollowUP.Infrastructure.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountProxies",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ProxyId = table.Column<Guid>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountProxies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AccountSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: false),
                    ActionsPerDay = table.Column<int>(nullable: false),
                    FollowsPerDay = table.Column<int>(nullable: false),
                    UnfollowsPerDay = table.Column<int>(nullable: false),
                    LikesPerDay = table.Column<int>(nullable: false),
                    MinIntervalMilliseconds = table.Column<int>(nullable: false),
                    MaxIntervalMilliseconds = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AccountStatistics",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: false),
                    ActionsCount = table.Column<int>(nullable: false),
                    LikesCount = table.Column<int>(nullable: false),
                    FollowsCount = table.Column<int>(nullable: false),
                    UnfollowsCount = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountStatistics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChildComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: false),
                    AuthorId = table.Column<long>(nullable: false),
                    Author = table.Column<string>(nullable: true),
                    ProfilePictureUri = table.Column<string>(nullable: true),
                    Content = table.Column<string>(nullable: true),
                    LikesCount = table.Column<int>(nullable: false),
                    ParentCommentId = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChildComments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: false),
                    AuthorId = table.Column<long>(nullable: false),
                    Author = table.Column<string>(nullable: true),
                    ProfilePictureUri = table.Column<string>(nullable: true),
                    Content = table.Column<string>(nullable: true),
                    LikesCount = table.Column<int>(nullable: false),
                    ParentMediaId = table.Column<string>(nullable: true),
                    ParentImageUri = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DailyPromotionPercentages",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ScheduleDayId = table.Column<Guid>(nullable: false),
                    PromotionId = table.Column<Guid>(nullable: false),
                    Percentage = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyPromotionPercentages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DayGroupConnections",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    SingleScheduleDayId = table.Column<Guid>(nullable: false),
                    ScheduleGroupId = table.Column<Guid>(nullable: false),
                    Order = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DayGroupConnections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExplicitDaySchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    SingleScheduleDayId = table.Column<Guid>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExplicitDaySchedules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FollowedProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: false),
                    ProfileId = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FollowedProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InstagramAccounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    Username = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    FilePath = table.Column<string>(nullable: true),
                    AndroidDevice = table.Column<string>(nullable: true),
                    AuthenticationStep = table.Column<int>(nullable: false),
                    CommentsModuleExpiry = table.Column<DateTime>(nullable: false),
                    PromotionsModuleExpiry = table.Column<DateTime>(nullable: false),
                    BannedUntil = table.Column<DateTime>(nullable: false),
                    ActionCooldown = table.Column<DateTime>(nullable: false),
                    PreviousCooldownMilliseconds = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstagramAccounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InstagramProxies",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Ip = table.Column<string>(nullable: true),
                    Port = table.Column<string>(nullable: true),
                    Username = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    ExpiryDate = table.Column<DateTime>(nullable: false),
                    IsTaken = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstagramProxies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MonthlyGroupSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: false),
                    ScheduleGroupId = table.Column<Guid>(nullable: false),
                    BeginDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthlyGroupSchedules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PromotionComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: false),
                    Content = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionComments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Promotions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: false),
                    PromotionType = table.Column<int>(nullable: false),
                    Label = table.Column<string>(nullable: true),
                    NextMinId = table.Column<string>(nullable: true),
                    LastMediaId = table.Column<string>(nullable: true),
                    NextMinIdDate = table.Column<DateTime>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Promotions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    Token = table.Column<string>(nullable: true),
                    UserAgent = table.Column<string>(nullable: true),
                    Revoked = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScheduleGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Colour = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SingleScheduleDays",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SingleScheduleDays", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    Salt = table.Column<string>(nullable: true),
                    Username = table.Column<string>(nullable: true),
                    FullName = table.Column<string>(nullable: true),
                    Role = table.Column<string>(nullable: true),
                    Verified = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountProxies");

            migrationBuilder.DropTable(
                name: "AccountSettings");

            migrationBuilder.DropTable(
                name: "AccountStatistics");

            migrationBuilder.DropTable(
                name: "ChildComments");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "DailyPromotionPercentages");

            migrationBuilder.DropTable(
                name: "DayGroupConnections");

            migrationBuilder.DropTable(
                name: "ExplicitDaySchedules");

            migrationBuilder.DropTable(
                name: "FollowedProfiles");

            migrationBuilder.DropTable(
                name: "InstagramAccounts");

            migrationBuilder.DropTable(
                name: "InstagramProxies");

            migrationBuilder.DropTable(
                name: "MonthlyGroupSchedules");

            migrationBuilder.DropTable(
                name: "PromotionComments");

            migrationBuilder.DropTable(
                name: "Promotions");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "ScheduleGroups");

            migrationBuilder.DropTable(
                name: "SingleScheduleDays");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
