using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PawNest.Repository.Migrations
{
    /// <inheritdoc />
    public partial class updateEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Users_OwnerId",
                schema: "PawNestV1",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Pets_Users_OwnerId",
                schema: "PawNestV1",
                table: "Pets");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Users_ReporterId",
                schema: "PawNestV1",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_reviews_Bookings_booking_id",
                schema: "PawNestV1",
                table: "reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_reviews_Users_customer_id",
                schema: "PawNestV1",
                table: "reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_reviews_Users_freelancer_id",
                schema: "PawNestV1",
                table: "reviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_reviews",
                schema: "PawNestV1",
                table: "reviews");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "PawNestV1",
                table: "Bookings");

            migrationBuilder.RenameTable(
                name: "reviews",
                schema: "PawNestV1",
                newName: "Reviews",
                newSchema: "PawNestV1");

            migrationBuilder.RenameIndex(
                name: "IX_reviews_freelancer_id",
                schema: "PawNestV1",
                table: "Reviews",
                newName: "IX_Reviews_freelancer_id");

            migrationBuilder.RenameIndex(
                name: "IX_reviews_customer_id",
                schema: "PawNestV1",
                table: "Reviews",
                newName: "IX_Reviews_customer_id");

            migrationBuilder.RenameIndex(
                name: "IX_reviews_booking_id",
                schema: "PawNestV1",
                table: "Reviews",
                newName: "IX_Reviews_booking_id");

            migrationBuilder.RenameColumn(
                name: "ReporterId",
                schema: "PawNestV1",
                table: "Reports",
                newName: "StaffId");

            migrationBuilder.RenameIndex(
                name: "IX_Reports_ReporterId",
                schema: "PawNestV1",
                table: "Reports",
                newName: "IX_Reports_StaffId");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                schema: "PawNestV1",
                table: "Pets",
                newName: "CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_Pets_OwnerId",
                schema: "PawNestV1",
                table: "Pets",
                newName: "IX_Pets_CustomerId");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                schema: "PawNestV1",
                table: "Bookings",
                newName: "CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_Bookings_OwnerId",
                schema: "PawNestV1",
                table: "Bookings",
                newName: "IX_Bookings_CustomerId");

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

            migrationBuilder.AddPrimaryKey(
                name: "PK_Reviews",
                schema: "PawNestV1",
                table: "Reviews",
                column: "id");

            

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Users_CustomerId",
                schema: "PawNestV1",
                table: "Bookings",
                column: "CustomerId",
                principalSchema: "PawNestV1",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Pets_Users_CustomerId",
                schema: "PawNestV1",
                table: "Pets",
                column: "CustomerId",
                principalSchema: "PawNestV1",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Users_StaffId",
                schema: "PawNestV1",
                table: "Reports",
                column: "StaffId",
                principalSchema: "PawNestV1",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Bookings_booking_id",
                schema: "PawNestV1",
                table: "Reviews",
                column: "booking_id",
                principalSchema: "PawNestV1",
                principalTable: "Bookings",
                principalColumn: "BookingId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Users_customer_id",
                schema: "PawNestV1",
                table: "Reviews",
                column: "customer_id",
                principalSchema: "PawNestV1",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Users_freelancer_id",
                schema: "PawNestV1",
                table: "Reviews",
                column: "freelancer_id",
                principalSchema: "PawNestV1",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Users_CustomerId",
                schema: "PawNestV1",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Pets_Users_CustomerId",
                schema: "PawNestV1",
                table: "Pets");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Users_StaffId",
                schema: "PawNestV1",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Bookings_booking_id",
                schema: "PawNestV1",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Users_customer_id",
                schema: "PawNestV1",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Users_freelancer_id",
                schema: "PawNestV1",
                table: "Reviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Reviews",
                schema: "PawNestV1",
                table: "Reviews");

            migrationBuilder.RenameTable(
                name: "Reviews",
                schema: "PawNestV1",
                newName: "reviews",
                newSchema: "PawNestV1");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_freelancer_id",
                schema: "PawNestV1",
                table: "reviews",
                newName: "IX_reviews_freelancer_id");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_customer_id",
                schema: "PawNestV1",
                table: "reviews",
                newName: "IX_reviews_customer_id");

            migrationBuilder.RenameIndex(
                name: "IX_Reviews_booking_id",
                schema: "PawNestV1",
                table: "reviews",
                newName: "IX_reviews_booking_id");

            migrationBuilder.RenameColumn(
                name: "StaffId",
                schema: "PawNestV1",
                table: "Reports",
                newName: "ReporterId");

            migrationBuilder.RenameIndex(
                name: "IX_Reports_StaffId",
                schema: "PawNestV1",
                table: "Reports",
                newName: "IX_Reports_ReporterId");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                schema: "PawNestV1",
                table: "Pets",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Pets_CustomerId",
                schema: "PawNestV1",
                table: "Pets",
                newName: "IX_Pets_OwnerId");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                schema: "PawNestV1",
                table: "Bookings",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Bookings_CustomerId",
                schema: "PawNestV1",
                table: "Bookings",
                newName: "IX_Bookings_OwnerId");

            migrationBuilder.AlterColumn<string>(
                name: "Breed",
                schema: "PawNestV1",
                table: "Pets",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                schema: "PawNestV1",
                table: "Bookings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_reviews",
                schema: "PawNestV1",
                table: "reviews",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Users_OwnerId",
                schema: "PawNestV1",
                table: "Bookings",
                column: "OwnerId",
                principalSchema: "PawNestV1",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Pets_Users_OwnerId",
                schema: "PawNestV1",
                table: "Pets",
                column: "OwnerId",
                principalSchema: "PawNestV1",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Users_ReporterId",
                schema: "PawNestV1",
                table: "Reports",
                column: "ReporterId",
                principalSchema: "PawNestV1",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_reviews_Bookings_booking_id",
                schema: "PawNestV1",
                table: "reviews",
                column: "booking_id",
                principalSchema: "PawNestV1",
                principalTable: "Bookings",
                principalColumn: "BookingId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_reviews_Users_customer_id",
                schema: "PawNestV1",
                table: "reviews",
                column: "customer_id",
                principalSchema: "PawNestV1",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_reviews_Users_freelancer_id",
                schema: "PawNestV1",
                table: "reviews",
                column: "freelancer_id",
                principalSchema: "PawNestV1",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
