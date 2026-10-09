using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CAMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMemberFingerprints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MemberFingerprints",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProtectedTemplate = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    FingerLabel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberFingerprints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MemberFingerprints_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MemberFingerprints_MemberId",
                table: "MemberFingerprints",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberFingerprints_MemberId_IsActive",
                table: "MemberFingerprints",
                columns: new[] { "MemberId", "IsActive" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MemberFingerprints");
        }
    }
}
