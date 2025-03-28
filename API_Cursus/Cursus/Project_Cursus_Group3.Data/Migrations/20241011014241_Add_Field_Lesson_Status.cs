using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project_Cursus_Group3.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_Field_Lesson_Status : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Lesson",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Lesson");
        }
    }
}
