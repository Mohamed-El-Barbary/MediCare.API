using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Health.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class AlterConsultationTablesConfigurations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Consultations_DoctorProfiles_DoctorId",
                table: "Consultations");

            migrationBuilder.DropForeignKey(
                name: "FK_Consultations_PatientProfiles_PatientId",
                table: "Consultations");

            migrationBuilder.DropForeignKey(
                name: "FK_Prescription_Consultations_ConsultationId",
                table: "Prescription");

            migrationBuilder.DropForeignKey(
                name: "FK_PrescriptionItem_Prescription_PrescriptionId",
                table: "PrescriptionItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PrescriptionItem",
                table: "PrescriptionItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Prescription",
                table: "Prescription");

            migrationBuilder.RenameTable(
                name: "PrescriptionItem",
                newName: "PrescriptionItems");

            migrationBuilder.RenameTable(
                name: "Prescription",
                newName: "Prescriptions");

            migrationBuilder.RenameIndex(
                name: "IX_PrescriptionItem_PrescriptionId",
                table: "PrescriptionItems",
                newName: "IX_PrescriptionItems_PrescriptionId");

            migrationBuilder.RenameIndex(
                name: "IX_Prescription_ConsultationId",
                table: "Prescriptions",
                newName: "IX_Prescriptions_ConsultationId");

            migrationBuilder.AlterColumn<string>(
                name: "Symptoms",
                table: "Consultations",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "RoomId",
                table: "Consultations",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Diagnosis",
                table: "Consultations",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "MedicineName",
                table: "PrescriptionItems",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Instructions",
                table: "PrescriptionItems",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Frequency",
                table: "PrescriptionItems",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Dosage",
                table: "PrescriptionItems",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AdditionalNotes",
                table: "Prescriptions",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PrescriptionItems",
                table: "PrescriptionItems",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Prescriptions",
                table: "Prescriptions",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Consultations_RoomId",
                table: "Consultations",
                column: "RoomId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Consultations_DoctorProfiles_DoctorId",
                table: "Consultations",
                column: "DoctorId",
                principalTable: "DoctorProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Consultations_PatientProfiles_PatientId",
                table: "Consultations",
                column: "PatientId",
                principalTable: "PatientProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PrescriptionItems_Prescriptions_PrescriptionId",
                table: "PrescriptionItems",
                column: "PrescriptionId",
                principalTable: "Prescriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Prescriptions_Consultations_ConsultationId",
                table: "Prescriptions",
                column: "ConsultationId",
                principalTable: "Consultations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Consultations_DoctorProfiles_DoctorId",
                table: "Consultations");

            migrationBuilder.DropForeignKey(
                name: "FK_Consultations_PatientProfiles_PatientId",
                table: "Consultations");

            migrationBuilder.DropForeignKey(
                name: "FK_PrescriptionItems_Prescriptions_PrescriptionId",
                table: "PrescriptionItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Prescriptions_Consultations_ConsultationId",
                table: "Prescriptions");

            migrationBuilder.DropIndex(
                name: "IX_Consultations_RoomId",
                table: "Consultations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Prescriptions",
                table: "Prescriptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PrescriptionItems",
                table: "PrescriptionItems");

            migrationBuilder.RenameTable(
                name: "Prescriptions",
                newName: "Prescription");

            migrationBuilder.RenameTable(
                name: "PrescriptionItems",
                newName: "PrescriptionItem");

            migrationBuilder.RenameIndex(
                name: "IX_Prescriptions_ConsultationId",
                table: "Prescription",
                newName: "IX_Prescription_ConsultationId");

            migrationBuilder.RenameIndex(
                name: "IX_PrescriptionItems_PrescriptionId",
                table: "PrescriptionItem",
                newName: "IX_PrescriptionItem_PrescriptionId");

            migrationBuilder.AlterColumn<string>(
                name: "Symptoms",
                table: "Consultations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AlterColumn<string>(
                name: "RoomId",
                table: "Consultations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Diagnosis",
                table: "Consultations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AlterColumn<string>(
                name: "AdditionalNotes",
                table: "Prescription",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MedicineName",
                table: "PrescriptionItem",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Instructions",
                table: "PrescriptionItem",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Frequency",
                table: "PrescriptionItem",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Dosage",
                table: "PrescriptionItem",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Prescription",
                table: "Prescription",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PrescriptionItem",
                table: "PrescriptionItem",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Consultations_DoctorProfiles_DoctorId",
                table: "Consultations",
                column: "DoctorId",
                principalTable: "DoctorProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Consultations_PatientProfiles_PatientId",
                table: "Consultations",
                column: "PatientId",
                principalTable: "PatientProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Prescription_Consultations_ConsultationId",
                table: "Prescription",
                column: "ConsultationId",
                principalTable: "Consultations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PrescriptionItem_Prescription_PrescriptionId",
                table: "PrescriptionItem",
                column: "PrescriptionId",
                principalTable: "Prescription",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
