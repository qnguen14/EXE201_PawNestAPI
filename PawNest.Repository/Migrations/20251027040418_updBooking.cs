using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PawNest.Repository.Migrations
{
    /// <inheritdoc />
    public partial class updBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                schema: "PawNestV1",
                table: "Bookings",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "PickUpTime",
                schema: "PawNestV1",
                table: "Bookings",
                type: "text",
                nullable: false,
                oldClrType: typeof(TimeOnly),
                oldType: "time without time zone");

            migrationBuilder.AddColumn<string>(
                name: "PickUpStatus",
                schema: "PawNestV1",
                table: "Bookings",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PickUpStatus",
                schema: "PawNestV1",
                table: "Bookings");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                schema: "PawNestV1",
                table: "Bookings",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "PickUpTime",
                schema: "PawNestV1",
                table: "Bookings",
                type: "time without time zone",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
