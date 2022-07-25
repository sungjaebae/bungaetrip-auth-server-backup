using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthenticationServer.API.Migrations
{
    public partial class modifyMember : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Member_MemberId",
                table: "AspNetUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Member",
                table: "Member");

            migrationBuilder.RenameTable(
                name: "Member",
                newName: "member");

            migrationBuilder.AddPrimaryKey(
                name: "PK_member",
                table: "member",
                column: "member_id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_member_MemberId",
                table: "AspNetUsers",
                column: "MemberId",
                principalTable: "member",
                principalColumn: "member_id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_member_MemberId",
                table: "AspNetUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_member",
                table: "member");

            migrationBuilder.RenameTable(
                name: "member",
                newName: "Member");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Member",
                table: "Member",
                column: "member_id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Member_MemberId",
                table: "AspNetUsers",
                column: "MemberId",
                principalTable: "Member",
                principalColumn: "member_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
