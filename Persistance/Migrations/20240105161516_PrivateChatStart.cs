using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistance.Migrations
{
    /// <inheritdoc />
    public partial class PrivateChatStart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPrivete",
                table: "ChatGroups",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "ReciverId",
                table: "ChatGroups",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChatGroups_ReciverId",
                table: "ChatGroups",
                column: "ReciverId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatGroups_Users_ReciverId",
                table: "ChatGroups",
                column: "ReciverId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatGroups_Users_ReciverId",
                table: "ChatGroups");

            migrationBuilder.DropIndex(
                name: "IX_ChatGroups_ReciverId",
                table: "ChatGroups");

            migrationBuilder.DropColumn(
                name: "IsPrivete",
                table: "ChatGroups");

            migrationBuilder.DropColumn(
                name: "ReciverId",
                table: "ChatGroups");
        }
    }
}
