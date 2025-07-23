using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkUp.Migrations
{
    /// <inheritdoc />
    public partial class FixChatUserManytoMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatUser_Chat_ChatId",
                table: "ChatUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChatUser",
                table: "ChatUser");

            migrationBuilder.AlterColumn<Guid>(
                name: "ChatId",
                table: "ChatUser",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "ChatsId",
                table: "ChatUser",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChatUser",
                table: "ChatUser",
                columns: new[] { "ChatsId", "UsersId" });

            migrationBuilder.CreateIndex(
                name: "IX_ChatUser_ChatId",
                table: "ChatUser",
                column: "ChatId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatUser_Chat_ChatId",
                table: "ChatUser",
                column: "ChatId",
                principalTable: "Chats",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatUser_Chats_ChatsId",
                table: "ChatUser",
                column: "ChatsId",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatUser_Chat_ChatId",
                table: "ChatUser");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatUser_Chats_ChatsId",
                table: "ChatUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChatUser",
                table: "ChatUser");

            migrationBuilder.DropIndex(
                name: "IX_ChatUser_ChatId",
                table: "ChatUser");

            migrationBuilder.DropColumn(
                name: "ChatsId",
                table: "ChatUser");

            migrationBuilder.AlterColumn<Guid>(
                name: "ChatId",
                table: "ChatUser",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChatUser",
                table: "ChatUser",
                columns: new[] { "ChatId", "UsersId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ChatUser_Chat_ChatId",
                table: "ChatUser",
                column: "ChatId",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
