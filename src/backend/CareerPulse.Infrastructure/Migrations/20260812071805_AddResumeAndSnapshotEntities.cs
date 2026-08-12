using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareerPulse.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddResumeAndSnapshotEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ResumeId",
                table: "ResumeRevisions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Educations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ResumeRevisionId = table.Column<Guid>(type: "uuid", nullable: false),
                    InstitutionName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Degree = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    FieldOfStudy = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    StartYear = table.Column<int>(type: "integer", nullable: true),
                    EndYear = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Educations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Educations_ResumeRevisions_ResumeRevisionId",
                        column: x => x.ResumeRevisionId,
                        principalTable: "ResumeRevisions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ResumeRevisionId = table.Column<Guid>(type: "uuid", nullable: false),
                    LanguageName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Proficiency = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Languages_ResumeRevisions_ResumeRevisionId",
                        column: x => x.ResumeRevisionId,
                        principalTable: "ResumeRevisions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ResumeRevisionId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Role = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    RepositoryUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    LiveDemoUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    TechStack = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_ResumeRevisions_ResumeRevisionId",
                        column: x => x.ResumeRevisionId,
                        principalTable: "ResumeRevisions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Resumes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Track = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CareerLevel = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TargetRole = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resumes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkExperiences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ResumeRevisionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CompanyName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    PositionTitle = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    StartMonth = table.Column<int>(type: "integer", nullable: false),
                    StartYear = table.Column<int>(type: "integer", nullable: false),
                    EndMonth = table.Column<int>(type: "integer", nullable: true),
                    EndYear = table.Column<int>(type: "integer", nullable: true),
                    IsCurrentJob = table.Column<bool>(type: "boolean", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Achievements = table.Column<string>(type: "text", nullable: true),
                    TechStack = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkExperiences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkExperiences_ResumeRevisions_ResumeRevisionId",
                        column: x => x.ResumeRevisionId,
                        principalTable: "ResumeRevisions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResumeRevisions_ResumeId",
                table: "ResumeRevisions",
                column: "ResumeId");

            migrationBuilder.CreateIndex(
                name: "IX_Educations_ResumeRevisionId",
                table: "Educations",
                column: "ResumeRevisionId");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_ResumeRevisionId",
                table: "Languages",
                column: "ResumeRevisionId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ResumeRevisionId",
                table: "Projects",
                column: "ResumeRevisionId");

            migrationBuilder.CreateIndex(
                name: "IX_Resumes_CareerLevel",
                table: "Resumes",
                column: "CareerLevel");

            migrationBuilder.CreateIndex(
                name: "IX_Resumes_Track",
                table: "Resumes",
                column: "Track");

            migrationBuilder.CreateIndex(
                name: "IX_WorkExperiences_ResumeRevisionId",
                table: "WorkExperiences",
                column: "ResumeRevisionId");

            migrationBuilder.AddForeignKey(
                name: "FK_ResumeRevisions_Resumes_ResumeId",
                table: "ResumeRevisions",
                column: "ResumeId",
                principalTable: "Resumes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResumeRevisions_Resumes_ResumeId",
                table: "ResumeRevisions");

            migrationBuilder.DropTable(
                name: "Educations");

            migrationBuilder.DropTable(
                name: "Languages");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "Resumes");

            migrationBuilder.DropTable(
                name: "WorkExperiences");

            migrationBuilder.DropIndex(
                name: "IX_ResumeRevisions_ResumeId",
                table: "ResumeRevisions");

            migrationBuilder.DropColumn(
                name: "ResumeId",
                table: "ResumeRevisions");
        }
    }
}
