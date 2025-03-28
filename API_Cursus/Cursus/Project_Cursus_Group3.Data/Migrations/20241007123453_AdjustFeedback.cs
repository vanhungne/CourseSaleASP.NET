using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project_Cursus_Group3.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdjustFeedback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Feedback_CourseId_UserName",
                table: "Feedback");

            migrationBuilder.CreateIndex(
                name: "IX_Feedback_CourseId_UserName",
                table: "Feedback",
                columns: new[] { "CourseId", "UserName" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Feedback_CourseId_UserName",
                table: "Feedback");

            migrationBuilder.CreateIndex(
                name: "IX_Feedback_CourseId_UserName",
                table: "Feedback",
                columns: new[] { "CourseId", "UserName" },
                unique: true,
                filter: "[CourseId] IS NOT NULL AND [UserName] IS NOT NULL");
        }
    }
}
