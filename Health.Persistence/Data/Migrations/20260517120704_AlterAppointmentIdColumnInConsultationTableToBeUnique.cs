using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Health.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class AlterAppointmentIdColumnInConsultationTableToBeUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Consultations_AppointmentId",
                table: "Consultations");

            migrationBuilder.CreateIndex(
                name: "IX_Consultations_AppointmentId",
                table: "Consultations",
                column: "AppointmentId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Consultations_AppointmentId",
                table: "Consultations");

            migrationBuilder.CreateIndex(
                name: "IX_Consultations_AppointmentId",
                table: "Consultations",
                column: "AppointmentId");
        }
    }
}
