using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Migrations.Migrations
{
    /// <inheritdoc />
    public partial class PostNumberTrigger : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder.ActiveProvider == "Microsoft.EntityFrameworkCore.Sqlite")
            {
                migrationBuilder.Sql(@"
            CREATE TRIGGER update_post_number_on_insert AFTER INSERT ON Post 
FOR EACH ROW
  BEGIN
    UPDATE Post
    SET Number = ifnull((SELECT MAX(Number)
                        FROM Post
                        WHERE SiteID = NEW.SiteID
                       ), 0) + 1
    WHERE rowid = NEW.rowid;
  END;
            ");
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            if (migrationBuilder.ActiveProvider == "Microsoft.EntityFrameworkCore.Sqlite")
            {
                migrationBuilder.Sql(@"DROP TRIGGER update_post_number_on_insert");
            }
        }
    }
}
