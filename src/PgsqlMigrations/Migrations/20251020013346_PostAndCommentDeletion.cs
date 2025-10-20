using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PgsqlMigrations.Migrations
{
    /// <inheritdoc />
    public partial class PostAndCommentDeletion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AutoCloseDays",
                table: "Site",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "Post",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "StateMessage",
                table: "Post",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StateUpdatedByID",
                table: "Post",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StateUpdatedOn",
                table: "Post",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RemovedByID",
                table: "Comment",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RemovedNote",
                table: "Comment",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RemovedOn",
                table: "Comment",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Post_StateUpdatedByID",
                table: "Post",
                column: "StateUpdatedByID");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_RemovedByID",
                table: "Comment",
                column: "RemovedByID");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_User_RemovedByID",
                table: "Comment",
                column: "RemovedByID",
                principalTable: "User",
                principalColumn: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Post_User_StateUpdatedByID",
                table: "Post",
                column: "StateUpdatedByID",
                principalTable: "User",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_User_RemovedByID",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Post_User_StateUpdatedByID",
                table: "Post");

            migrationBuilder.DropIndex(
                name: "IX_Post_StateUpdatedByID",
                table: "Post");

            migrationBuilder.DropIndex(
                name: "IX_Comment_RemovedByID",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "AutoCloseDays",
                table: "Site");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "StateMessage",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "StateUpdatedByID",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "StateUpdatedOn",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "RemovedByID",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "RemovedNote",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "RemovedOn",
                table: "Comment");
        }
    }
}
