using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthMSR.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordToStaff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Pharmacists",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "LabTechnicians",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "Pharmacists");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "LabTechnicians");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Doctors");
        }
    }
}
