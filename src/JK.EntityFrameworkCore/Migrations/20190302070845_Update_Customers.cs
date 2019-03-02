using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace JK.Migrations
{
    public partial class Update_Customers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerClaims_Customers_CustomerId",
                table: "CustomerClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerLogins_Customers_CustomerId",
                table: "CustomerLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerTokens_Customers_CustomerId",
                table: "CustomerTokens");

            migrationBuilder.DropIndex(
                name: "IX_CustomerTokens_CustomerId",
                table: "CustomerTokens");

            migrationBuilder.DropIndex(
                name: "IX_CustomerLogins_CustomerId",
                table: "CustomerLogins");

            migrationBuilder.DropIndex(
                name: "IX_CustomerClaims_CustomerId",
                table: "CustomerClaims");

            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "CustomerClaims");

            migrationBuilder.RenameColumn(
                name: "CreatorUserId",
                table: "CustomerClaims",
                newName: "CustomerClaim_CustomerId");

            migrationBuilder.AddColumn<long>(
                name: "CustomerToken_CustomerId",
                table: "CustomerTokens",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CustomerLogin_CustomerId",
                table: "CustomerLogins",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerTokens_CustomerToken_CustomerId",
                table: "CustomerTokens",
                column: "CustomerToken_CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerLogins_CustomerLogin_CustomerId",
                table: "CustomerLogins",
                column: "CustomerLogin_CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerClaims_CustomerClaim_CustomerId",
                table: "CustomerClaims",
                column: "CustomerClaim_CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerClaims_Customers_CustomerClaim_CustomerId",
                table: "CustomerClaims",
                column: "CustomerClaim_CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerLogins_Customers_CustomerLogin_CustomerId",
                table: "CustomerLogins",
                column: "CustomerLogin_CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerTokens_Customers_CustomerToken_CustomerId",
                table: "CustomerTokens",
                column: "CustomerToken_CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerClaims_Customers_CustomerClaim_CustomerId",
                table: "CustomerClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerLogins_Customers_CustomerLogin_CustomerId",
                table: "CustomerLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_CustomerTokens_Customers_CustomerToken_CustomerId",
                table: "CustomerTokens");

            migrationBuilder.DropIndex(
                name: "IX_CustomerTokens_CustomerToken_CustomerId",
                table: "CustomerTokens");

            migrationBuilder.DropIndex(
                name: "IX_CustomerLogins_CustomerLogin_CustomerId",
                table: "CustomerLogins");

            migrationBuilder.DropIndex(
                name: "IX_CustomerClaims_CustomerClaim_CustomerId",
                table: "CustomerClaims");

            migrationBuilder.DropColumn(
                name: "CustomerToken_CustomerId",
                table: "CustomerTokens");

            migrationBuilder.DropColumn(
                name: "CustomerLogin_CustomerId",
                table: "CustomerLogins");

            migrationBuilder.RenameColumn(
                name: "CustomerClaim_CustomerId",
                table: "CustomerClaims",
                newName: "CreatorUserId");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "CustomerClaims",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_CustomerTokens_CustomerId",
                table: "CustomerTokens",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerLogins_CustomerId",
                table: "CustomerLogins",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerClaims_CustomerId",
                table: "CustomerClaims",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerClaims_Customers_CustomerId",
                table: "CustomerClaims",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerLogins_Customers_CustomerId",
                table: "CustomerLogins",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerTokens_Customers_CustomerId",
                table: "CustomerTokens",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
