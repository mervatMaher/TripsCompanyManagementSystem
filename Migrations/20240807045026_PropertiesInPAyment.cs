using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TripsCompanySystem.Migrations
{
    /// <inheritdoc />
    public partial class PropertiesInPAyment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductDescription",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductDescription",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "Payments");
        }
    }
}
