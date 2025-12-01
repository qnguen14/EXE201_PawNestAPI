using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PawNest.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddTrackingIdToPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TransactionId",
                schema: "PawNestV1",
                table: "Payments",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransactionId",
                schema: "PawNestV1",
                table: "Payments");
        }
    }
}
