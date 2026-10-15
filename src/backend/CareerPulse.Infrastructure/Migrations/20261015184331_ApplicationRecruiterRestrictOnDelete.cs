using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CareerPulse.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ApplicationRecruiterRestrictOnDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_Recruiters_RecruiterId",
                table: "Applications");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_Recruiters_RecruiterId",
                table: "Applications",
                column: "RecruiterId",
                principalTable: "Recruiters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_Recruiters_RecruiterId",
                table: "Applications");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_Recruiters_RecruiterId",
                table: "Applications",
                column: "RecruiterId",
                principalTable: "Recruiters",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
