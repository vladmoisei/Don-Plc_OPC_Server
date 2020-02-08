using Microsoft.EntityFrameworkCore.Migrations;

namespace Don_PlcDashboard_and_Reports.Migrations
{
    public partial class AddIsEnableToPlcModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEnable",
                table: "Plcs",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEnable",
                table: "Plcs");
        }
    }
}
