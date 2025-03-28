using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project_Cursus_Group3.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldBalanceNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wallet_User_UserName",
                table: "Wallet");

            migrationBuilder.DropIndex(
                name: "IX_Wallet_UserName",
                table: "Wallet");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "Wallet",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_Wallet_UserName",
                table: "Wallet",
                column: "UserName",
                unique: true,
                filter: "[UserName] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Wallet_User_UserName",
                table: "Wallet",
                column: "UserName",
                principalTable: "User",
                principalColumn: "UserName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wallet_User_UserName",
                table: "Wallet");

            migrationBuilder.DropIndex(
                name: "IX_Wallet_UserName",
                table: "Wallet");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "Wallet",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Wallet_UserName",
                table: "Wallet",
                column: "UserName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Wallet_User_UserName",
                table: "Wallet",
                column: "UserName",
                principalTable: "User",
                principalColumn: "UserName",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
