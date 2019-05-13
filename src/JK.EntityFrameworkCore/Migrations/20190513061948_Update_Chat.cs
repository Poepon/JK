using Microsoft.EntityFrameworkCore.Migrations;

namespace JK.Migrations
{
    public partial class Update_Chat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TenantId",
                table: "ChatSessions",
                newName: "CreatorTenantId");

            migrationBuilder.AddColumn<int>(
                name: "TenantId",
                table: "ChatSessionMembers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "ChatSessionMembers");

            migrationBuilder.RenameColumn(
                name: "CreatorTenantId",
                table: "ChatSessions",
                newName: "TenantId");
        }
    }
}
