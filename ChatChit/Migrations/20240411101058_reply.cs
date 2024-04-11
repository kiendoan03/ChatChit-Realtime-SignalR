using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatChit.Migrations
{
    /// <inheritdoc />
    public partial class reply : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "Messages",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ParentId",
                table: "Messages",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Messages_ParentId",
                table: "Messages",
                column: "ParentId",
                principalTable: "Messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Messages_ParentId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_ParentId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Messages");
        }
    }
}
