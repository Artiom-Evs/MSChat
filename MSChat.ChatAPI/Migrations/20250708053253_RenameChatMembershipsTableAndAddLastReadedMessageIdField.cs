using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MSChat.Chat.Migrations
{
    /// <inheritdoc />
    public partial class RenameChatMembershipsTableAndAddLastReadedMessageIdField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "ChatMemberLinks",
                newName: "ChatMemberships"
            );

            migrationBuilder.AddColumn<long>(
                name: "LastReadedMessageId",
                table: "ChatMemberships",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastReadedMessageId",
                table: "ChatMemberLinks");

            migrationBuilder.RenameTable(
                name: "ChatMemberships",
                newName: "ChatMemberLinks"
            );
        }
    }
}
