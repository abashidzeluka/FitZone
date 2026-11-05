using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FitZone.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTitleAndDescriptionToTrainingSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "TrainingSessions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "TrainingSessions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "TrainingSessions");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "TrainingSessions");
        }
    }
}
