using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Health.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class AppointMentModuleInitialSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientChronicDisease_PatientProfile_PatientId",
                table: "PatientChronicDisease");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PatientProfile",
                table: "PatientProfile");

            migrationBuilder.RenameTable(
                name: "PatientProfile",
                newName: "PatientProfiles");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PatientProfiles",
                table: "PatientProfiles",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DoctorProfileId = table.Column<int>(type: "int", nullable: false),
                    PatientProfileId = table.Column<int>(type: "int", nullable: false),
                    DoctorGeneratedSlotsId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    AppointmentType = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Appointments_DoctorGeneratedSlots_DoctorGeneratedSlotsId",
                        column: x => x.DoctorGeneratedSlotsId,
                        principalTable: "DoctorGeneratedSlots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointments_DoctorProfiles_DoctorProfileId",
                        column: x => x.DoctorProfileId,
                        principalTable: "DoctorProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointments_PatientProfiles_PatientProfileId",
                        column: x => x.PatientProfileId,
                        principalTable: "PatientProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_DoctorGeneratedSlotsId",
                table: "Appointments",
                column: "DoctorGeneratedSlotsId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_DoctorProfileId",
                table: "Appointments",
                column: "DoctorProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_PatientProfileId",
                table: "Appointments",
                column: "PatientProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientChronicDisease_PatientProfiles_PatientId",
                table: "PatientChronicDisease",
                column: "PatientId",
                principalTable: "PatientProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientChronicDisease_PatientProfiles_PatientId",
                table: "PatientChronicDisease");

            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PatientProfiles",
                table: "PatientProfiles");

            migrationBuilder.RenameTable(
                name: "PatientProfiles",
                newName: "PatientProfile");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PatientProfile",
                table: "PatientProfile",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientChronicDisease_PatientProfile_PatientId",
                table: "PatientChronicDisease",
                column: "PatientId",
                principalTable: "PatientProfile",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
