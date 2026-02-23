using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Health.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDisplayNameColumnAndPriceConsultationColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "DoctorProfiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceConsultation",
                table: "DoctorProfiles",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "DoctorProfiles");

            migrationBuilder.DropColumn(
                name: "PriceConsultation",
                table: "DoctorProfiles");
        }
    }
}
