using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Health.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSlotStatusEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBooked",
                table: "DoctorGeneratedSlots");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "DoctorGeneratedSlots",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "DoctorGeneratedSlots");

            migrationBuilder.AddColumn<bool>(
                name: "IsBooked",
                table: "DoctorGeneratedSlots",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
