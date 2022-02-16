using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

namespace TerraSpiel.DAL.Migrations.Migrations
{
    public partial class Map : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Flag",
                table: "Players",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Map",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    X = table.Column<int>(nullable: false),
                    Y = table.Column<int>(nullable: false),
                    OwnerId = table.Column<long>(nullable: false),
                    BuildingId = table.Column<int>(nullable: false),
                    ReinforcementLevel = table.Column<int>(nullable: false),
                    ReinforcementCounter = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Map", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Map");

            migrationBuilder.DropColumn(
                name: "Flag",
                table: "Players");
        }
    }
}
