// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TerraSpiel.DAL;

namespace TerraSpiel.DAL.Migrations.Migrations
{
    [DbContext(typeof(GameContext))]
    [Migration("20220109040846_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.22")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("TerraSpiel.DAL.Models.Player", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<bool>("Alive")
                        .HasColumnType("tinyint(1)");

                    b.Property<long>("BuildTimestamp")
                        .HasColumnType("bigint");

                    b.Property<long>("DiscordID")
                        .HasColumnType("bigint");

                    b.Property<long>("HarvestTimestamp")
                        .HasColumnType("bigint");

                    b.Property<int>("Military")
                        .HasColumnType("int");

                    b.Property<int>("Money")
                        .HasColumnType("int");

                    b.Property<long>("TrainTimestamp")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("Players");
                });
#pragma warning restore 612, 618
        }
    }
}
