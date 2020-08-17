using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SFM.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "Configs",
                table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Key = table.Column<string>(maxLength: 100, nullable: false),
                    Value = table.Column<string>(maxLength: 255, nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Configs", x => x.Id); });

            migrationBuilder.CreateTable(
                "SpotifyUsers",
                table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DisplayName = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: false),
                    ApiUrl = table.Column<string>(nullable: true),
                    SpotifyId = table.Column<string>(nullable: false),
                    ImageUrl = table.Column<string>(nullable: true),
                    Product = table.Column<string>(nullable: false),
                    Type = table.Column<string>(nullable: true),
                    Uri = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    Updated = table.Column<DateTime>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_SpotifyUsers", x => x.Id); });

            migrationBuilder.CreateTable(
                "Subscriptions",
                table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SpotifyUserId = table.Column<long>(nullable: false),
                    PaymentInterval = table.Column<int>(nullable: false),
                    Price = table.Column<double>(nullable: false),
                    LastPayment = table.Column<DateTime>(nullable: false),
                    Token = table.Column<string>(nullable: true),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.Id);
                    table.ForeignKey(
                        "FK_Subscriptions_SpotifyUsers_SpotifyUserId",
                        x => x.SpotifyUserId,
                        "SpotifyUsers",
                        "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                "IX_Subscriptions_SpotifyUserId",
                "Subscriptions",
                "SpotifyUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "Configs");

            migrationBuilder.DropTable(
                "Subscriptions");

            migrationBuilder.DropTable(
                "SpotifyUsers");
        }
    }
}