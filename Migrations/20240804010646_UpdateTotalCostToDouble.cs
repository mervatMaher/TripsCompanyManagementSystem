﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TripsCompanySystem.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTotalCostToDouble : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalCost",
                table: "Bookings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "TotalCost",
                table: "Bookings",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
