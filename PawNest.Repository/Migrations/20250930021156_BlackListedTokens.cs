using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PawNest.Repository.Migrations
{
    /// <inheritdoc />
    public partial class BlackListedTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BlacklistedTokens",
                schema: "PawNestV1",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tokenHash = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    blacklistedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlacklistedTokens", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlacklistedTokens_expiresAt",
                schema: "PawNestV1",
                table: "BlacklistedTokens",
                column: "expiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_BlacklistedTokens_tokenHash",
                schema: "PawNestV1",
                table: "BlacklistedTokens",
                column: "tokenHash",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlacklistedTokens",
                schema: "PawNestV1");
        }
    }
}
