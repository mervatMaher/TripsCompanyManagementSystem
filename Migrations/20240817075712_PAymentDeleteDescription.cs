﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TripsCompanySystem.Migrations
{
    /// <inheritdoc />
    public partial class PAymentDeleteDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Payments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
