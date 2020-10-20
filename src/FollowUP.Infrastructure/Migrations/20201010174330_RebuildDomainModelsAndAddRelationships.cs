using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FollowUP.Infrastructure.Migrations
{
    public partial class RebuildDomainModelsAndAddRelationships : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DailyPromotionPercentages_Promotions_PromotionId",
                table: "DailyPromotionPercentages");

            migrationBuilder.DropTable(
                name: "AccountProxies");

            migrationBuilder.DropTable(
                name: "Promotions");

            migrationBuilder.DropIndex(
                name: "IX_DailyPromotionPercentages_PromotionId",
                table: "DailyPromotionPercentages");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "ChildComments");

            migrationBuilder.RenameColumn(
                name: "AccountId",
                table: "SingleScheduleDays",
                newName: "InstagramAccountId");

            migrationBuilder.RenameColumn(
                name: "AccountId",
                table: "ScheduleGroups",
                newName: "InstagramAccountId");

            migrationBuilder.RenameColumn(
                name: "AccountId",
                table: "PromotionComments",
                newName: "InstagramAccountId");

            migrationBuilder.RenameColumn(
                name: "AccountId",
                table: "MonthlyGroupSchedules",
                newName: "InstagramAccountId");

            migrationBuilder.RenameColumn(
                name: "AccountId",
                table: "FollowedProfiles",
                newName: "InstagramAccountId");

            migrationBuilder.RenameColumn(
                name: "AccountId",
                table: "ExplicitDaySchedules",
                newName: "InstagramAccountId");

            migrationBuilder.RenameColumn(
                name: "AccountId",
                table: "Comments",
                newName: "InstagramAccountId");

            migrationBuilder.RenameColumn(
                name: "ParentCommentId",
                table: "ChildComments",
                newName: "CommentId");

            migrationBuilder.RenameColumn(
                name: "AccountId",
                table: "AccountStatistics",
                newName: "InstagramAccountId");

            migrationBuilder.RenameColumn(
                name: "AccountId",
                table: "AccountSettings",
                newName: "InstagramAccountId");

            migrationBuilder.AddColumn<Guid>(
                name: "InstagramAccountId",
                table: "InstagramProxies",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "InstagramAccounts",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddColumn<Guid>(
                name: "FollowPromotionId",
                table: "DailyPromotionPercentages",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UnfollowPromotionId",
                table: "DailyPromotionPercentages",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FollowPromotions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Label = table.Column<string>(maxLength: 100, nullable: false),
                    LastMediaId = table.Column<string>(maxLength: 30, nullable: true),
                    NextMinId = table.Column<string>(maxLength: 30, nullable: true),
                    NextMinIdDate = table.Column<DateTime>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    InstagramAccountId = table.Column<Guid>(nullable: false),
                    Discriminator = table.Column<string>(maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FollowPromotions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FollowPromotions_InstagramAccounts_InstagramAccountId",
                        column: x => x.InstagramAccountId,
                        principalTable: "InstagramAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UnfollowPromotions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    InstagramAccountId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnfollowPromotions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnfollowPromotions_InstagramAccounts_InstagramAccountId",
                        column: x => x.InstagramAccountId,
                        principalTable: "InstagramAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SingleScheduleDays_InstagramAccountId",
                table: "SingleScheduleDays",
                column: "InstagramAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleGroups_InstagramAccountId",
                table: "ScheduleGroups",
                column: "InstagramAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionComments_InstagramAccountId",
                table: "PromotionComments",
                column: "InstagramAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyGroupSchedules_InstagramAccountId",
                table: "MonthlyGroupSchedules",
                column: "InstagramAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_InstagramProxies_InstagramAccountId",
                table: "InstagramProxies",
                column: "InstagramAccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InstagramAccounts_UserId",
                table: "InstagramAccounts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FollowedProfiles_InstagramAccountId",
                table: "FollowedProfiles",
                column: "InstagramAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ExplicitDaySchedules_InstagramAccountId",
                table: "ExplicitDaySchedules",
                column: "InstagramAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyPromotionPercentages_FollowPromotionId",
                table: "DailyPromotionPercentages",
                column: "FollowPromotionId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyPromotionPercentages_UnfollowPromotionId",
                table: "DailyPromotionPercentages",
                column: "UnfollowPromotionId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_InstagramAccountId",
                table: "Comments",
                column: "InstagramAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ChildComments_CommentId",
                table: "ChildComments",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountStatistics_InstagramAccountId",
                table: "AccountStatistics",
                column: "InstagramAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountSettings_InstagramAccountId",
                table: "AccountSettings",
                column: "InstagramAccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FollowPromotions_InstagramAccountId",
                table: "FollowPromotions",
                column: "InstagramAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_UnfollowPromotions_InstagramAccountId",
                table: "UnfollowPromotions",
                column: "InstagramAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_AccountSettings_InstagramAccounts_InstagramAccountId",
                table: "AccountSettings",
                column: "InstagramAccountId",
                principalTable: "InstagramAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AccountStatistics_InstagramAccounts_InstagramAccountId",
                table: "AccountStatistics",
                column: "InstagramAccountId",
                principalTable: "InstagramAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChildComments_Comments_CommentId",
                table: "ChildComments",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_InstagramAccounts_InstagramAccountId",
                table: "Comments",
                column: "InstagramAccountId",
                principalTable: "InstagramAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DailyPromotionPercentages_FollowPromotions_FollowPromotionId",
                table: "DailyPromotionPercentages",
                column: "FollowPromotionId",
                principalTable: "FollowPromotions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DailyPromotionPercentages_UnfollowPromotions_UnfollowPromotionId",
                table: "DailyPromotionPercentages",
                column: "UnfollowPromotionId",
                principalTable: "UnfollowPromotions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ExplicitDaySchedules_InstagramAccounts_InstagramAccountId",
                table: "ExplicitDaySchedules",
                column: "InstagramAccountId",
                principalTable: "InstagramAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FollowedProfiles_InstagramAccounts_InstagramAccountId",
                table: "FollowedProfiles",
                column: "InstagramAccountId",
                principalTable: "InstagramAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InstagramAccounts_Users_UserId",
                table: "InstagramAccounts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InstagramProxies_InstagramAccounts_InstagramAccountId",
                table: "InstagramProxies",
                column: "InstagramAccountId",
                principalTable: "InstagramAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MonthlyGroupSchedules_InstagramAccounts_InstagramAccountId",
                table: "MonthlyGroupSchedules",
                column: "InstagramAccountId",
                principalTable: "InstagramAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PromotionComments_InstagramAccounts_InstagramAccountId",
                table: "PromotionComments",
                column: "InstagramAccountId",
                principalTable: "InstagramAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_Users_UserId",
                table: "RefreshTokens",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleGroups_InstagramAccounts_InstagramAccountId",
                table: "ScheduleGroups",
                column: "InstagramAccountId",
                principalTable: "InstagramAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SingleScheduleDays_InstagramAccounts_InstagramAccountId",
                table: "SingleScheduleDays",
                column: "InstagramAccountId",
                principalTable: "InstagramAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccountSettings_InstagramAccounts_InstagramAccountId",
                table: "AccountSettings");

            migrationBuilder.DropForeignKey(
                name: "FK_AccountStatistics_InstagramAccounts_InstagramAccountId",
                table: "AccountStatistics");

            migrationBuilder.DropForeignKey(
                name: "FK_ChildComments_Comments_CommentId",
                table: "ChildComments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_InstagramAccounts_InstagramAccountId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_DailyPromotionPercentages_FollowPromotions_FollowPromotionId",
                table: "DailyPromotionPercentages");

            migrationBuilder.DropForeignKey(
                name: "FK_DailyPromotionPercentages_UnfollowPromotions_UnfollowPromotionId",
                table: "DailyPromotionPercentages");

            migrationBuilder.DropForeignKey(
                name: "FK_ExplicitDaySchedules_InstagramAccounts_InstagramAccountId",
                table: "ExplicitDaySchedules");

            migrationBuilder.DropForeignKey(
                name: "FK_FollowedProfiles_InstagramAccounts_InstagramAccountId",
                table: "FollowedProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_InstagramAccounts_Users_UserId",
                table: "InstagramAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_InstagramProxies_InstagramAccounts_InstagramAccountId",
                table: "InstagramProxies");

            migrationBuilder.DropForeignKey(
                name: "FK_MonthlyGroupSchedules_InstagramAccounts_InstagramAccountId",
                table: "MonthlyGroupSchedules");

            migrationBuilder.DropForeignKey(
                name: "FK_PromotionComments_InstagramAccounts_InstagramAccountId",
                table: "PromotionComments");

            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_Users_UserId",
                table: "RefreshTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleGroups_InstagramAccounts_InstagramAccountId",
                table: "ScheduleGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_SingleScheduleDays_InstagramAccounts_InstagramAccountId",
                table: "SingleScheduleDays");

            migrationBuilder.DropTable(
                name: "FollowPromotions");

            migrationBuilder.DropTable(
                name: "UnfollowPromotions");

            migrationBuilder.DropIndex(
                name: "IX_SingleScheduleDays_InstagramAccountId",
                table: "SingleScheduleDays");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleGroups_InstagramAccountId",
                table: "ScheduleGroups");

            migrationBuilder.DropIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens");

            migrationBuilder.DropIndex(
                name: "IX_PromotionComments_InstagramAccountId",
                table: "PromotionComments");

            migrationBuilder.DropIndex(
                name: "IX_MonthlyGroupSchedules_InstagramAccountId",
                table: "MonthlyGroupSchedules");

            migrationBuilder.DropIndex(
                name: "IX_InstagramProxies_InstagramAccountId",
                table: "InstagramProxies");

            migrationBuilder.DropIndex(
                name: "IX_InstagramAccounts_UserId",
                table: "InstagramAccounts");

            migrationBuilder.DropIndex(
                name: "IX_FollowedProfiles_InstagramAccountId",
                table: "FollowedProfiles");

            migrationBuilder.DropIndex(
                name: "IX_ExplicitDaySchedules_InstagramAccountId",
                table: "ExplicitDaySchedules");

            migrationBuilder.DropIndex(
                name: "IX_DailyPromotionPercentages_FollowPromotionId",
                table: "DailyPromotionPercentages");

            migrationBuilder.DropIndex(
                name: "IX_DailyPromotionPercentages_UnfollowPromotionId",
                table: "DailyPromotionPercentages");

            migrationBuilder.DropIndex(
                name: "IX_Comments_InstagramAccountId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_ChildComments_CommentId",
                table: "ChildComments");

            migrationBuilder.DropIndex(
                name: "IX_AccountStatistics_InstagramAccountId",
                table: "AccountStatistics");

            migrationBuilder.DropIndex(
                name: "IX_AccountSettings_InstagramAccountId",
                table: "AccountSettings");

            migrationBuilder.DropColumn(
                name: "InstagramAccountId",
                table: "InstagramProxies");

            migrationBuilder.DropColumn(
                name: "FollowPromotionId",
                table: "DailyPromotionPercentages");

            migrationBuilder.DropColumn(
                name: "UnfollowPromotionId",
                table: "DailyPromotionPercentages");

            migrationBuilder.RenameColumn(
                name: "InstagramAccountId",
                table: "SingleScheduleDays",
                newName: "AccountId");

            migrationBuilder.RenameColumn(
                name: "InstagramAccountId",
                table: "ScheduleGroups",
                newName: "AccountId");

            migrationBuilder.RenameColumn(
                name: "InstagramAccountId",
                table: "PromotionComments",
                newName: "AccountId");

            migrationBuilder.RenameColumn(
                name: "InstagramAccountId",
                table: "MonthlyGroupSchedules",
                newName: "AccountId");

            migrationBuilder.RenameColumn(
                name: "InstagramAccountId",
                table: "FollowedProfiles",
                newName: "AccountId");

            migrationBuilder.RenameColumn(
                name: "InstagramAccountId",
                table: "ExplicitDaySchedules",
                newName: "AccountId");

            migrationBuilder.RenameColumn(
                name: "InstagramAccountId",
                table: "Comments",
                newName: "AccountId");

            migrationBuilder.RenameColumn(
                name: "CommentId",
                table: "ChildComments",
                newName: "ParentCommentId");

            migrationBuilder.RenameColumn(
                name: "InstagramAccountId",
                table: "AccountStatistics",
                newName: "AccountId");

            migrationBuilder.RenameColumn(
                name: "InstagramAccountId",
                table: "AccountSettings",
                newName: "AccountId");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "InstagramAccounts",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AccountId",
                table: "ChildComments",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "AccountProxies",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: false),
                    ProxyId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountProxies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Promotions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AccountId = table.Column<Guid>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    Label = table.Column<string>(maxLength: 100, nullable: false),
                    LastMediaId = table.Column<string>(maxLength: 100, nullable: true),
                    NextMinId = table.Column<string>(maxLength: 100, nullable: true),
                    NextMinIdDate = table.Column<DateTime>(nullable: false),
                    PromotionType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Promotions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailyPromotionPercentages_PromotionId",
                table: "DailyPromotionPercentages",
                column: "PromotionId");

            migrationBuilder.AddForeignKey(
                name: "FK_DailyPromotionPercentages_Promotions_PromotionId",
                table: "DailyPromotionPercentages",
                column: "PromotionId",
                principalTable: "Promotions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
