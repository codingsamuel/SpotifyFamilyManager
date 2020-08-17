using Microsoft.EntityFrameworkCore.Migrations;

namespace SFM.Migrations
{
    public partial class AddressState : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                "State",
                "UserAddresses",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "State",
                "UserAddresses");
        }
    }
}