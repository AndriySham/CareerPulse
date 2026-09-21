using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareerPulse.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLanguageProficiencyAndVacancyLanguageRequirements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Proficiency",
                table: "Languages",
                type: "character varying(6)",
                maxLength: 6,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "VacancyLanguageRequirements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VacancyId = table.Column<Guid>(type: "uuid", nullable: false),
                    LanguageName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Proficiency = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: true),
                    ProficiencyDescription = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VacancyLanguageRequirements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VacancyLanguageRequirements_Vacancies_VacancyId",
                        column: x => x.VacancyId,
                        principalTable: "Vacancies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VacancyLanguageRequirements_VacancyId",
                table: "VacancyLanguageRequirements",
                column: "VacancyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VacancyLanguageRequirements");

            migrationBuilder.AlterColumn<string>(
                name: "Proficiency",
                table: "Languages",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(6)",
                oldMaxLength: 6);
        }
    }
}
