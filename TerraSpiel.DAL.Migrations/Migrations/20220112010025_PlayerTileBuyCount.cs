using Microsoft.EntityFrameworkCore.Migrations;

namespace TerraSpiel.DAL.Migrations.Migrations
{
    public partial class PlayerTileBuyCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TilesBoughtCounter",
                table: "Players",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TilesBoughtCounter",
                table: "Players");
        }
    }
}
