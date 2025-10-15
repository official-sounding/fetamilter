using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AdditionalPostState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StateMessage",
                table: "Post",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StateUpdatedByID",
                table: "Post",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Post_StateUpdatedByID",
                table: "Post",
                column: "StateUpdatedByID");

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
                name: "FK_Post_User_StateUpdatedByID",
                table: "Post");

            migrationBuilder.DropIndex(
                name: "IX_Post_StateUpdatedByID",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "StateMessage",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "StateUpdatedByID",
                table: "Post");
        }
    }
}
