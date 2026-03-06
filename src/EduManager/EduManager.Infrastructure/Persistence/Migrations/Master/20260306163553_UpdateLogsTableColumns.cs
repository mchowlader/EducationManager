using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduManager.Infrastructure.Persistence.Migrations.Master
{
    /// <inheritdoc />
    public partial class UpdateLogsTableColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Timestamp",
                table: "Logs",
                newName: "TimeStamp");

            migrationBuilder.AddColumn<string>(
                name: "MessageTemplate",
                table: "Logs",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MessageTemplate",
                table: "Logs");

            migrationBuilder.RenameColumn(
                name: "TimeStamp",
                table: "Logs",
                newName: "Timestamp");
        }
    }
}
