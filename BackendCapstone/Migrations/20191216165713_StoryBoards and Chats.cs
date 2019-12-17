using Microsoft.EntityFrameworkCore.Migrations;

namespace BackendCapstone.Migrations
{
    public partial class StoryBoardsandChats : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chat_ClientPages_ClientPageId",
                table: "Chat");

            migrationBuilder.DropIndex(
                name: "IX_Chat_ClientPageId",
                table: "Chat");

            migrationBuilder.DropColumn(
                name: "ClientPageId",
                table: "Chat");

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "StoryBoards",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "StoryBoardId",
                table: "Chat",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "00000000-ffff-ffff-ffff-ffffffffffff",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "04ce3b02-c4ea-4091-98ff-bfa7f77f10fb", "AQAAAAEAACcQAAAAEKPssKx4Qw0l8ghDT0V4DQoDxxGiqebCJF/bfF4Ut3Pz+Wmzp54qRei/zDgJGlX/nw==" });

            migrationBuilder.CreateIndex(
                name: "IX_Chat_StoryBoardId",
                table: "Chat",
                column: "StoryBoardId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chat_StoryBoards_StoryBoardId",
                table: "Chat",
                column: "StoryBoardId",
                principalTable: "StoryBoards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chat_StoryBoards_StoryBoardId",
                table: "Chat");

            migrationBuilder.DropIndex(
                name: "IX_Chat_StoryBoardId",
                table: "Chat");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "StoryBoards");

            migrationBuilder.DropColumn(
                name: "StoryBoardId",
                table: "Chat");

            migrationBuilder.AddColumn<int>(
                name: "ClientPageId",
                table: "Chat",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "00000000-ffff-ffff-ffff-ffffffffffff",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "4414f3d8-cf02-474c-a96a-54704ed2c1e3", "AQAAAAEAACcQAAAAEHT/sQom9sqKHGGHrR9uFlkF/vMEXAyYNWDK0eXdPxS0Ss9nLJpDjzOb4TzEWVrFQA==" });

            migrationBuilder.CreateIndex(
                name: "IX_Chat_ClientPageId",
                table: "Chat",
                column: "ClientPageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chat_ClientPages_ClientPageId",
                table: "Chat",
                column: "ClientPageId",
                principalTable: "ClientPages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
