using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PawNest.Repository.Migrations
{
    /// <inheritdoc />
    public partial class CleanUpEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Services_ServiceId",
                schema: "PawNestV1",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_ServiceId",
                schema: "PawNestV1",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "ServiceId",
                schema: "PawNestV1",
                table: "Bookings");

            migrationBuilder.RenameColumn(
                name: "PickpTime",
                schema: "PawNestV1",
                table: "Bookings",
                newName: "PickUpTime");

            migrationBuilder.AlterColumn<string>(
                name: "Breed",
                schema: "PawNestV1",
                table: "Pets",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "PawNestV1",
                table: "Bookings",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                schema: "PawNestV1",
                table: "Bookings",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                schema: "PawNestV1",
                table: "Bookings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPrice",
                schema: "PawNestV1",
                table: "Bookings",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "BookingService",
                schema: "PawNestV1",
                columns: table => new
                {
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    ServiceId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingService", x => new { x.BookingId, x.ServiceId });
                    table.ForeignKey(
                        name: "FK_BookingService_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalSchema: "PawNestV1",
                        principalTable: "Bookings",
                        principalColumn: "BookingId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingService_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalSchema: "PawNestV1",
                        principalTable: "Services",
                        principalColumn: "ServiceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                schema: "PawNestV1",
                columns: table => new
                {
                    PaymentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    CommissionAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    Method = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.PaymentId);
                    table.ForeignKey(
                        name: "FK_Payments_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalSchema: "PawNestV1",
                        principalTable: "Bookings",
                        principalColumn: "BookingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookingService_ServiceId",
                schema: "PawNestV1",
                table: "BookingService",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_BookingId",
                schema: "PawNestV1",
                table: "Payments",
                column: "BookingId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingService",
                schema: "PawNestV1");

            migrationBuilder.DropTable(
                name: "Payments",
                schema: "PawNestV1");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "PawNestV1",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "IsPaid",
                schema: "PawNestV1",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "Notes",
                schema: "PawNestV1",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "TotalPrice",
                schema: "PawNestV1",
                table: "Bookings");

            migrationBuilder.RenameColumn(
                name: "PickUpTime",
                schema: "PawNestV1",
                table: "Bookings",
                newName: "PickpTime");

            migrationBuilder.AlterColumn<string>(
                name: "Breed",
                schema: "PawNestV1",
                table: "Pets",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ServiceId",
                schema: "PawNestV1",
                table: "Bookings",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_ServiceId",
                schema: "PawNestV1",
                table: "Bookings",
                column: "ServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Services_ServiceId",
                schema: "PawNestV1",
                table: "Bookings",
                column: "ServiceId",
                principalSchema: "PawNestV1",
                principalTable: "Services",
                principalColumn: "ServiceId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
