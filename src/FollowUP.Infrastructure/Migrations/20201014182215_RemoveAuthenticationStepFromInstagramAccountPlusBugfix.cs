using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FollowUP.Infrastructure.Migrations
{
    public partial class RemoveAuthenticationStepFromInstagramAccountPlusBugfix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InstagramAccounts_Users_UserId",
                table: "InstagramAccounts");

            migrationBuilder.DropColumn(
                name: "AuthenticationStep",
                table: "InstagramAccounts");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "InstagramAccounts",
                nullable: false,
                oldClrType: typeof(Guid),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_InstagramAccounts_Users_UserId",
                table: "InstagramAccounts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InstagramAccounts_Users_UserId",
                table: "InstagramAccounts");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "InstagramAccounts",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddColumn<int>(
                name: "AuthenticationStep",
                table: "InstagramAccounts",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_InstagramAccounts_Users_UserId",
                table: "InstagramAccounts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
