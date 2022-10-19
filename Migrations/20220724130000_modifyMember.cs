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
            migrationBuilder.InsertData(
                table: "member",
                columns: new[] { "member_id", "age", "created_at", "deleted_at", "description", "email", "gender", "nickname", "password", "role", "username" },
                values: new object[,]
                {
                    { 1L, null, new DateTimeOffset(new DateTime(2022, 10, 19, 5, 33, 44, 935, DateTimeKind.Unspecified).AddTicks(1153), new TimeSpan(0, 0, 0, 0, 0)), null, null, "seedMail1@google.com", "MALE", "이한별", "$2a$11$mxHSiaqgPKuhCf0G83fEquaLN7qi7MbU.sMVPiswlM94BCB3iSjGO", "ROLE_USER", "seedUsername1" },
                    { 2L, null, new DateTimeOffset(new DateTime(2022, 10, 19, 5, 33, 44, 935, DateTimeKind.Unspecified).AddTicks(1257), new TimeSpan(0, 0, 0, 0, 0)), null, null, "seedMail2@google.com", "FEMALE", "박성재", "$2a$11$mxHSiaqgPKuhCf0G83fEquaLN7qi7MbU.sMVPiswlM94BCB3iSjGO", "ROLE_USER", "seedUsername2" },
                    { 3L, null, new DateTimeOffset(new DateTime(2022, 10, 19, 5, 33, 44, 935, DateTimeKind.Unspecified).AddTicks(1261), new TimeSpan(0, 0, 0, 0, 0)), null, null, "seedMail3@google.com", "FEMALE", "이한별", "$2a$11$mxHSiaqgPKuhCf0G83fEquaLN7qi7MbU.sMVPiswlM94BCB3iSjGO", "ROLE_USER", "seedUsername3" },
                    { 4L, null, new DateTimeOffset(new DateTime(2022, 10, 19, 5, 33, 44, 935, DateTimeKind.Unspecified).AddTicks(1264), new TimeSpan(0, 0, 0, 0, 0)), null, null, "seedMail4@google.com", "FEMALE", "최저스틴", "$2a$11$mxHSiaqgPKuhCf0G83fEquaLN7qi7MbU.sMVPiswlM94BCB3iSjGO", "ROLE_USER", "seedUsername4" },
                    { 5L, null, new DateTimeOffset(new DateTime(2022, 10, 19, 5, 33, 44, 935, DateTimeKind.Unspecified).AddTicks(1267), new TimeSpan(0, 0, 0, 0, 0)), null, null, "seedMail5@google.com", "FEMALE", "박두훈", "$2a$11$mxHSiaqgPKuhCf0G83fEquaLN7qi7MbU.sMVPiswlM94BCB3iSjGO", "ROLE_USER", "seedUsername5" },
                    { 6L, null, new DateTimeOffset(new DateTime(2022, 10, 19, 5, 33, 44, 935, DateTimeKind.Unspecified).AddTicks(1279), new TimeSpan(0, 0, 0, 0, 0)), null, null, "seedMail6@google.com", "MALE", "박성재", "$2a$11$mxHSiaqgPKuhCf0G83fEquaLN7qi7MbU.sMVPiswlM94BCB3iSjGO", "ROLE_USER", "seedUsername6" },
                    { 7L, null, new DateTimeOffset(new DateTime(2022, 10, 19, 5, 33, 44, 935, DateTimeKind.Unspecified).AddTicks(1281), new TimeSpan(0, 0, 0, 0, 0)), null, null, "seedMail7@google.com", "FEMALE", "한두훈", "$2a$11$mxHSiaqgPKuhCf0G83fEquaLN7qi7MbU.sMVPiswlM94BCB3iSjGO", "ROLE_USER", "seedUsername7" },
                    { 8L, null, new DateTimeOffset(new DateTime(2022, 10, 19, 5, 33, 44, 935, DateTimeKind.Unspecified).AddTicks(1353), new TimeSpan(0, 0, 0, 0, 0)), null, null, "seedMail8@google.com", "FEMALE", "이저스틴", "$2a$11$mxHSiaqgPKuhCf0G83fEquaLN7qi7MbU.sMVPiswlM94BCB3iSjGO", "ROLE_USER", "seedUsername8" },
                    { 9L, null, new DateTimeOffset(new DateTime(2022, 10, 19, 5, 33, 44, 935, DateTimeKind.Unspecified).AddTicks(1356), new TimeSpan(0, 0, 0, 0, 0)), null, null, "seedMail9@google.com", "MALE", "김저스틴", "$2a$11$mxHSiaqgPKuhCf0G83fEquaLN7qi7MbU.sMVPiswlM94BCB3iSjGO", "ROLE_USER", "seedUsername9" },
                    { 10L, null, new DateTimeOffset(new DateTime(2022, 10, 19, 5, 33, 44, 935, DateTimeKind.Unspecified).AddTicks(1360), new TimeSpan(0, 0, 0, 0, 0)), null, null, "seedMail10@google.com", "MALE", "한한별", "$2a$11$mxHSiaqgPKuhCf0G83fEquaLN7qi7MbU.sMVPiswlM94BCB3iSjGO", "ROLE_USER", "seedUsername10" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "MemberId", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { 1, 0, "0212b38e-ed16-486a-a6df-68f904b8a228", "seedMail1@google.com", false, false, null, 1L, null, null, "$2a$11$mxHSiaqgPKuhCf0G83fEquaLN7qi7MbU.sMVPiswlM94BCB3iSjGO", null, false, null, false, "seedUsername1" },
                    { 2, 0, "5bdedb2e-66fd-489d-917d-435c0f2f89c1", "seedMail2@google.com", false,  false, null, 2L, null, null, "$2a$11$mxHSiaqgPKuhCf0G83fEquaLN7qi7MbU.sMVPiswlM94BCB3iSjGO", null, false, null, false, "seedUsername2" },
                    { 3, 0, "66e080e4-8773-4d48-b6ed-efd497970438", "seedMail3@google.com", false, false, null, 3L, null, null, "$2a$11$mxHSiaqgPKuhCf0G83fEquaLN7qi7MbU.sMVPiswlM94BCB3iSjGO", null, false, null, false, "seedUsername3" },
                    { 4, 0, "3b137b98-3a53-49d9-9852-df69a03398a0", "seedMail4@google.com", false, false, null, 4L, null, null, "$2a$11$mxHSiaqgPKuhCf0G83fEquaLN7qi7MbU.sMVPiswlM94BCB3iSjGO", null, false, null, false, "seedUsername4" },
                    { 5, 0, "fec042a9-56e2-451c-9b04-f9b4e7dc8ac6", "seedMail5@google.com", false, false, null, 5L, null, null, "$2a$11$mxHSiaqgPKuhCf0G83fEquaLN7qi7MbU.sMVPiswlM94BCB3iSjGO", null, false, null, false, "seedUsername5" },
                    { 6, 0, "39b37158-c7c1-4cb4-85e2-48fd94ae6f3b", "seedMail6@google.com", false,  false, null, 6L, null, null, "$2a$11$mxHSiaqgPKuhCf0G83fEquaLN7qi7MbU.sMVPiswlM94BCB3iSjGO", null, false, null, false, "seedUsername6" },
                    { 7, 0, "95f6c680-860b-479f-98ad-d6b831821042", "seedMail7@google.com", false, false, null, 7L, null, null, "$2a$11$mxHSiaqgPKuhCf0G83fEquaLN7qi7MbU.sMVPiswlM94BCB3iSjGO", null, false, null, false, "seedUsername7" },
                    { 8, 0, "cb9d0229-0aba-4a3e-a2e0-82b3e047e31a", "seedMail8@google.com", false, false, null, 8L, null, null, "$2a$11$mxHSiaqgPKuhCf0G83fEquaLN7qi7MbU.sMVPiswlM94BCB3iSjGO", null, false, null, false, "seedUsername8" },
                    { 9, 0, "7143820d-681d-437e-9644-3bc68d20d992", "seedMail9@google.com", false,  false, null, 9L, null, null, "$2a$11$mxHSiaqgPKuhCf0G83fEquaLN7qi7MbU.sMVPiswlM94BCB3iSjGO", null, false, null, false, "seedUsername9" },
                    { 10, 0, "214ad1b8-613d-4fef-89ee-97a3d9ac4c72", "seedMail10@google.com", false, false, null, 10L, null, null, "$2a$11$mxHSiaqgPKuhCf0G83fEquaLN7qi7MbU.sMVPiswlM94BCB3iSjGO", null, false, null, false, "seedUsername10" }
                });
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
            migrationBuilder.DeleteData(
               table: "AspNetUsers",
               keyColumn: "Id",
               keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "member",
                keyColumn: "member_id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "member",
                keyColumn: "member_id",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "member",
                keyColumn: "member_id",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "member",
                keyColumn: "member_id",
                keyValue: 4L);

            migrationBuilder.DeleteData(
                table: "member",
                keyColumn: "member_id",
                keyValue: 5L);

            migrationBuilder.DeleteData(
                table: "member",
                keyColumn: "member_id",
                keyValue: 6L);

            migrationBuilder.DeleteData(
                table: "member",
                keyColumn: "member_id",
                keyValue: 7L);

            migrationBuilder.DeleteData(
                table: "member",
                keyColumn: "member_id",
                keyValue: 8L);

            migrationBuilder.DeleteData(
                table: "member",
                keyColumn: "member_id",
                keyValue: 9L);

            migrationBuilder.DeleteData(
                table: "member",
                keyColumn: "member_id",
                keyValue: 10L);
        }
    }
}
