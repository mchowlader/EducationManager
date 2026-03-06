using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduManager.Infrastructure.Persistence.Migrations.Master
{
    /// <inheritdoc />
    public partial class AddErrorCodeToLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ErrorCode",
                table: "Logs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Logs_ErrorCode",
                table: "Logs",
                column: "ErrorCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Logs_ErrorCode",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "ErrorCode",
                table: "Logs");
        }
    }
}
