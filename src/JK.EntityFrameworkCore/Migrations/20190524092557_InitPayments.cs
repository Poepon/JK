using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace JK.Migrations
{
    public partial class InitPayments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    HasResponeParameter = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiConfigurations", x => x.Id);
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
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    CurrencyUnit = table.Column<int>(nullable: false),
                    DefaultFeeRate = table.Column<decimal>(nullable: true),
                    MinOrderAmount = table.Column<long>(nullable: true),
                    MaxOrderAmount = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
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
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeleterUserId = table.Column<long>(nullable: true),
                    DeletionTime = table.Column<DateTime>(nullable: true),
                    TenantId = table.Column<int>(nullable: false),
                    CompanyId = table.Column<int>(nullable: false),
                    ChannelId = table.Column<int>(nullable: false),
                    AccountId = table.Column<int>(nullable: false),
                    BankId = table.Column<int>(nullable: true),
                    ExternalOrderId = table.Column<string>(maxLength: 32, nullable: true),
                    ThirdPartyOrderId = table.Column<string>(maxLength: 32, nullable: true),
                    Amount = table.Column<long>(nullable: false),
                    PaidAmount = table.Column<long>(nullable: true),
                    Fee = table.Column<long>(nullable: false),
                    Expire = table.Column<DateTime>(nullable: false),
                    PaymentStatus = table.Column<int>(nullable: false),
                    PaidDate = table.Column<DateTime>(nullable: true),
                    CancelledDate = table.Column<DateTime>(nullable: true),
                    CallbackStatus = table.Column<int>(nullable: false),
                    CreateIp = table.Column<string>(maxLength: 64, nullable: false),
                    Device = table.Column<int>(nullable: false),
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
                name: "TenantPaymentApps",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TenantId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 32, nullable: false),
                    AppId = table.Column<string>(maxLength: 32, nullable: false),
                    Key = table.Column<string>(maxLength: 32, nullable: false),
                    Device = table.Column<int>(nullable: false),
                    CallbackDomain = table.Column<string>(maxLength: 256, nullable: false),
                    UseSSL = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantPaymentApps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantPaymentApps_AbpTenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "AbpTenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApiCallbackParameters",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ApiId = table.Column<int>(nullable: false),
                    Key = table.Column<string>(maxLength: 32, nullable: false),
                    Expression = table.Column<string>(maxLength: 500, nullable: false),
                    Required = table.Column<bool>(nullable: false),
                    Location = table.Column<int>(nullable: false),
                    DataTag = table.Column<int>(nullable: true),
                    Format = table.Column<string>(maxLength: 32, nullable: true),
                    Encryption = table.Column<int>(nullable: true),
                    EncryptionParameters = table.Column<string>(maxLength: 100, nullable: true),
                    Desc = table.Column<string>(maxLength: 32, nullable: true),
                    OrderNumber = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiCallbackParameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiCallbackParameters_ApiConfigurations_ApiId",
                        column: x => x.ApiId,
                        principalTable: "ApiConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApiRequestParameters",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ApiId = table.Column<int>(nullable: false),
                    Key = table.Column<string>(maxLength: 32, nullable: false),
                    ValueOrExpression = table.Column<string>(maxLength: 500, nullable: false),
                    Location = table.Column<int>(nullable: false),
                    Required = table.Column<bool>(nullable: false),
                    Encryption = table.Column<int>(nullable: true),
                    EncryptionParameters = table.Column<string>(maxLength: 100, nullable: true),
                    Format = table.Column<string>(maxLength: 32, nullable: true),
                    Desc = table.Column<string>(maxLength: 32, nullable: true),
                    OrderNumber = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiRequestParameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiRequestParameters_ApiConfigurations_ApiId",
                        column: x => x.ApiId,
                        principalTable: "ApiConfigurations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApiResponeParameters",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ApiId = table.Column<int>(nullable: false),
                    Key = table.Column<string>(maxLength: 32, nullable: false),
                    Expression = table.Column<string>(maxLength: 500, nullable: false),
                    DataTag = table.Column<int>(nullable: true),
                    Location = table.Column<int>(nullable: false),
                    Desc = table.Column<string>(maxLength: 32, nullable: true),
                    OrderNumber = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiResponeParameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiResponeParameters_ApiConfigurations_ApiId",
                        column: x => x.ApiId,
                        principalTable: "ApiConfigurations",
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
                name: "BankOverrides",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CompanyId = table.Column<int>(nullable: false),
                    BankId = table.Column<int>(nullable: false),
                    OverrideCode = table.Column<string>(maxLength: 16, nullable: false)
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
                name: "ResultCodeConfigurations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CompanyId = table.Column<int>(nullable: false),
                    ResultCode = table.Column<string>(maxLength: 16, nullable: false),
                    Mean = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResultCodeConfigurations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResultCodeConfigurations_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
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
                name: "TenantPaymentAppChannel",
                columns: table => new
                {
                    AppId = table.Column<int>(nullable: false),
                    ChannelId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantPaymentAppChannel", x => new { x.AppId, x.ChannelId });
                    table.ForeignKey(
                        name: "FK_TenantPaymentAppChannel_TenantPaymentApps_AppId",
                        column: x => x.AppId,
                        principalTable: "TenantPaymentApps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TenantPaymentAppChannel_Channels_ChannelId",
                        column: x => x.ChannelId,
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenantPaymentAppCompany",
                columns: table => new
                {
                    AppId = table.Column<int>(nullable: false),
                    CompanyId = table.Column<int>(nullable: false),
                    ChannelId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantPaymentAppCompany", x => new { x.AppId, x.CompanyId });
                    table.ForeignKey(
                        name: "FK_TenantPaymentAppCompany_TenantPaymentApps_AppId",
                        column: x => x.AppId,
                        principalTable: "TenantPaymentApps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TenantPaymentAppCompany_Channels_ChannelId",
                        column: x => x.ChannelId,
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TenantPaymentAppCompany_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
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
                name: "IX_ApiCallbackParameters_ApiId",
                table: "ApiCallbackParameters",
                column: "ApiId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiChannel_ChannelId",
                table: "ApiChannel",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiRequestParameters_ApiId",
                table: "ApiRequestParameters",
                column: "ApiId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiResponeParameters_ApiId",
                table: "ApiResponeParameters",
                column: "ApiId");

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
                name: "IX_CompanyAccounts_CompanyId",
                table: "CompanyAccounts",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyChannel_ChannelId",
                table: "CompanyChannel",
                column: "ChannelId");

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
                name: "IX_ResultCodeConfigurations_CompanyId",
                table: "ResultCodeConfigurations",
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
                name: "IX_TenantPaymentAppChannel_ChannelId",
                table: "TenantPaymentAppChannel",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantPaymentAppCompany_ChannelId",
                table: "TenantPaymentAppCompany",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantPaymentAppCompany_CompanyId",
                table: "TenantPaymentAppCompany",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantPaymentApps_TenantId",
                table: "TenantPaymentApps",
                column: "TenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiCallbackParameters");

            migrationBuilder.DropTable(
                name: "ApiChannel");

            migrationBuilder.DropTable(
                name: "ApiRequestParameters");

            migrationBuilder.DropTable(
                name: "ApiResponeParameters");

            migrationBuilder.DropTable(
                name: "BankOverrides");

            migrationBuilder.DropTable(
                name: "ChannelOverrides");

            migrationBuilder.DropTable(
                name: "CompanyAccounts");

            migrationBuilder.DropTable(
                name: "CompanyChannel");

            migrationBuilder.DropTable(
                name: "CompanyLimitPolicyRuleValues");

            migrationBuilder.DropTable(
                name: "PaymentOrderPolicyChannel");

            migrationBuilder.DropTable(
                name: "PaymentOrderPolicyRuleValues");

            migrationBuilder.DropTable(
                name: "PaymentOrders");

            migrationBuilder.DropTable(
                name: "ResultCodeConfigurations");

            migrationBuilder.DropTable(
                name: "TenantLimitPolicyRuleValues");

            migrationBuilder.DropTable(
                name: "TenantPaymentAppChannel");

            migrationBuilder.DropTable(
                name: "TenantPaymentAppCompany");

            migrationBuilder.DropTable(
                name: "ApiConfigurations");

            migrationBuilder.DropTable(
                name: "Banks");

            migrationBuilder.DropTable(
                name: "CompanyLimitPolicyRules");

            migrationBuilder.DropTable(
                name: "PaymentOrderPolicyRules");

            migrationBuilder.DropTable(
                name: "TenantLimitPolicyRules");

            migrationBuilder.DropTable(
                name: "TenantPaymentApps");

            migrationBuilder.DropTable(
                name: "Channels");

            migrationBuilder.DropTable(
                name: "CompanyLimitPolicies");

            migrationBuilder.DropTable(
                name: "PaymentOrderPolicies");

            migrationBuilder.DropTable(
                name: "TenantLimitPolicies");

            migrationBuilder.DropTable(
                name: "Companies");
        }
    }
}
