using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CAMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAttendanceTableConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Attendances_MemberId",
                table: "Attendances");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Attendances_MemberId",
                table: "Attendances",
                column: "MemberId");
        }
    }
}
