using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Health.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class AlterYearsOfExperienceToStringInDoctorProfileTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "YearsOfExperience",
                table: "DoctorProfiles",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "YearsOfExperience",
                table: "DoctorProfiles",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
