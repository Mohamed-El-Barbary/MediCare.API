using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Health.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreatePhoneClincColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhoneClinc",
                table: "DoctorProfiles",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhoneClinc",
                table: "DoctorProfiles");
        }
    }
}
