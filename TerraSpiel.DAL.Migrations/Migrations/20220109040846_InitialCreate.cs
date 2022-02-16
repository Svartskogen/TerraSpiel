using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

namespace TerraSpiel.DAL.Migrations.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    DiscordID = table.Column<long>(nullable: false),
                    Money = table.Column<int>(nullable: false),
                    Military = table.Column<int>(nullable: false),
                    Alive = table.Column<bool>(nullable: false),
                    HarvestTimestamp = table.Column<long>(nullable: false),
                    TrainTimestamp = table.Column<long>(nullable: false),
                    BuildTimestamp = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Players");
        }
    }
}
