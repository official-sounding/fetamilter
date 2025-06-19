using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations.Migrations
{
    /// <inheritdoc />
    public partial class CommentFavoriteTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "idx_postfavorite_userID",
                table: "PostFavorite",
                newName: "IX_PostFavorite_UserID");

            migrationBuilder.RenameIndex(
                name: "idx_postfavorite_postID",
                table: "PostFavorite",
                newName: "IX_PostFavorite_PostID");

            migrationBuilder.AddColumn<DateTime>(
                name: "FavoritedOn",
                table: "PostFavorite",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "CommentFavorite",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CommentID = table.Column<int>(type: "INTEGER", nullable: false),
                    UserID = table.Column<int>(type: "INTEGER", nullable: false),
                    FavoritedOn = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentFavorite", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CommentFavorite_Comment_CommentID",
                        column: x => x.CommentID,
                        principalTable: "Comment",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommentFavorite_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommentFavorite_CommentID",
                table: "CommentFavorite",
                column: "CommentID");

            migrationBuilder.CreateIndex(
                name: "IX_CommentFavorite_UserID",
                table: "CommentFavorite",
                column: "UserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommentFavorite");

            migrationBuilder.DropColumn(
                name: "FavoritedOn",
                table: "PostFavorite");

            migrationBuilder.RenameIndex(
                name: "IX_PostFavorite_UserID",
                table: "PostFavorite",
                newName: "idx_postfavorite_userID");

            migrationBuilder.RenameIndex(
                name: "IX_PostFavorite_PostID",
                table: "PostFavorite",
                newName: "idx_postfavorite_postID");
        }
    }
}
