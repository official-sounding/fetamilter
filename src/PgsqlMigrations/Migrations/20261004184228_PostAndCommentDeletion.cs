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
                name: "auto_close_days",
                table: "site",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "state",
                table: "post",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "state_message",
                table: "post",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "state_updated_by_id",
                table: "post",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "state_updated_on",
                table: "post",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "removed_by_id",
                table: "comment",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "removed_note",
                table: "comment",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "removed_on",
                table: "comment",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_post_state_updated_by_id",
                table: "post",
                column: "state_updated_by_id");

            migrationBuilder.CreateIndex(
                name: "ix_comment_removed_by_id",
                table: "comment",
                column: "removed_by_id");

            migrationBuilder.AddForeignKey(
                name: "fk_comment_user_removed_by_id",
                table: "comment",
                column: "removed_by_id",
                principalTable: "user",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_post_user_state_updated_by_id",
                table: "post",
                column: "state_updated_by_id",
                principalTable: "user",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_comment_user_removed_by_id",
                table: "comment");

            migrationBuilder.DropForeignKey(
                name: "fk_post_user_state_updated_by_id",
                table: "post");

            migrationBuilder.DropIndex(
                name: "ix_post_state_updated_by_id",
                table: "post");

            migrationBuilder.DropIndex(
                name: "ix_comment_removed_by_id",
                table: "comment");

            migrationBuilder.DropColumn(
                name: "auto_close_days",
                table: "site");

            migrationBuilder.DropColumn(
                name: "state",
                table: "post");

            migrationBuilder.DropColumn(
                name: "state_message",
                table: "post");

            migrationBuilder.DropColumn(
                name: "state_updated_by_id",
                table: "post");

            migrationBuilder.DropColumn(
                name: "state_updated_on",
                table: "post");

            migrationBuilder.DropColumn(
                name: "removed_by_id",
                table: "comment");

            migrationBuilder.DropColumn(
                name: "removed_note",
                table: "comment");

            migrationBuilder.DropColumn(
                name: "removed_on",
                table: "comment");
        }
    }
}
