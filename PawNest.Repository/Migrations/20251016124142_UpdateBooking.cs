using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PawNest.Repository.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Pets_PetId",
                schema: "PawNestV1",
                table: "Bookings");

            migrationBuilder.RenameColumn(
                name: "PetId",
                schema: "PawNestV1",
                table: "Bookings",
                newName: "FreelancerId");

            migrationBuilder.RenameIndex(
                name: "IX_Bookings_PetId",
                schema: "PawNestV1",
                table: "Bookings",
                newName: "IX_Bookings_FreelancerId");

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

            migrationBuilder.AddColumn<DateOnly>(
                name: "BookingDate",
                schema: "PawNestV1",
                table: "Bookings",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "PawNestV1",
                table: "Bookings",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "BookingPet",
                schema: "PawNestV1",
                columns: table => new
                {
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    PetId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingPet", x => new { x.BookingId, x.PetId });
                    table.ForeignKey(
                        name: "FK_BookingPet_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalSchema: "PawNestV1",
                        principalTable: "Bookings",
                        principalColumn: "BookingId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingPet_Pets_PetId",
                        column: x => x.PetId,
                        principalSchema: "PawNestV1",
                        principalTable: "Pets",
                        principalColumn: "PetId",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_BookingPet_PetId",
                schema: "PawNestV1",
                table: "BookingPet",
                column: "PetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Users_FreelancerId",
                schema: "PawNestV1",
                table: "Bookings",
                column: "FreelancerId",
                principalSchema: "PawNestV1",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Users_FreelancerId",
                schema: "PawNestV1",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Pets_Services_ServiceId",
                schema: "PawNestV1",
                table: "Pets");

            migrationBuilder.DropForeignKey(
                name: "FK_Services_Users_CustomerId",
                schema: "PawNestV1",
                table: "Services");

            migrationBuilder.DropTable(
                name: "BookingPet",
                schema: "PawNestV1");

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
                name: "BookingDate",
                schema: "PawNestV1",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "PawNestV1",
                table: "Bookings");

            migrationBuilder.RenameColumn(
                name: "FreelancerId",
                schema: "PawNestV1",
                table: "Bookings",
                newName: "PetId");

            migrationBuilder.RenameIndex(
                name: "IX_Bookings_FreelancerId",
                schema: "PawNestV1",
                table: "Bookings",
                newName: "IX_Bookings_PetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Pets_PetId",
                schema: "PawNestV1",
                table: "Bookings",
                column: "PetId",
                principalSchema: "PawNestV1",
                principalTable: "Pets",
                principalColumn: "PetId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
