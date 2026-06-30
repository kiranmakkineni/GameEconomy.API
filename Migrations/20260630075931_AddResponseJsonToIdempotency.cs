using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameEconomy.API.Migrations
{
    /// <inheritdoc />
    public partial class AddResponseJsonToIdempotency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Response",
                table: "IdempotencyRequests",
                newName: "ResponseJson");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ResponseJson",
                table: "IdempotencyRequests",
                newName: "Response");
        }
    }
}
