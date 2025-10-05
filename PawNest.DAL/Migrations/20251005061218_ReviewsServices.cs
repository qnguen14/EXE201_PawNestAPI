using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PawNest.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ReviewsServices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Pets_PetId",
                schema: "PawNestV1",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Services_ServiceId",
                schema: "PawNestV1",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Users_OwnerId",
                schema: "PawNestV1",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Post_Users_staff_id",
                schema: "PawNestV1",
                table: "Post");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_role_id",
                schema: "PawNestV1",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Payments",
                schema: "PawNestV1");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                schema: "PawNestV1",
                table: "Services",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "reviews",
                schema: "PawNestV1",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    comment = table.Column<string>(type: "text", nullable: false),
                    rating = table.Column<double>(type: "double precision", nullable: false),
                    freelancer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    booking_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reviews", x => x.id);
                    table.ForeignKey(
                        name: "FK_reviews_Bookings_booking_id",
                        column: x => x.booking_id,
                        principalSchema: "PawNestV1",
                        principalTable: "Bookings",
                        principalColumn: "BookingId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_reviews_Users_customer_id",
                        column: x => x.customer_id,
                        principalSchema: "PawNestV1",
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_reviews_Users_freelancer_id",
                        column: x => x.freelancer_id,
                        principalSchema: "PawNestV1",
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_email",
                schema: "PawNestV1",
                table: "Users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_reviews_booking_id",
                schema: "PawNestV1",
                table: "reviews",
                column: "booking_id");

            migrationBuilder.CreateIndex(
                name: "IX_reviews_customer_id",
                schema: "PawNestV1",
                table: "reviews",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "IX_reviews_freelancer_id",
                schema: "PawNestV1",
                table: "reviews",
                column: "freelancer_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Pets_PetId",
                schema: "PawNestV1",
                table: "Bookings",
                column: "PetId",
                principalSchema: "PawNestV1",
                principalTable: "Pets",
                principalColumn: "PetId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Services_ServiceId",
                schema: "PawNestV1",
                table: "Bookings",
                column: "ServiceId",
                principalSchema: "PawNestV1",
                principalTable: "Services",
                principalColumn: "ServiceId",
                onDelete: ReferentialAction.Restrict);

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
                name: "FK_Post_Users_staff_id",
                schema: "PawNestV1",
                table: "Post",
                column: "staff_id",
                principalSchema: "PawNestV1",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Roles_role_id",
                schema: "PawNestV1",
                table: "Users",
                column: "role_id",
                principalSchema: "PawNestV1",
                principalTable: "Roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Pets_PetId",
                schema: "PawNestV1",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Services_ServiceId",
                schema: "PawNestV1",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Users_OwnerId",
                schema: "PawNestV1",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Post_Users_staff_id",
                schema: "PawNestV1",
                table: "Post");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Roles_role_id",
                schema: "PawNestV1",
                table: "Users");

            migrationBuilder.DropTable(
                name: "reviews",
                schema: "PawNestV1");

            migrationBuilder.DropIndex(
                name: "IX_Users_email",
                schema: "PawNestV1",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Type",
                schema: "PawNestV1",
                table: "Services");

            migrationBuilder.CreateTable(
                name: "Payments",
                schema: "PawNestV1",
                columns: table => new
                {
                    PaymentId = table.Column<Guid>(type: "uuid", nullable: false),
                    BookingId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    CommissionAmount = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Method = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false)
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
                name: "IX_Payments_BookingId",
                schema: "PawNestV1",
                table: "Payments",
                column: "BookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Pets_PetId",
                schema: "PawNestV1",
                table: "Bookings",
                column: "PetId",
                principalSchema: "PawNestV1",
                principalTable: "Pets",
                principalColumn: "PetId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Services_ServiceId",
                schema: "PawNestV1",
                table: "Bookings",
                column: "ServiceId",
                principalSchema: "PawNestV1",
                principalTable: "Services",
                principalColumn: "ServiceId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Users_OwnerId",
                schema: "PawNestV1",
                table: "Bookings",
                column: "OwnerId",
                principalSchema: "PawNestV1",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Post_Users_staff_id",
                schema: "PawNestV1",
                table: "Post",
                column: "staff_id",
                principalSchema: "PawNestV1",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Roles_role_id",
                schema: "PawNestV1",
                table: "Users",
                column: "role_id",
                principalSchema: "PawNestV1",
                principalTable: "Roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
