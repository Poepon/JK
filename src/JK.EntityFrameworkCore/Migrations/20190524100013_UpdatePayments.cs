using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace JK.Migrations
{
    public partial class UpdatePayments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeleterUserId",
                table: "PaymentOrders");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "PaymentOrders");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "PaymentOrders");

            migrationBuilder.AddColumn<int>(
                name: "AppId",
                table: "PaymentOrders",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppId",
                table: "PaymentOrders");

            migrationBuilder.AddColumn<long>(
                name: "DeleterUserId",
                table: "PaymentOrders",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                table: "PaymentOrders",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "PaymentOrders",
                nullable: false,
                defaultValue: false);
        }
    }
}
