using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GabrielOS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCalendarEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CalendarEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    EventType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    StartTime = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    EndTime = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    IsAllDay = table.Column<bool>(type: "INTEGER", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: true),
                    Priority = table.Column<string>(type: "TEXT", maxLength: 10, nullable: true),
                    IsCompleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    GoalId = table.Column<Guid>(type: "TEXT", nullable: true),
                    PillarId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Recurrence = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    RecurrenceEndDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CalendarEvents_Goals_GoalId",
                        column: x => x.GoalId,
                        principalTable: "Goals",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CalendarEvents_Pillars_PillarId",
                        column: x => x.PillarId,
                        principalTable: "Pillars",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CalendarEvents_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEvents_GoalId",
                table: "CalendarEvents",
                column: "GoalId");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEvents_PillarId",
                table: "CalendarEvents",
                column: "PillarId");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEvents_UserId_Date",
                table: "CalendarEvents",
                columns: new[] { "UserId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_CalendarEvents_UserId_EventType",
                table: "CalendarEvents",
                columns: new[] { "UserId", "EventType" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CalendarEvents");
        }
    }
}
