using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace JK.Migrations
{
    public partial class Add_Alliance : Migration
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
                name: "AgentId",
                table: "Customers",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "RefCustomerId",
                table: "Customers",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CustomerLogin_CustomerId",
                table: "CustomerLogins",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AgentLoginAttempts",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TenantId = table.Column<int>(nullable: false),
                    TenancyName = table.Column<string>(maxLength: 64, nullable: false),
                    UserName = table.Column<string>(maxLength: 255, nullable: true),
                    ClientIpAddress = table.Column<string>(maxLength: 64, nullable: true),
                    ClientName = table.Column<string>(maxLength: 128, nullable: true),
                    BrowserInfo = table.Column<string>(maxLength: 512, nullable: true),
                    Result = table.Column<byte>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    AgentId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgentLoginAttempts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Agents",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    UserName = table.Column<string>(maxLength: 256, nullable: false),
                    Password = table.Column<string>(maxLength: 128, nullable: false),
                    Name = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    TenantId = table.Column<int>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    IsTwoFactorEnabled = table.Column<bool>(nullable: false),
                    IsPhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    PhoneNumber = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    IsEmailConfirmed = table.Column<bool>(nullable: false),
                    NormalizedEmail = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    NormalizedUserName = table.Column<string>(nullable: true),
                    IsLockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    ParentId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AgentClaims",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TenantId = table.Column<int>(nullable: false),
                    ClaimType = table.Column<string>(maxLength: 256, nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    AgentId = table.Column<long>(nullable: false),
                    AgentClaim_AgentId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgentClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgentClaims_Agents_AgentClaim_AgentId",
                        column: x => x.AgentClaim_AgentId,
                        principalTable: "Agents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AgentLogins",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TenantId = table.Column<int>(nullable: false),
                    LoginProvider = table.Column<string>(maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(maxLength: 256, nullable: false),
                    AgentId = table.Column<long>(nullable: false),
                    AgentLogin_AgentId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgentLogins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgentLogins_Agents_AgentLogin_AgentId",
                        column: x => x.AgentLogin_AgentId,
                        principalTable: "Agents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AgentTokens",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TenantId = table.Column<int>(nullable: false),
                    LoginProvider = table.Column<string>(maxLength: 128, nullable: true),
                    Name = table.Column<string>(maxLength: 128, nullable: true),
                    Value = table.Column<string>(maxLength: 512, nullable: true),
                    ExpireDate = table.Column<DateTime>(nullable: true),
                    AgentId = table.Column<long>(nullable: false),
                    AgentToken_AgentId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgentTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgentTokens_Agents_AgentToken_AgentId",
                        column: x => x.AgentToken_AgentId,
                        principalTable: "Agents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_AgentClaims_AgentClaim_AgentId",
                table: "AgentClaims",
                column: "AgentClaim_AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_AgentLogins_AgentLogin_AgentId",
                table: "AgentLogins",
                column: "AgentLogin_AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_AgentTokens_AgentToken_AgentId",
                table: "AgentTokens",
                column: "AgentToken_AgentId");

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

            migrationBuilder.DropTable(
                name: "AgentClaims");

            migrationBuilder.DropTable(
                name: "AgentLoginAttempts");

            migrationBuilder.DropTable(
                name: "AgentLogins");

            migrationBuilder.DropTable(
                name: "AgentTokens");

            migrationBuilder.DropTable(
                name: "Agents");

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
                name: "AgentId",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "RefCustomerId",
                table: "Customers");

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
