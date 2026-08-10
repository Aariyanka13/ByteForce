using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartRecruitmentMatchingPlatform.Migrations
{
    /// <inheritdoc />
    public partial class AddContactRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContactRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobApplicationId = table.Column<int>(type: "int", nullable: false),
                    EmployerProfileId = table.Column<int>(type: "int", nullable: false),
                    JobSeekerProfileId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RespondedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContactRequests_EmployerProfiles_EmployerProfileId",
                        column: x => x.EmployerProfileId,
                        principalTable: "EmployerProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContactRequests_JobApplications_JobApplicationId",
                        column: x => x.JobApplicationId,
                        principalTable: "JobApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContactRequests_JobSeekerProfiles_JobSeekerProfileId",
                        column: x => x.JobSeekerProfileId,
                        principalTable: "JobSeekerProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContactRequests_EmployerProfileId",
                table: "ContactRequests",
                column: "EmployerProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactRequests_JobApplicationId_EmployerProfileId_JobSeekerProfileId",
                table: "ContactRequests",
                columns: new[] { "JobApplicationId", "EmployerProfileId", "JobSeekerProfileId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContactRequests_JobSeekerProfileId",
                table: "ContactRequests",
                column: "JobSeekerProfileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContactRequests");
        }
    }
}
