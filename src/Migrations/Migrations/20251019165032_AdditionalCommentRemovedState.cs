using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AdditionalCommentRemovedState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RemovedByID",
                table: "Comment",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RemovedNote",
                table: "Comment",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RemovedOn",
                table: "Comment",
                type: "TEXT",
                nullable: true);

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_User_RemovedByID",
                table: "Comment");

            migrationBuilder.DropIndex(
                name: "IX_Comment_RemovedByID",
                table: "Comment");

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
