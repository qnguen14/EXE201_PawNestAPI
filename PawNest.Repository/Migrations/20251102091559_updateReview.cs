using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PawNest.Repository.Migrations
{
    /// <inheritdoc />
    public partial class updateReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "reponsed_at",
                schema: "PawNestV1",
                table: "Reviews",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "response",
                schema: "PawNestV1",
                table: "Reviews",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "reponsed_at",
                schema: "PawNestV1",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "response",
                schema: "PawNestV1",
                table: "Reviews");
        }
    }
}
