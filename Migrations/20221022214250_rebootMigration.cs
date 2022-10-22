using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthenticationServer.API.Migrations
{
    public partial class rebootMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NormalizedName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "member",
                columns: table => new
                {
                    member_id = table.Column<long>(type: "bigint(19)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    age = table.Column<int>(type: "int(10)", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    deleted_at = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    gender = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    nickname = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    password = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    role = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    username = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_member", x => x.member_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RefreshToken",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Token = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshToken", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClaimValue = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    MemberId = table.Column<long>(type: "bigint(19)", nullable: false),
                    IsAgreeToTermsOfServiceVersion = table.Column<int>(type: "int", nullable: false),
                    UserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NormalizedUserName = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NormalizedEmail = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EmailConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PasswordHash = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SecurityStamp = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConcurrencyStamp = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PhoneNumber = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PhoneNumberConfirmed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_member_MemberId",
                        column: x => x.MemberId,
                        principalTable: "member",
                        principalColumn: "member_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClaimValue = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProviderKey = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProviderDisplayName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LoginProvider = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Value = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "member",
                columns: new[] { "member_id", "age", "created_at", "deleted_at", "description", "email", "gender", "nickname", "password", "role", "username" },
                values: new object[,]
                {
                    { 1L, null, new DateTimeOffset(new DateTime(2022, 10, 22, 21, 42, 50, 773, DateTimeKind.Unspecified).AddTicks(285), new TimeSpan(0, 0, 0, 0, 0)), null, null, "seedMail1@google.com", "FEMALE", "김성재", "$2a$11$ieLMeurrnhmxf0p0kKuu9.u1mfCC/tsaNYCnirnYxxMppFxiBMIRK", "ROLE_USER", "seedUsername1" },
                    { 2L, null, new DateTimeOffset(new DateTime(2022, 10, 22, 21, 42, 50, 773, DateTimeKind.Unspecified).AddTicks(355), new TimeSpan(0, 0, 0, 0, 0)), null, null, "seedMail2@google.com", "FEMALE", "최한별", "$2a$11$ieLMeurrnhmxf0p0kKuu9.u1mfCC/tsaNYCnirnYxxMppFxiBMIRK", "ROLE_USER", "seedUsername2" },
                    { 3L, null, new DateTimeOffset(new DateTime(2022, 10, 22, 21, 42, 50, 773, DateTimeKind.Unspecified).AddTicks(359), new TimeSpan(0, 0, 0, 0, 0)), null, null, "seedMail3@google.com", "MALE", "한두훈", "$2a$11$ieLMeurrnhmxf0p0kKuu9.u1mfCC/tsaNYCnirnYxxMppFxiBMIRK", "ROLE_USER", "seedUsername3" },
                    { 4L, null, new DateTimeOffset(new DateTime(2022, 10, 22, 21, 42, 50, 773, DateTimeKind.Unspecified).AddTicks(362), new TimeSpan(0, 0, 0, 0, 0)), null, null, "seedMail4@google.com", "FEMALE", "김한별", "$2a$11$ieLMeurrnhmxf0p0kKuu9.u1mfCC/tsaNYCnirnYxxMppFxiBMIRK", "ROLE_USER", "seedUsername4" },
                    { 5L, null, new DateTimeOffset(new DateTime(2022, 10, 22, 21, 42, 50, 773, DateTimeKind.Unspecified).AddTicks(409), new TimeSpan(0, 0, 0, 0, 0)), null, null, "seedMail5@google.com", "FEMALE", "한성재", "$2a$11$ieLMeurrnhmxf0p0kKuu9.u1mfCC/tsaNYCnirnYxxMppFxiBMIRK", "ROLE_USER", "seedUsername5" },
                    { 6L, null, new DateTimeOffset(new DateTime(2022, 10, 22, 21, 42, 50, 773, DateTimeKind.Unspecified).AddTicks(420), new TimeSpan(0, 0, 0, 0, 0)), null, null, "seedMail6@google.com", "MALE", "최형수", "$2a$11$ieLMeurrnhmxf0p0kKuu9.u1mfCC/tsaNYCnirnYxxMppFxiBMIRK", "ROLE_USER", "seedUsername6" },
                    { 7L, null, new DateTimeOffset(new DateTime(2022, 10, 22, 21, 42, 50, 773, DateTimeKind.Unspecified).AddTicks(423), new TimeSpan(0, 0, 0, 0, 0)), null, null, "seedMail7@google.com", "FEMALE", "최성재", "$2a$11$ieLMeurrnhmxf0p0kKuu9.u1mfCC/tsaNYCnirnYxxMppFxiBMIRK", "ROLE_USER", "seedUsername7" },
                    { 8L, null, new DateTimeOffset(new DateTime(2022, 10, 22, 21, 42, 50, 773, DateTimeKind.Unspecified).AddTicks(425), new TimeSpan(0, 0, 0, 0, 0)), null, null, "seedMail8@google.com", "FEMALE", "박두훈", "$2a$11$ieLMeurrnhmxf0p0kKuu9.u1mfCC/tsaNYCnirnYxxMppFxiBMIRK", "ROLE_USER", "seedUsername8" },
                    { 9L, null, new DateTimeOffset(new DateTime(2022, 10, 22, 21, 42, 50, 773, DateTimeKind.Unspecified).AddTicks(427), new TimeSpan(0, 0, 0, 0, 0)), null, null, "seedMail9@google.com", "FEMALE", "한진석", "$2a$11$ieLMeurrnhmxf0p0kKuu9.u1mfCC/tsaNYCnirnYxxMppFxiBMIRK", "ROLE_USER", "seedUsername9" },
                    { 10L, null, new DateTimeOffset(new DateTime(2022, 10, 22, 21, 42, 50, 773, DateTimeKind.Unspecified).AddTicks(430), new TimeSpan(0, 0, 0, 0, 0)), null, null, "seedMail10@google.com", "MALE", "한한별", "$2a$11$ieLMeurrnhmxf0p0kKuu9.u1mfCC/tsaNYCnirnYxxMppFxiBMIRK", "ROLE_USER", "seedUsername10" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "IsAgreeToTermsOfServiceVersion", "LockoutEnabled", "LockoutEnd", "MemberId", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { 1, 0, "85e35ff3-ce9e-4a25-9adb-9f408804ad3d", "seedMail1@google.com", false, 0, false, null, 1L, null, null, "$2a$11$ieLMeurrnhmxf0p0kKuu9.u1mfCC/tsaNYCnirnYxxMppFxiBMIRK", null, false, null, false, "seedUsername1" },
                    { 2, 0, "4c611a98-e419-46c3-a238-ecab73857a03", "seedMail2@google.com", false, 0, false, null, 2L, null, null, "$2a$11$ieLMeurrnhmxf0p0kKuu9.u1mfCC/tsaNYCnirnYxxMppFxiBMIRK", null, false, null, false, "seedUsername2" },
                    { 3, 0, "f6ab5332-9e0a-48cc-a493-45b142deae7e", "seedMail3@google.com", false, 0, false, null, 3L, null, null, "$2a$11$ieLMeurrnhmxf0p0kKuu9.u1mfCC/tsaNYCnirnYxxMppFxiBMIRK", null, false, null, false, "seedUsername3" },
                    { 4, 0, "ea1ab3bd-6a85-4256-a975-8cff4a0881df", "seedMail4@google.com", false, 0, false, null, 4L, null, null, "$2a$11$ieLMeurrnhmxf0p0kKuu9.u1mfCC/tsaNYCnirnYxxMppFxiBMIRK", null, false, null, false, "seedUsername4" },
                    { 5, 0, "1cc68759-bdf6-41db-a46c-aaf03ce205c9", "seedMail5@google.com", false, 0, false, null, 5L, null, null, "$2a$11$ieLMeurrnhmxf0p0kKuu9.u1mfCC/tsaNYCnirnYxxMppFxiBMIRK", null, false, null, false, "seedUsername5" },
                    { 6, 0, "b764b89c-30d4-46bb-872c-eb89ecc62417", "seedMail6@google.com", false, 0, false, null, 6L, null, null, "$2a$11$ieLMeurrnhmxf0p0kKuu9.u1mfCC/tsaNYCnirnYxxMppFxiBMIRK", null, false, null, false, "seedUsername6" },
                    { 7, 0, "923fc466-66e9-473f-a584-36d74e6dd781", "seedMail7@google.com", false, 0, false, null, 7L, null, null, "$2a$11$ieLMeurrnhmxf0p0kKuu9.u1mfCC/tsaNYCnirnYxxMppFxiBMIRK", null, false, null, false, "seedUsername7" },
                    { 8, 0, "c658b62c-164e-4ce5-8e72-58e76334d4cf", "seedMail8@google.com", false, 0, false, null, 8L, null, null, "$2a$11$ieLMeurrnhmxf0p0kKuu9.u1mfCC/tsaNYCnirnYxxMppFxiBMIRK", null, false, null, false, "seedUsername8" },
                    { 9, 0, "d0a9c08d-77f9-43ac-8414-7d0b18d16303", "seedMail9@google.com", false, 0, false, null, 9L, null, null, "$2a$11$ieLMeurrnhmxf0p0kKuu9.u1mfCC/tsaNYCnirnYxxMppFxiBMIRK", null, false, null, false, "seedUsername9" },
                    { 10, 0, "bc55f51c-a1e4-4ee4-9de0-e968760c160c", "seedMail10@google.com", false, 0, false, null, 10L, null, null, "$2a$11$ieLMeurrnhmxf0p0kKuu9.u1mfCC/tsaNYCnirnYxxMppFxiBMIRK", null, false, null, false, "seedUsername10" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_MemberId",
                table: "AspNetUsers",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "RefreshToken");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "member");
        }
    }
}
