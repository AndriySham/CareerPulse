using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareerPulse.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeEducationFromDegreeAndFieldOfStudyToDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Degree",
                table: "Educations");

            migrationBuilder.RenameColumn(
                name: "FieldOfStudy",
                table: "Educations",
                newName: "Description");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Educations",
                newName: "FieldOfStudy");

            migrationBuilder.AddColumn<string>(
                name: "Degree",
                table: "Educations",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);
        }
    }
}
