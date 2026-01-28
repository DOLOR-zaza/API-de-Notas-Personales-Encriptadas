using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API_BACKEND1.Migrations
{
    /// <inheritdoc />
    public partial class AddSharedWithUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanRead",
                table: "SharedNotes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SharedWithUserId",
                table: "SharedNotes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SharedNotes_SharedWithUserId",
                table: "SharedNotes",
                column: "SharedWithUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_SharedNotes_Users_SharedWithUserId",
                table: "SharedNotes",
                column: "SharedWithUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SharedNotes_Users_SharedWithUserId",
                table: "SharedNotes");

            migrationBuilder.DropIndex(
                name: "IX_SharedNotes_SharedWithUserId",
                table: "SharedNotes");

            migrationBuilder.DropColumn(
                name: "CanRead",
                table: "SharedNotes");

            migrationBuilder.DropColumn(
                name: "SharedWithUserId",
                table: "SharedNotes");
        }
    }
}
