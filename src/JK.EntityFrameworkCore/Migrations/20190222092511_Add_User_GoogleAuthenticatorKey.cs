using Microsoft.EntityFrameworkCore.Migrations;

namespace JK.Migrations
{
    public partial class Add_User_GoogleAuthenticatorKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "NormalizedEmailAddress",
                table: "AbpUsers",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 256);

            migrationBuilder.AddColumn<string>(
                name: "GoogleAuthenticatorKey",
                table: "AbpUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GoogleAuthenticatorKey",
                table: "AbpUsers");

            migrationBuilder.AlterColumn<string>(
                name: "NormalizedEmailAddress",
                table: "AbpUsers",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 256,
                oldNullable: true);
        }
    }
}
