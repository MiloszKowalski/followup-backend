using Microsoft.EntityFrameworkCore.Migrations;

namespace FollowUP.Infrastructure.Migrations
{
    public partial class AddScheduleRelations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_MonthlyGroupSchedules_ScheduleGroupId",
                table: "MonthlyGroupSchedules",
                column: "ScheduleGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ExplicitDaySchedules_SingleScheduleDayId",
                table: "ExplicitDaySchedules",
                column: "SingleScheduleDayId");

            migrationBuilder.CreateIndex(
                name: "IX_DayGroupConnections_ScheduleGroupId",
                table: "DayGroupConnections",
                column: "ScheduleGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_DayGroupConnections_SingleScheduleDayId",
                table: "DayGroupConnections",
                column: "SingleScheduleDayId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyPromotionPercentages_PromotionId",
                table: "DailyPromotionPercentages",
                column: "PromotionId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyPromotionPercentages_SingleScheduleDayId",
                table: "DailyPromotionPercentages",
                column: "SingleScheduleDayId");

            migrationBuilder.AddForeignKey(
                name: "FK_DailyPromotionPercentages_Promotions_PromotionId",
                table: "DailyPromotionPercentages",
                column: "PromotionId",
                principalTable: "Promotions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DailyPromotionPercentages_SingleScheduleDays_SingleScheduleDayId",
                table: "DailyPromotionPercentages",
                column: "SingleScheduleDayId",
                principalTable: "SingleScheduleDays",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DayGroupConnections_ScheduleGroups_ScheduleGroupId",
                table: "DayGroupConnections",
                column: "ScheduleGroupId",
                principalTable: "ScheduleGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DayGroupConnections_SingleScheduleDays_SingleScheduleDayId",
                table: "DayGroupConnections",
                column: "SingleScheduleDayId",
                principalTable: "SingleScheduleDays",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExplicitDaySchedules_SingleScheduleDays_SingleScheduleDayId",
                table: "ExplicitDaySchedules",
                column: "SingleScheduleDayId",
                principalTable: "SingleScheduleDays",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MonthlyGroupSchedules_ScheduleGroups_ScheduleGroupId",
                table: "MonthlyGroupSchedules",
                column: "ScheduleGroupId",
                principalTable: "ScheduleGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DailyPromotionPercentages_Promotions_PromotionId",
                table: "DailyPromotionPercentages");

            migrationBuilder.DropForeignKey(
                name: "FK_DailyPromotionPercentages_SingleScheduleDays_SingleScheduleDayId",
                table: "DailyPromotionPercentages");

            migrationBuilder.DropForeignKey(
                name: "FK_DayGroupConnections_ScheduleGroups_ScheduleGroupId",
                table: "DayGroupConnections");

            migrationBuilder.DropForeignKey(
                name: "FK_DayGroupConnections_SingleScheduleDays_SingleScheduleDayId",
                table: "DayGroupConnections");

            migrationBuilder.DropForeignKey(
                name: "FK_ExplicitDaySchedules_SingleScheduleDays_SingleScheduleDayId",
                table: "ExplicitDaySchedules");

            migrationBuilder.DropForeignKey(
                name: "FK_MonthlyGroupSchedules_ScheduleGroups_ScheduleGroupId",
                table: "MonthlyGroupSchedules");

            migrationBuilder.DropIndex(
                name: "IX_MonthlyGroupSchedules_ScheduleGroupId",
                table: "MonthlyGroupSchedules");

            migrationBuilder.DropIndex(
                name: "IX_ExplicitDaySchedules_SingleScheduleDayId",
                table: "ExplicitDaySchedules");

            migrationBuilder.DropIndex(
                name: "IX_DayGroupConnections_ScheduleGroupId",
                table: "DayGroupConnections");

            migrationBuilder.DropIndex(
                name: "IX_DayGroupConnections_SingleScheduleDayId",
                table: "DayGroupConnections");

            migrationBuilder.DropIndex(
                name: "IX_DailyPromotionPercentages_PromotionId",
                table: "DailyPromotionPercentages");

            migrationBuilder.DropIndex(
                name: "IX_DailyPromotionPercentages_SingleScheduleDayId",
                table: "DailyPromotionPercentages");
        }
    }
}
