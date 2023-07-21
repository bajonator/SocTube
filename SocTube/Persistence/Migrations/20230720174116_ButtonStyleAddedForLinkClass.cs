using Microsoft.EntityFrameworkCore.Migrations;

namespace SocTube.Data.Migrations
{
    public partial class ButtonStyleAddedForLinkClass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LinkVisible",
                table: "Settings");

            migrationBuilder.AddColumn<string>(
                name: "ButtonStyle",
                table: "Links",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ButtonStyle",
                table: "Links");

            migrationBuilder.AddColumn<bool>(
                name: "LinkVisible",
                table: "Settings",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
