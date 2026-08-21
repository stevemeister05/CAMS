using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CAMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordHashToRegistrationRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "RegistrationRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "RegistrationRequests");
        }
    }
}
