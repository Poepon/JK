using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace JK.Migrations
{
    public partial class Update_Chat_DateTime_Unix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "CreationTime",
                table: "ChatMessages",
                nullable: false,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<long>(
                name: "CreationTime",
                table: "ChatGroups",
                nullable: false,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<long>(
                name: "CreationTime",
                table: "ChatGrouppMembers",
                nullable: false,
                oldClrType: typeof(DateTime));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationTime",
                table: "ChatMessages",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationTime",
                table: "ChatGroups",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreationTime",
                table: "ChatGrouppMembers",
                nullable: false,
                oldClrType: typeof(long));
        }
    }
}
