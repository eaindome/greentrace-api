using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GreenTrace.Server.Migrations
{
    /// <inheritdoc />
    public partial class StartUp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    permissions = table.Column<string[]>(type: "text[]", nullable: false),
                    description = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    isActive = table.Column<bool>(type: "boolean", nullable: false),
                    createdBy = table.Column<string>(type: "text", nullable: false),
                    createdOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updatedBy = table.Column<string>(type: "text", nullable: false),
                    updatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    revision = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "auditLogs",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    organizationId = table.Column<long>(type: "bigint", nullable: true),
                    userId = table.Column<long>(type: "bigint", nullable: true),
                    userEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    entityType = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    entityId = table.Column<long>(type: "bigint", nullable: false),
                    action = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    changes = table.Column<string>(type: "jsonb", nullable: true),
                    occurredAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_auditLogs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "organizations",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    slug = table.Column<string>(type: "text", nullable: false),
                    isActive = table.Column<bool>(type: "boolean", nullable: false),
                    createdBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    createdOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updatedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    updatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    revision = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_organizations", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "refreshTokens",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    userId = table.Column<long>(type: "bigint", nullable: false),
                    token = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    expiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    isRevoked = table.Column<bool>(type: "boolean", nullable: false),
                    replacedByToken = table.Column<string>(type: "text", nullable: true),
                    createdBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    createdOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updatedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    updatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    revision = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refreshTokens", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<long>(type: "bigint", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
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
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    fullName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    organizationId = table.Column<long>(type: "bigint", nullable: false),
                    isActive = table.Column<bool>(type: "boolean", nullable: false),
                    createdBy = table.Column<string>(type: "text", nullable: false),
                    createdOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updatedBy = table.Column<string>(type: "text", nullable: false),
                    updatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    revision = table.Column<int>(type: "integer", nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_organizations_organizationId",
                        column: x => x.organizationId,
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "domains",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    organizationId = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    isActive = table.Column<bool>(type: "boolean", nullable: false),
                    createdBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    createdOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updatedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    updatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    revision = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_domains", x => x.id);
                    table.ForeignKey(
                        name: "FK_domains_organizations_organizationId",
                        column: x => x.organizationId,
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
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
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: false)
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
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    RoleId = table.Column<long>(type: "bigint", nullable: false)
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
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
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
                });

            migrationBuilder.CreateTable(
                name: "actorRoles",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    organizationId = table.Column<long>(type: "bigint", nullable: false),
                    domainId = table.Column<long>(type: "bigint", nullable: true),
                    code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    label = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    allowedStages = table.Column<long[]>(type: "bigint[]", nullable: true),
                    isActive = table.Column<bool>(type: "boolean", nullable: false),
                    createdBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    createdOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updatedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    updatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    revision = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_actorRoles", x => x.id);
                    table.ForeignKey(
                        name: "FK_actorRoles_domains_domainId",
                        column: x => x.domainId,
                        principalTable: "domains",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_actorRoles_organizations_organizationId",
                        column: x => x.organizationId,
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    organizationId = table.Column<long>(type: "bigint", nullable: false),
                    domainId = table.Column<long>(type: "bigint", nullable: true),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    sku = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    unitOfMeasure = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    isActive = table.Column<bool>(type: "boolean", nullable: false),
                    createdBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    createdOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updatedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    updatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    revision = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.id);
                    table.ForeignKey(
                        name: "FK_products_domains_domainId",
                        column: x => x.domainId,
                        principalTable: "domains",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_products_organizations_organizationId",
                        column: x => x.organizationId,
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "stages",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    domainId = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    sequence = table.Column<short>(type: "smallint", nullable: false),
                    isRequired = table.Column<bool>(type: "boolean", nullable: false),
                    description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    isActive = table.Column<bool>(type: "boolean", nullable: false),
                    createdBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    createdOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updatedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    updatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    revision = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stages", x => x.id);
                    table.ForeignKey(
                        name: "FK_stages_domains_domainId",
                        column: x => x.domainId,
                        principalTable: "domains",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "actors",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    organizationId = table.Column<long>(type: "bigint", nullable: false),
                    roleId = table.Column<long>(type: "bigint", nullable: true),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    externalId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    contact = table.Column<string>(type: "jsonb", nullable: true),
                    location = table.Column<string>(type: "jsonb", nullable: true),
                    registrationMeta = table.Column<string>(type: "jsonb", nullable: true),
                    isActive = table.Column<bool>(type: "boolean", nullable: false),
                    registeredBy = table.Column<long>(type: "bigint", nullable: true),
                    createdBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    createdOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updatedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    updatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    revision = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_actors", x => x.id);
                    table.ForeignKey(
                        name: "FK_actors_AspNetUsers_registeredBy",
                        column: x => x.registeredBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_actors_actorRoles_roleId",
                        column: x => x.roleId,
                        principalTable: "actorRoles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_actors_organizations_organizationId",
                        column: x => x.organizationId,
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "stageFields",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    stageId = table.Column<long>(type: "bigint", nullable: false),
                    code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    label = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    dataType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    isRequired = table.Column<bool>(type: "boolean", nullable: false),
                    fieldOrder = table.Column<int>(type: "integer", nullable: false),
                    validation = table.Column<string>(type: "jsonb", nullable: true),
                    options = table.Column<string>(type: "jsonb", nullable: true),
                    uiHint = table.Column<string>(type: "jsonb", nullable: true),
                    isActive = table.Column<bool>(type: "boolean", nullable: false),
                    createdBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    createdOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updatedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    updatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    revision = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stageFields", x => x.id);
                    table.ForeignKey(
                        name: "FK_stageFields_stages_stageId",
                        column: x => x.stageId,
                        principalTable: "stages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "batches",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    organizationId = table.Column<long>(type: "bigint", nullable: false),
                    domainId = table.Column<long>(type: "bigint", nullable: false),
                    productId = table.Column<long>(type: "bigint", nullable: true),
                    batchCode = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    originActorId = table.Column<long>(type: "bigint", nullable: true),
                    originDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    originLocation = table.Column<string>(type: "jsonb", nullable: true),
                    initialQuantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    initialUnitPrice = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    currentQuantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    currentStageId = table.Column<long>(type: "bigint", nullable: true),
                    totalValue = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    metadata = table.Column<string>(type: "jsonb", nullable: true),
                    isDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    createdBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    createdOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updatedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    updatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    revision = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_batches", x => x.id);
                    table.ForeignKey(
                        name: "FK_batches_actors_originActorId",
                        column: x => x.originActorId,
                        principalTable: "actors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_batches_domains_domainId",
                        column: x => x.domainId,
                        principalTable: "domains",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_batches_organizations_organizationId",
                        column: x => x.organizationId,
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_batches_products_productId",
                        column: x => x.productId,
                        principalTable: "products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_batches_stages_currentStageId",
                        column: x => x.currentStageId,
                        principalTable: "stages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "credentials",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    actorId = table.Column<long>(type: "bigint", nullable: false),
                    organizationId = table.Column<long>(type: "bigint", nullable: false),
                    type = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    issuer = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    reference = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    issuedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    expiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    isActive = table.Column<bool>(type: "boolean", nullable: false),
                    createdBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    createdOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updatedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    updatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    revision = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_credentials", x => x.id);
                    table.ForeignKey(
                        name: "FK_credentials_actors_actorId",
                        column: x => x.actorId,
                        principalTable: "actors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "batchLineages",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    organizationId = table.Column<long>(type: "bigint", nullable: false),
                    parentBatchId = table.Column<long>(type: "bigint", nullable: false),
                    childBatchId = table.Column<long>(type: "bigint", nullable: false),
                    type = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    notes = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    createdBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    createdOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updatedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    updatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    revision = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_batchLineages", x => x.id);
                    table.ForeignKey(
                        name: "FK_batchLineages_batches_childBatchId",
                        column: x => x.childBatchId,
                        principalTable: "batches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_batchLineages_batches_parentBatchId",
                        column: x => x.parentBatchId,
                        principalTable: "batches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "stageRecords",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    organizationId = table.Column<long>(type: "bigint", nullable: false),
                    batchId = table.Column<long>(type: "bigint", nullable: false),
                    stageId = table.Column<long>(type: "bigint", nullable: false),
                    actorId = table.Column<long>(type: "bigint", nullable: true),
                    quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    unitPrice = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    currency = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    geoPoint = table.Column<string>(type: "jsonb", nullable: true),
                    notes = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    recordedBy = table.Column<long>(type: "bigint", nullable: true),
                    recordedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    validatedBy = table.Column<long>(type: "bigint", nullable: true),
                    validatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    validationNotes = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    isDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    createdBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    createdOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updatedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    updatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    revision = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stageRecords", x => x.id);
                    table.ForeignKey(
                        name: "FK_stageRecords_AspNetUsers_recordedBy",
                        column: x => x.recordedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_stageRecords_AspNetUsers_validatedBy",
                        column: x => x.validatedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_stageRecords_actors_actorId",
                        column: x => x.actorId,
                        principalTable: "actors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_stageRecords_batches_batchId",
                        column: x => x.batchId,
                        principalTable: "batches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_stageRecords_organizations_organizationId",
                        column: x => x.organizationId,
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_stageRecords_stages_stageId",
                        column: x => x.stageId,
                        principalTable: "stages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "evidences",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    organizationId = table.Column<long>(type: "bigint", nullable: false),
                    stageRecordId = table.Column<long>(type: "bigint", nullable: true),
                    credentialId = table.Column<long>(type: "bigint", nullable: true),
                    storageKey = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    originalFilename = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    mimeType = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    sizeBytes = table.Column<long>(type: "bigint", nullable: true),
                    type = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    description = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true),
                    uploadedBy = table.Column<long>(type: "bigint", nullable: true),
                    createdBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    createdOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updatedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    updatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    revision = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_evidences", x => x.id);
                    table.ForeignKey(
                        name: "FK_evidences_AspNetUsers_uploadedBy",
                        column: x => x.uploadedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_evidences_credentials_credentialId",
                        column: x => x.credentialId,
                        principalTable: "credentials",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_evidences_stageRecords_stageRecordId",
                        column: x => x.stageRecordId,
                        principalTable: "stageRecords",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "stageRecordFields",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    stageRecordId = table.Column<long>(type: "bigint", nullable: false),
                    stageFieldId = table.Column<long>(type: "bigint", nullable: false),
                    value = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: true),
                    createdBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    createdOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updatedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    updatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    revision = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stageRecordFields", x => x.id);
                    table.ForeignKey(
                        name: "FK_stageRecordFields_stageFields_stageFieldId",
                        column: x => x.stageFieldId,
                        principalTable: "stageFields",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_stageRecordFields_stageRecords_stageRecordId",
                        column: x => x.stageRecordId,
                        principalTable: "stageRecords",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "valueRecords",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    organizationId = table.Column<long>(type: "bigint", nullable: false),
                    batchId = table.Column<long>(type: "bigint", nullable: false),
                    stageRecordId = table.Column<long>(type: "bigint", nullable: false),
                    stageId = table.Column<long>(type: "bigint", nullable: false),
                    actorId = table.Column<long>(type: "bigint", nullable: true),
                    quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    unitPrice = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    totalValue = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    currency = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    recordedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    validatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    createdBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    createdOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updatedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    updatedOn = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    revision = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_valueRecords", x => x.id);
                    table.ForeignKey(
                        name: "FK_valueRecords_actors_actorId",
                        column: x => x.actorId,
                        principalTable: "actors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_valueRecords_batches_batchId",
                        column: x => x.batchId,
                        principalTable: "batches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_valueRecords_organizations_organizationId",
                        column: x => x.organizationId,
                        principalTable: "organizations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_valueRecords_stageRecords_stageRecordId",
                        column: x => x.stageRecordId,
                        principalTable: "stageRecords",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_valueRecords_stages_stageId",
                        column: x => x.stageId,
                        principalTable: "stages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_actorRoles_domainId",
                table: "actorRoles",
                column: "domainId");

            migrationBuilder.CreateIndex(
                name: "IX_actorRoles_organizationId_code",
                table: "actorRoles",
                columns: new[] { "organizationId", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_actors_organizationId",
                table: "actors",
                column: "organizationId");

            migrationBuilder.CreateIndex(
                name: "IX_actors_organizationId_externalId",
                table: "actors",
                columns: new[] { "organizationId", "externalId" },
                unique: true,
                filter: "\"externalId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_actors_registeredBy",
                table: "actors",
                column: "registeredBy");

            migrationBuilder.CreateIndex(
                name: "IX_actors_roleId",
                table: "actors",
                column: "roleId");

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
                name: "IX_AspNetUsers_Email",
                table: "AspNetUsers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_organizationId",
                table: "AspNetUsers",
                column: "organizationId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_auditLogs_entityType_entityId",
                table: "auditLogs",
                columns: new[] { "entityType", "entityId" });

            migrationBuilder.CreateIndex(
                name: "IX_auditLogs_occurredAt",
                table: "auditLogs",
                column: "occurredAt");

            migrationBuilder.CreateIndex(
                name: "IX_auditLogs_organizationId",
                table: "auditLogs",
                column: "organizationId");

            migrationBuilder.CreateIndex(
                name: "IX_auditLogs_userId",
                table: "auditLogs",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_batches_currentStageId",
                table: "batches",
                column: "currentStageId");

            migrationBuilder.CreateIndex(
                name: "IX_batches_domainId",
                table: "batches",
                column: "domainId");

            migrationBuilder.CreateIndex(
                name: "IX_batches_organizationId_batchCode",
                table: "batches",
                columns: new[] { "organizationId", "batchCode" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_batches_organizationId_domainId_status",
                table: "batches",
                columns: new[] { "organizationId", "domainId", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_batches_organizationId_status",
                table: "batches",
                columns: new[] { "organizationId", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_batches_originActorId",
                table: "batches",
                column: "originActorId");

            migrationBuilder.CreateIndex(
                name: "IX_batches_productId",
                table: "batches",
                column: "productId");

            migrationBuilder.CreateIndex(
                name: "IX_batchLineages_childBatchId",
                table: "batchLineages",
                column: "childBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_batchLineages_parentBatchId",
                table: "batchLineages",
                column: "parentBatchId");

            migrationBuilder.CreateIndex(
                name: "IX_credentials_actorId",
                table: "credentials",
                column: "actorId");

            migrationBuilder.CreateIndex(
                name: "IX_credentials_organizationId",
                table: "credentials",
                column: "organizationId");

            migrationBuilder.CreateIndex(
                name: "IX_domains_organizationId_code",
                table: "domains",
                columns: new[] { "organizationId", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_evidences_credentialId",
                table: "evidences",
                column: "credentialId");

            migrationBuilder.CreateIndex(
                name: "IX_evidences_organizationId_stageRecordId",
                table: "evidences",
                columns: new[] { "organizationId", "stageRecordId" });

            migrationBuilder.CreateIndex(
                name: "IX_evidences_stageRecordId",
                table: "evidences",
                column: "stageRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_evidences_uploadedBy",
                table: "evidences",
                column: "uploadedBy");

            migrationBuilder.CreateIndex(
                name: "IX_organizations_slug",
                table: "organizations",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_products_domainId",
                table: "products",
                column: "domainId");

            migrationBuilder.CreateIndex(
                name: "IX_products_organizationId",
                table: "products",
                column: "organizationId");

            migrationBuilder.CreateIndex(
                name: "IX_products_organizationId_sku",
                table: "products",
                columns: new[] { "organizationId", "sku" },
                unique: true,
                filter: "\"sku\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_refreshTokens_token",
                table: "refreshTokens",
                column: "token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_refreshTokens_userId",
                table: "refreshTokens",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_stageFields_stageId_code",
                table: "stageFields",
                columns: new[] { "stageId", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_stageRecordFields_stageFieldId",
                table: "stageRecordFields",
                column: "stageFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_stageRecordFields_stageRecordId_stageFieldId",
                table: "stageRecordFields",
                columns: new[] { "stageRecordId", "stageFieldId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_stageRecords_actorId",
                table: "stageRecords",
                column: "actorId");

            migrationBuilder.CreateIndex(
                name: "IX_stageRecords_batchId_stageId",
                table: "stageRecords",
                columns: new[] { "batchId", "stageId" });

            migrationBuilder.CreateIndex(
                name: "IX_stageRecords_organizationId_actorId",
                table: "stageRecords",
                columns: new[] { "organizationId", "actorId" });

            migrationBuilder.CreateIndex(
                name: "IX_stageRecords_organizationId_batchId",
                table: "stageRecords",
                columns: new[] { "organizationId", "batchId" });

            migrationBuilder.CreateIndex(
                name: "IX_stageRecords_organizationId_stageId",
                table: "stageRecords",
                columns: new[] { "organizationId", "stageId" });

            migrationBuilder.CreateIndex(
                name: "IX_stageRecords_recordedBy",
                table: "stageRecords",
                column: "recordedBy");

            migrationBuilder.CreateIndex(
                name: "IX_stageRecords_stageId",
                table: "stageRecords",
                column: "stageId");

            migrationBuilder.CreateIndex(
                name: "IX_stageRecords_validatedBy",
                table: "stageRecords",
                column: "validatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_stages_domainId_code",
                table: "stages",
                columns: new[] { "domainId", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_stages_domainId_sequence",
                table: "stages",
                columns: new[] { "domainId", "sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_valueRecords_actorId",
                table: "valueRecords",
                column: "actorId");

            migrationBuilder.CreateIndex(
                name: "IX_valueRecords_batchId",
                table: "valueRecords",
                column: "batchId");

            migrationBuilder.CreateIndex(
                name: "IX_valueRecords_organizationId_actorId",
                table: "valueRecords",
                columns: new[] { "organizationId", "actorId" });

            migrationBuilder.CreateIndex(
                name: "IX_valueRecords_organizationId_batchId",
                table: "valueRecords",
                columns: new[] { "organizationId", "batchId" });

            migrationBuilder.CreateIndex(
                name: "IX_valueRecords_organizationId_stageId",
                table: "valueRecords",
                columns: new[] { "organizationId", "stageId" });

            migrationBuilder.CreateIndex(
                name: "IX_valueRecords_stageId",
                table: "valueRecords",
                column: "stageId");

            migrationBuilder.CreateIndex(
                name: "IX_valueRecords_stageRecordId",
                table: "valueRecords",
                column: "stageRecordId",
                unique: true);
        }

        /// <inheritdoc />
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
                name: "auditLogs");

            migrationBuilder.DropTable(
                name: "batchLineages");

            migrationBuilder.DropTable(
                name: "evidences");

            migrationBuilder.DropTable(
                name: "refreshTokens");

            migrationBuilder.DropTable(
                name: "stageRecordFields");

            migrationBuilder.DropTable(
                name: "valueRecords");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "credentials");

            migrationBuilder.DropTable(
                name: "stageFields");

            migrationBuilder.DropTable(
                name: "stageRecords");

            migrationBuilder.DropTable(
                name: "batches");

            migrationBuilder.DropTable(
                name: "actors");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "stages");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "actorRoles");

            migrationBuilder.DropTable(
                name: "domains");

            migrationBuilder.DropTable(
                name: "organizations");
        }
    }
}
