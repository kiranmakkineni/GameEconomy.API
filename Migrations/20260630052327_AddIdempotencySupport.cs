using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameEconomy.API.Migrations
{
    /// <inheritdoc />
    public partial class AddIdempotencySupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PlayerId",
                table: "Wallets",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "IdempotencyKey",
                table: "IdempotencyRequests",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_PlayerId",
                table: "Wallets",
                column: "PlayerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IdempotencyRequests_IdempotencyKey",
                table: "IdempotencyRequests",
                column: "IdempotencyKey",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Wallets_PlayerId",
                table: "Wallets");

            migrationBuilder.DropIndex(
                name: "IX_IdempotencyRequests_IdempotencyKey",
                table: "IdempotencyRequests");

            migrationBuilder.AlterColumn<string>(
                name: "PlayerId",
                table: "Wallets",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "IdempotencyKey",
                table: "IdempotencyRequests",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);
        }
    }
}
