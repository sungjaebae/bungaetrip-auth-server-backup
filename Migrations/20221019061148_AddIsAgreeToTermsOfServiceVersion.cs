using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthenticationServer.API.Migrations
{
    public partial class AddIsAgreeToTermsOfServiceVersion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAgreeToTermsOfService",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "IsAgreeToTermsOfServiceVersion",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAgreeToTermsOfServiceVersion",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<bool>(
                name: "IsAgreeToTermsOfService",
                table: "AspNetUsers",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }
    }
}
