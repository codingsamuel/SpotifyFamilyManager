using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SFM.Migrations
{
    public partial class Paypal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApiUrl",
                table: "SpotifyUsers",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "SpotifyUsers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "SpotifyUsers",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "SpotifyUsers",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "SpotifyUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Product",
                table: "SpotifyUsers",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "SpotifyId",
                table: "SpotifyUsers",
                nullable: false);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "SpotifyUsers",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Updated",
                table: "SpotifyUsers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Uri",
                table: "SpotifyUsers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Configs",
                columns: table => new
                {
                    Key = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configs", x => x.Key);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Configs");

            migrationBuilder.DropColumn(
                name: "ApiUrl",
                table: "SpotifyUsers");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "SpotifyUsers");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "SpotifyUsers");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "SpotifyUsers");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "SpotifyUsers");

            migrationBuilder.DropColumn(
                name: "Product",
                table: "SpotifyUsers");

            migrationBuilder.DropColumn(
                name: "SpotifyId",
                table: "SpotifyUsers");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "SpotifyUsers");

            migrationBuilder.DropColumn(
                name: "Updated",
                table: "SpotifyUsers");

            migrationBuilder.DropColumn(
                name: "Uri",
                table: "SpotifyUsers");
        }
    }
}
