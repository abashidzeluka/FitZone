using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitZone.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTrainerProfileImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Bio",
                table: "Trainers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfileImageUrl",
                table: "Trainers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bio",
                table: "Trainers");

            migrationBuilder.DropColumn(
                name: "ProfileImageUrl",
                table: "Trainers");
        }
    }
}
