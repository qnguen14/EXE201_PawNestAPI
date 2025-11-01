using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PawNest.Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBookingV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pets_Services_ServiceId",
                schema: "PawNestV1",
                table: "Pets");

            migrationBuilder.DropForeignKey(
                name: "FK_Services_Users_CustomerId",
                schema: "PawNestV1",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Services_CustomerId",
                schema: "PawNestV1",
                table: "Services");

            migrationBuilder.DropIndex(
                name: "IX_Pets_ServiceId",
                schema: "PawNestV1",
                table: "Pets");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                schema: "PawNestV1",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "ServiceId",
                schema: "PawNestV1",
                table: "Pets");

            migrationBuilder.DropColumn(
                name: "EndTime",
                schema: "PawNestV1",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "StartTime",
                schema: "PawNestV1",
                table: "Bookings");

            migrationBuilder.AddColumn<TimeOnly>(
                name: "PickpTime",
                schema: "PawNestV1",
                table: "Bookings",
                type: "time without time zone",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PickpTime",
                schema: "PawNestV1",
                table: "Bookings");

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId",
                schema: "PawNestV1",
                table: "Services",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ServiceId",
                schema: "PawNestV1",
                table: "Pets",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                schema: "PawNestV1",
                table: "Bookings",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "StartTime",
                schema: "PawNestV1",
                table: "Bookings",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Services_CustomerId",
                schema: "PawNestV1",
                table: "Services",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Pets_ServiceId",
                schema: "PawNestV1",
                table: "Pets",
                column: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pets_Services_ServiceId",
                schema: "PawNestV1",
                table: "Pets",
                column: "ServiceId",
                principalSchema: "PawNestV1",
                principalTable: "Services",
                principalColumn: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_Users_CustomerId",
                schema: "PawNestV1",
                table: "Services",
                column: "CustomerId",
                principalSchema: "PawNestV1",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
