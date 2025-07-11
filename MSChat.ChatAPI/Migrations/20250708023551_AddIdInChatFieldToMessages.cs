using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MSChat.ChatAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddIdInChatFieldToMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Messages_ChatId",
                table: "Messages");

            migrationBuilder.AddColumn<long>(
                name: "IdInChat",
                table: "Messages",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ChatId_IdInChat",
                table: "Messages",
                columns: new[] { "ChatId", "IdInChat" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Messages_ChatId_IdInChat",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "IdInChat",
                table: "Messages");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ChatId",
                table: "Messages",
                column: "ChatId");
        }
    }
}
