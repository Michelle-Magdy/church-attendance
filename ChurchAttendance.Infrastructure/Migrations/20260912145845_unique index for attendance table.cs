using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChurchAttendance.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class uniqueindexforattendancetable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_AttendanceDays_AttendaceDayId",
                table: "Attendances");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_UserId",
                table: "Attendances");

            migrationBuilder.RenameColumn(
                name: "AttendaceDayId",
                table: "Attendances",
                newName: "AttendanceDayId");

            migrationBuilder.RenameIndex(
                name: "IX_Attendances_AttendaceDayId",
                table: "Attendances",
                newName: "IX_Attendances_AttendanceDayId");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_UserId_AttendanceDayId",
                table: "Attendances",
                columns: new[] { "UserId", "AttendanceDayId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_AttendanceDays_AttendanceDayId",
                table: "Attendances",
                column: "AttendanceDayId",
                principalTable: "AttendanceDays",
                principalColumn: "AttendanceDayId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendances_AttendanceDays_AttendanceDayId",
                table: "Attendances");

            migrationBuilder.DropIndex(
                name: "IX_Attendances_UserId_AttendanceDayId",
                table: "Attendances");

            migrationBuilder.RenameColumn(
                name: "AttendanceDayId",
                table: "Attendances",
                newName: "AttendaceDayId");

            migrationBuilder.RenameIndex(
                name: "IX_Attendances_AttendanceDayId",
                table: "Attendances",
                newName: "IX_Attendances_AttendaceDayId");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_UserId",
                table: "Attendances",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_AttendanceDays_AttendaceDayId",
                table: "Attendances",
                column: "AttendaceDayId",
                principalTable: "AttendanceDays",
                principalColumn: "AttendanceDayId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
