using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace JK.Migrations
{
    public partial class Add_PaymentModule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AbpSettings_TenantId_Name",
                table: "AbpSettings");

            migrationBuilder.DropColumn(
                name: "LastLoginTime",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "LastLoginTime",
                table: "AbpUserAccounts");

            migrationBuilder.AddColumn<string>(
                name: "GoogleAuthenticatorKey",
                table: "AbpUsers",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LanguageName",
                table: "AbpLanguageTexts",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AbpLanguages",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 10);

            migrationBuilder.AddColumn<string>(
                name: "ReturnValue",
                table: "AbpAuditLogs",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AbpOrganizationUnitRoles",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: true),
                    RoleId = table.Column<int>(nullable: false),
                    OrganizationUnitId = table.Column<long>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbpOrganizationUnitRoles", x => x.Id);
                });

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
                name: "AgentRelationships",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AgentId = table.Column<long>(nullable: false),
                    SubAgentId = table.Column<long>(nullable: false),
                    SubParentId = table.Column<long>(nullable: false),
                    Level = table.Column<int>(nullable: false),
                    CommissionRate = table.Column<decimal>(nullable: false),
                    NodeCommissionRate = table.Column<decimal>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgentRelationships", x => x.Id);
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
                name: "AppBinaryObjects",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TenantId = table.Column<int>(nullable: true),
                    Bytes = table.Column<byte[]>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppBinaryObjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Banks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 20, nullable: false),
                    BankCode = table.Column<string>(maxLength: 20, nullable: false),
                    OrderNumber = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Channels",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 20, nullable: false),
                    DisplayName = table.Column<string>(maxLength: 20, nullable: true),
                    ChannelCode = table.Column<string>(maxLength: 20, nullable: false),
                    RequiredBank = table.Column<bool>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Channels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChatSessions",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 100, nullable: true),
                    Description = table.Column<string>(maxLength: 2048, nullable: true),
                    SessionType = table.Column<int>(nullable: false),
                    CreatorTenantId = table.Column<int>(nullable: true),
                    CreatorUserId = table.Column<long>(nullable: false),
                    CreationTime = table.Column<long>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatSessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    CurrencyUnit = table.Column<int>(nullable: false),
                    DefaultFeeRate = table.Column<decimal>(nullable: true),
                    SupportedWithdrawals = table.Column<bool>(nullable: false),
                    SupportedQueryBalance = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomerLoginAttempts",
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
                    CustomerId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerLoginAttempts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
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
                    AgentId = table.Column<long>(nullable: true),
                    RefCustomerId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentApps",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TenantId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 32, nullable: false),
                    AppId = table.Column<string>(maxLength: 32, nullable: false),
                    Key = table.Column<string>(maxLength: 32, nullable: false),
                    TransparentKey = table.Column<string>(maxLength: 32, nullable: false),
                    DeviceType = table.Column<int>(nullable: false),
                    CallbackDomain = table.Column<string>(maxLength: 256, nullable: false),
                    UseSSL = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentApps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentApps_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentOrders",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    CompanyId = table.Column<int>(nullable: false),
                    ChannelId = table.Column<int>(nullable: false),
                    AccountId = table.Column<int>(nullable: false),
                    AppId = table.Column<int>(nullable: false),
                    BankId = table.Column<int>(nullable: true),
                    ExternalOrderId = table.Column<string>(maxLength: 32, nullable: true),
                    ThirdPartyOrderId = table.Column<string>(maxLength: 32, nullable: true),
                    Amount = table.Column<long>(nullable: false),
                    PaidAmount = table.Column<int>(nullable: true),
                    Fee = table.Column<int>(nullable: false),
                    Expire = table.Column<DateTime>(nullable: false),
                    PaymentStatus = table.Column<int>(nullable: false),
                    PaidDate = table.Column<DateTime>(nullable: true),
                    CancelledDate = table.Column<DateTime>(nullable: true),
                    CallbackStatus = table.Column<int>(nullable: false),
                    CreateIp = table.Column<string>(maxLength: 64, nullable: false),
                    DeviceType = table.Column<int>(nullable: false),
                    Md5 = table.Column<string>(maxLength: 32, nullable: false),
                    SyncCallback = table.Column<string>(maxLength: 256, nullable: true),
                    AsyncCallback = table.Column<string>(maxLength: 256, nullable: true),
                    ExtData = table.Column<string>(maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentOrders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TenantDomains",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Host = table.Column<string>(maxLength: 500, nullable: true),
                    Port = table.Column<int>(nullable: false),
                    TenantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantDomains", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TenantLimitPolicies",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TenantId = table.Column<int>(nullable: false),
                    PolicyName = table.Column<string>(maxLength: 32, nullable: false),
                    Priority = table.Column<int>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantLimitPolicies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantLimitPolicies_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.CreateTable(
                name: "ChatMessages",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<long>(nullable: false),
                    SenderName = table.Column<string>(maxLength: 20, nullable: true),
                    SessionId = table.Column<long>(nullable: false),
                    Message = table.Column<string>(maxLength: 4096, nullable: true),
                    CreationTime = table.Column<long>(nullable: false),
                    ReadState = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatMessages_ChatSessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "ChatSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatMessages_AbpUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AbpUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatSessionMembers",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SessionId = table.Column<long>(nullable: false),
                    TenantId = table.Column<int>(nullable: true),
                    UserId = table.Column<long>(nullable: false),
                    CreationTime = table.Column<long>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatSessionMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatSessionMembers_ChatSessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "ChatSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserChatMessageLogs",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SessionId = table.Column<long>(nullable: false),
                    UserId = table.Column<long>(nullable: false),
                    LastReceivedMessageId = table.Column<long>(nullable: false),
                    LastReadMessageId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserChatMessageLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserChatMessageLogs_ChatSessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "ChatSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApiConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CompanyId = table.Column<int>(nullable: false),
                    ApiMethod = table.Column<int>(nullable: false),
                    RequestType = table.Column<int>(nullable: false),
                    Url = table.Column<string>(maxLength: 256, nullable: false),
                    Method = table.Column<string>(maxLength: 10, nullable: false),
                    ContentType = table.Column<string>(maxLength: 32, nullable: false),
                    DataType = table.Column<int>(nullable: false),
                    AcceptCharset = table.Column<string>(maxLength: 16, nullable: false),
                    HasResponseParameter = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiConfigurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiConfigurations_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApiParameters",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CompanyId = table.Column<int>(nullable: false),
                    ApiMethod = table.Column<int>(nullable: false),
                    Key = table.Column<string>(maxLength: 32, nullable: false),
                    ExpressionType = table.Column<int>(nullable: false),
                    ValueOrExpression = table.Column<string>(maxLength: 500, nullable: false),
                    Required = table.Column<bool>(nullable: false),
                    ParameterType = table.Column<int>(nullable: false),
                    DataTag = table.Column<int>(nullable: true),
                    Format = table.Column<string>(maxLength: 32, nullable: true),
                    Location = table.Column<int>(nullable: true),
                    Encryption = table.Column<int>(nullable: true),
                    EncryptionParameters = table.Column<string>(maxLength: 100, nullable: true),
                    Desc = table.Column<string>(maxLength: 32, nullable: true),
                    OrderNumber = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiParameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiParameters_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BankOverrides",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CompanyId = table.Column<int>(nullable: false),
                    BankId = table.Column<int>(nullable: false),
                    OverrideCode = table.Column<string>(maxLength: 16, nullable: false),
                    OverrideIsActive = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankOverrides", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BankOverrides_Banks_BankId",
                        column: x => x.BankId,
                        principalTable: "Banks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BankOverrides_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChannelOverrides",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ChannelId = table.Column<int>(nullable: false),
                    CompanyId = table.Column<int>(nullable: false),
                    OverrideCode = table.Column<string>(maxLength: 16, nullable: false),
                    OverrideFeeRate = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelOverrides", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChannelOverrides_Channels_ChannelId",
                        column: x => x.ChannelId,
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChannelOverrides_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompanyAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    CompanyId = table.Column<int>(nullable: false),
                    MerchantId = table.Column<string>(maxLength: 32, nullable: false),
                    MerchantKey = table.Column<string>(maxLength: 32, nullable: false),
                    OverrideFeeRate = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyAccounts_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyAccounts_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompanyChannel",
                columns: table => new
                {
                    CompanyId = table.Column<int>(nullable: false),
                    ChannelId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyChannel", x => new { x.CompanyId, x.ChannelId });
                    table.ForeignKey(
                        name: "FK_CompanyChannel_Channels_ChannelId",
                        column: x => x.ChannelId,
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyChannel_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompanyLimitPolicies",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CompanyId = table.Column<int>(nullable: false),
                    PolicyName = table.Column<string>(maxLength: 32, nullable: false),
                    Priority = table.Column<int>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyLimitPolicies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyLimitPolicies_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentOrderPolicies",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TenantId = table.Column<int>(nullable: false),
                    PolicyName = table.Column<string>(maxLength: 32, nullable: false),
                    CompanyId = table.Column<int>(nullable: false),
                    AccountId = table.Column<int>(nullable: false),
                    Priority = table.Column<int>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentOrderPolicies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentOrderPolicies_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaymentOrderPolicies_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResultCode",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CompanyId = table.Column<int>(nullable: false),
                    Code = table.Column<string>(maxLength: 16, nullable: false),
                    Mean = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResultCode", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResultCode_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerClaims",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TenantId = table.Column<int>(nullable: false),
                    ClaimType = table.Column<string>(maxLength: 256, nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    CustomerId = table.Column<long>(nullable: false),
                    CustomerClaim_CustomerId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerClaims_Customers_CustomerClaim_CustomerId",
                        column: x => x.CustomerClaim_CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CustomerLogins",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TenantId = table.Column<int>(nullable: false),
                    LoginProvider = table.Column<string>(maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(maxLength: 256, nullable: false),
                    CustomerId = table.Column<long>(nullable: false),
                    CustomerLogin_CustomerId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerLogins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerLogins_Customers_CustomerLogin_CustomerId",
                        column: x => x.CustomerLogin_CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CustomerTokens",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TenantId = table.Column<int>(nullable: false),
                    LoginProvider = table.Column<string>(maxLength: 128, nullable: true),
                    Name = table.Column<string>(maxLength: 128, nullable: true),
                    Value = table.Column<string>(maxLength: 512, nullable: true),
                    ExpireDate = table.Column<DateTime>(nullable: true),
                    CustomerId = table.Column<long>(nullable: false),
                    CustomerToken_CustomerId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerTokens_Customers_CustomerToken_CustomerId",
                        column: x => x.CustomerToken_CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaymentAppChannel",
                columns: table => new
                {
                    AppId = table.Column<int>(nullable: false),
                    ChannelId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentAppChannel", x => new { x.AppId, x.ChannelId });
                    table.ForeignKey(
                        name: "FK_PaymentAppChannel_PaymentApps_AppId",
                        column: x => x.AppId,
                        principalTable: "PaymentApps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaymentAppChannel_Channels_ChannelId",
                        column: x => x.ChannelId,
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentAppCompany",
                columns: table => new
                {
                    AppId = table.Column<int>(nullable: false),
                    CompanyId = table.Column<int>(nullable: false),
                    ChannelId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentAppCompany", x => new { x.AppId, x.CompanyId });
                    table.ForeignKey(
                        name: "FK_PaymentAppCompany_PaymentApps_AppId",
                        column: x => x.AppId,
                        principalTable: "PaymentApps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaymentAppCompany_Channels_ChannelId",
                        column: x => x.ChannelId,
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentAppCompany_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenantLimitPolicyRules",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TenantId = table.Column<int>(nullable: false),
                    PolicyId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantLimitPolicyRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantLimitPolicyRules_TenantLimitPolicies_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "TenantLimitPolicies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApiChannel",
                columns: table => new
                {
                    ApiId = table.Column<int>(nullable: false),
                    ChannelId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiChannel", x => new { x.ApiId, x.ChannelId });
                    table.ForeignKey(
                        name: "FK_ApiChannel_ApiConfigurations_ApiId",
                        column: x => x.ApiId,
                        principalTable: "ApiConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApiChannel_Channels_ChannelId",
                        column: x => x.ChannelId,
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ParameterChannel",
                columns: table => new
                {
                    ParameterId = table.Column<int>(nullable: false),
                    ChannelId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParameterChannel", x => new { x.ParameterId, x.ChannelId });
                    table.ForeignKey(
                        name: "FK_ParameterChannel_Channels_ChannelId",
                        column: x => x.ChannelId,
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ParameterChannel_ApiParameters_ParameterId",
                        column: x => x.ParameterId,
                        principalTable: "ApiParameters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompanyLimitPolicyRules",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CompanyId = table.Column<int>(nullable: false),
                    PolicyId = table.Column<int>(nullable: false),
                    ParentId = table.Column<int>(nullable: true),
                    RuleSystemName = table.Column<string>(maxLength: 50, nullable: false),
                    InteractionType = table.Column<int>(nullable: true),
                    IsGroup = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyLimitPolicyRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyLimitPolicyRules_CompanyLimitPolicyRules_ParentId",
                        column: x => x.ParentId,
                        principalTable: "CompanyLimitPolicyRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyLimitPolicyRules_CompanyLimitPolicies_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "CompanyLimitPolicies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentOrderPolicyChannel",
                columns: table => new
                {
                    ChannelId = table.Column<int>(nullable: false),
                    PolicyId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentOrderPolicyChannel", x => new { x.PolicyId, x.ChannelId });
                    table.ForeignKey(
                        name: "FK_PaymentOrderPolicyChannel_Channels_ChannelId",
                        column: x => x.ChannelId,
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaymentOrderPolicyChannel_PaymentOrderPolicies_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "PaymentOrderPolicies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentOrderPolicyRules",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TenantId = table.Column<int>(nullable: false),
                    PolicyId = table.Column<int>(nullable: false),
                    ParentId = table.Column<int>(nullable: true),
                    RuleSystemName = table.Column<string>(maxLength: 50, nullable: false),
                    InteractionType = table.Column<int>(nullable: true),
                    IsGroup = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentOrderPolicyRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentOrderPolicyRules_PaymentOrderPolicyRules_ParentId",
                        column: x => x.ParentId,
                        principalTable: "PaymentOrderPolicyRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentOrderPolicyRules_PaymentOrderPolicies_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "PaymentOrderPolicies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenantLimitPolicyRuleValues",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TenantId = table.Column<int>(nullable: false),
                    RuleId = table.Column<int>(nullable: false),
                    Value = table.Column<string>(maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantLimitPolicyRuleValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantLimitPolicyRuleValues_TenantLimitPolicyRules_RuleId",
                        column: x => x.RuleId,
                        principalTable: "TenantLimitPolicyRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompanyLimitPolicyRuleValues",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CompanyId = table.Column<int>(nullable: false),
                    RuleId = table.Column<int>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyLimitPolicyRuleValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyLimitPolicyRuleValues_CompanyLimitPolicyRules_RuleId",
                        column: x => x.RuleId,
                        principalTable: "CompanyLimitPolicyRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentOrderPolicyRuleValues",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    CreatorUserId = table.Column<long>(nullable: true),
                    LastModificationTime = table.Column<DateTime>(nullable: true),
                    LastModifierUserId = table.Column<long>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    RuleId = table.Column<int>(nullable: false),
                    Value = table.Column<string>(maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentOrderPolicyRuleValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentOrderPolicyRuleValues_PaymentOrderPolicyRules_RuleId",
                        column: x => x.RuleId,
                        principalTable: "PaymentOrderPolicyRules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AbpSettings_TenantId_Name_UserId",
                table: "AbpSettings",
                columns: new[] { "TenantId", "Name", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AbpOrganizationUnitRoles_TenantId_OrganizationUnitId",
                table: "AbpOrganizationUnitRoles",
                columns: new[] { "TenantId", "OrganizationUnitId" });

            migrationBuilder.CreateIndex(
                name: "IX_AbpOrganizationUnitRoles_TenantId_RoleId",
                table: "AbpOrganizationUnitRoles",
                columns: new[] { "TenantId", "RoleId" });

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

            migrationBuilder.CreateIndex(
                name: "IX_ApiChannel_ChannelId",
                table: "ApiChannel",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiConfigurations_CompanyId",
                table: "ApiConfigurations",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiParameters_CompanyId",
                table: "ApiParameters",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_BankOverrides_BankId",
                table: "BankOverrides",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_BankOverrides_CompanyId",
                table: "BankOverrides",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelOverrides_ChannelId",
                table: "ChannelOverrides",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelOverrides_CompanyId",
                table: "ChannelOverrides",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_SessionId",
                table: "ChatMessages",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_UserId",
                table: "ChatMessages",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatSessionMembers_SessionId",
                table: "ChatSessionMembers",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyAccounts_CompanyId",
                table: "CompanyAccounts",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyAccounts_TenantId",
                table: "CompanyAccounts",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyChannel_ChannelId",
                table: "CompanyChannel",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyLimitPolicies_CompanyId",
                table: "CompanyLimitPolicies",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyLimitPolicyRules_ParentId",
                table: "CompanyLimitPolicyRules",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyLimitPolicyRules_PolicyId",
                table: "CompanyLimitPolicyRules",
                column: "PolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyLimitPolicyRuleValues_RuleId",
                table: "CompanyLimitPolicyRuleValues",
                column: "RuleId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerClaims_CustomerClaim_CustomerId",
                table: "CustomerClaims",
                column: "CustomerClaim_CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerLogins_CustomerLogin_CustomerId",
                table: "CustomerLogins",
                column: "CustomerLogin_CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerTokens_CustomerToken_CustomerId",
                table: "CustomerTokens",
                column: "CustomerToken_CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_ParameterChannel_ChannelId",
                table: "ParameterChannel",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAppChannel_ChannelId",
                table: "PaymentAppChannel",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAppCompany_ChannelId",
                table: "PaymentAppCompany",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAppCompany_CompanyId",
                table: "PaymentAppCompany",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentApps_TenantId",
                table: "PaymentApps",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentOrderPolicies_CompanyId",
                table: "PaymentOrderPolicies",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentOrderPolicies_TenantId",
                table: "PaymentOrderPolicies",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentOrderPolicyChannel_ChannelId",
                table: "PaymentOrderPolicyChannel",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentOrderPolicyRules_ParentId",
                table: "PaymentOrderPolicyRules",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentOrderPolicyRules_PolicyId",
                table: "PaymentOrderPolicyRules",
                column: "PolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentOrderPolicyRuleValues_RuleId",
                table: "PaymentOrderPolicyRuleValues",
                column: "RuleId");

            migrationBuilder.CreateIndex(
                name: "IX_ResultCode_CompanyId",
                table: "ResultCode",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantLimitPolicies_TenantId",
                table: "TenantLimitPolicies",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantLimitPolicyRules_PolicyId",
                table: "TenantLimitPolicyRules",
                column: "PolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantLimitPolicyRuleValues_RuleId",
                table: "TenantLimitPolicyRuleValues",
                column: "RuleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserChatMessageLogs_SessionId_UserId",
                table: "UserChatMessageLogs",
                columns: new[] { "SessionId", "UserId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AbpOrganizationUnitRoles");

            migrationBuilder.DropTable(
                name: "AgentClaims");

            migrationBuilder.DropTable(
                name: "AgentLoginAttempts");

            migrationBuilder.DropTable(
                name: "AgentLogins");

            migrationBuilder.DropTable(
                name: "AgentRelationships");

            migrationBuilder.DropTable(
                name: "AgentTokens");

            migrationBuilder.DropTable(
                name: "ApiChannel");

            migrationBuilder.DropTable(
                name: "AppBinaryObjects");

            migrationBuilder.DropTable(
                name: "BankOverrides");

            migrationBuilder.DropTable(
                name: "ChannelOverrides");

            migrationBuilder.DropTable(
                name: "ChatMessages");

            migrationBuilder.DropTable(
                name: "ChatSessionMembers");

            migrationBuilder.DropTable(
                name: "CompanyAccounts");

            migrationBuilder.DropTable(
                name: "CompanyChannel");

            migrationBuilder.DropTable(
                name: "CompanyLimitPolicyRuleValues");

            migrationBuilder.DropTable(
                name: "CustomerClaims");

            migrationBuilder.DropTable(
                name: "CustomerLoginAttempts");

            migrationBuilder.DropTable(
                name: "CustomerLogins");

            migrationBuilder.DropTable(
                name: "CustomerTokens");

            migrationBuilder.DropTable(
                name: "ParameterChannel");

            migrationBuilder.DropTable(
                name: "PaymentAppChannel");

            migrationBuilder.DropTable(
                name: "PaymentAppCompany");

            migrationBuilder.DropTable(
                name: "PaymentOrderPolicyChannel");

            migrationBuilder.DropTable(
                name: "PaymentOrderPolicyRuleValues");

            migrationBuilder.DropTable(
                name: "PaymentOrders");

            migrationBuilder.DropTable(
                name: "ResultCode");

            migrationBuilder.DropTable(
                name: "TenantDomains");

            migrationBuilder.DropTable(
                name: "TenantLimitPolicyRuleValues");

            migrationBuilder.DropTable(
                name: "UserChatMessageLogs");

            migrationBuilder.DropTable(
                name: "Agents");

            migrationBuilder.DropTable(
                name: "ApiConfigurations");

            migrationBuilder.DropTable(
                name: "Banks");

            migrationBuilder.DropTable(
                name: "CompanyLimitPolicyRules");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "ApiParameters");

            migrationBuilder.DropTable(
                name: "PaymentApps");

            migrationBuilder.DropTable(
                name: "Channels");

            migrationBuilder.DropTable(
                name: "PaymentOrderPolicyRules");

            migrationBuilder.DropTable(
                name: "TenantLimitPolicyRules");

            migrationBuilder.DropTable(
                name: "ChatSessions");

            migrationBuilder.DropTable(
                name: "CompanyLimitPolicies");

            migrationBuilder.DropTable(
                name: "PaymentOrderPolicies");

            migrationBuilder.DropTable(
                name: "TenantLimitPolicies");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropIndex(
                name: "IX_AbpSettings_TenantId_Name_UserId",
                table: "AbpSettings");

            migrationBuilder.DropColumn(
                name: "GoogleAuthenticatorKey",
                table: "AbpUsers");

            migrationBuilder.DropColumn(
                name: "ReturnValue",
                table: "AbpAuditLogs");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLoginTime",
                table: "AbpUsers",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLoginTime",
                table: "AbpUserAccounts",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LanguageName",
                table: "AbpLanguageTexts",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AbpLanguages",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 128);

            migrationBuilder.CreateIndex(
                name: "IX_AbpSettings_TenantId_Name",
                table: "AbpSettings",
                columns: new[] { "TenantId", "Name" });
        }
    }
}
