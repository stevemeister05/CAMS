using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CAMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAttendanceQrSessionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AttendanceQrSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Action = table.Column<int>(type: "int", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendanceQrSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttendanceQrSessions_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceQrSessions_EventId_Action",
                table: "AttendanceQrSessions",
                columns: new[] { "EventId", "Action" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceQrSessions_Token",
                table: "AttendanceQrSessions",
                column: "Token",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttendanceQrSessions");
        }
    }
}
