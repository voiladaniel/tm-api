using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TravianManager.Migrations
{
    public partial class Database_v0 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    AccountID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccountType = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    XCoord = table.Column<string>(nullable: true),
                    YCoord = table.Column<string>(nullable: true),
                    UserID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.AccountID);
                });

            migrationBuilder.CreateTable(
                name: "Plans",
                columns: table => new
                {
                    PlanID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    UserID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plans", x => x.PlanID);
                });

            migrationBuilder.CreateTable(
                name: "PlanSettings",
                columns: table => new
                {
                    PlanSettingID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TimeBuffer = table.Column<int>(nullable: false),
                    SafeTime = table.Column<int>(nullable: false),
                    PlanID = table.Column<int>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    IncludeTTA = table.Column<int>(nullable: false),
                    IncludeTTL = table.Column<int>(nullable: false),
                    FakeMessage = table.Column<string>(nullable: true),
                    RealMessage = table.Column<string>(nullable: true),
                    TTLMessage = table.Column<string>(nullable: true),
                    TTAMessage = table.Column<string>(nullable: true),
                    ServerSpeed = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanSettings", x => x.PlanSettingID);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    SettingID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TimeInterval = table.Column<int>(nullable: false),
                    TemplateID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.SettingID);
                });

            migrationBuilder.CreateTable(
                name: "Templates",
                columns: table => new
                {
                    TemplateID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    UserID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Templates", x => x.TemplateID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Username = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    RoleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Attackers",
                columns: table => new
                {
                    AttackerID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccountID = table.Column<int>(nullable: false),
                    TemplateID = table.Column<int>(nullable: false),
                    TournamentSquare = table.Column<int>(nullable: false),
                    TroopSpeed = table.Column<int>(nullable: false),
                    NotBeforeTime = table.Column<string>(nullable: true),
                    SpeedArtifact = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attackers", x => x.AttackerID);
                    table.ForeignKey(
                        name: "FK_Attackers_Accounts_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Accounts",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlanAttackers",
                columns: table => new
                {
                    PlanAttackerID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccountID = table.Column<int>(nullable: false),
                    PlanID = table.Column<int>(nullable: false),
                    TournamentSquare = table.Column<int>(nullable: false),
                    TroopSpeed = table.Column<int>(nullable: false),
                    SpeedArtifact = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanAttackers", x => x.PlanAttackerID);
                    table.ForeignKey(
                        name: "FK_PlanAttackers_Accounts_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Accounts",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Targets",
                columns: table => new
                {
                    TargetID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AccountID = table.Column<int>(nullable: false),
                    PlanID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Targets", x => x.TargetID);
                    table.ForeignKey(
                        name: "FK_Targets_Accounts_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Accounts",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Defenders",
                columns: table => new
                {
                    DefenderID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AttackerID = table.Column<int>(nullable: false),
                    AccountID = table.Column<int>(nullable: false),
                    TemplateID = table.Column<int>(nullable: false),
                    ArrivingTime = table.Column<string>(nullable: true),
                    AttackingTime = table.Column<string>(nullable: true),
                    AttackType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Defenders", x => x.DefenderID);
                    table.ForeignKey(
                        name: "FK_Defenders_Accounts_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Accounts",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Defenders_Attackers_AttackerID",
                        column: x => x.AttackerID,
                        principalTable: "Attackers",
                        principalColumn: "AttackerID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlanDefenders",
                columns: table => new
                {
                    PlanDefenderID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PlanAttackerID = table.Column<int>(nullable: false),
                    AccountID = table.Column<int>(nullable: false),
                    TargetID = table.Column<int>(nullable: false),
                    PlanID = table.Column<int>(nullable: false),
                    ArrivingTime = table.Column<string>(nullable: true),
                    AttackingTime = table.Column<string>(nullable: true),
                    AttackType = table.Column<int>(nullable: false),
                    AttackerConflict = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanDefenders", x => x.PlanDefenderID);
                    table.ForeignKey(
                        name: "FK_PlanDefenders_Accounts_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Accounts",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlanDefenders_PlanAttackers_PlanAttackerID",
                        column: x => x.PlanAttackerID,
                        principalTable: "PlanAttackers",
                        principalColumn: "PlanAttackerID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlanDefenders_Targets_TargetID",
                        column: x => x.TargetID,
                        principalTable: "Targets",
                        principalColumn: "TargetID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attackers_AccountID",
                table: "Attackers",
                column: "AccountID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Defenders_AccountID",
                table: "Defenders",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_Defenders_AttackerID",
                table: "Defenders",
                column: "AttackerID");

            migrationBuilder.CreateIndex(
                name: "IX_PlanAttackers_AccountID",
                table: "PlanAttackers",
                column: "AccountID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlanDefenders_AccountID",
                table: "PlanDefenders",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_PlanDefenders_PlanAttackerID",
                table: "PlanDefenders",
                column: "PlanAttackerID");

            migrationBuilder.CreateIndex(
                name: "IX_PlanDefenders_TargetID",
                table: "PlanDefenders",
                column: "TargetID");

            migrationBuilder.CreateIndex(
                name: "IX_Targets_AccountID",
                table: "Targets",
                column: "AccountID",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Defenders");

            migrationBuilder.DropTable(
                name: "PlanDefenders");

            migrationBuilder.DropTable(
                name: "Plans");

            migrationBuilder.DropTable(
                name: "PlanSettings");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "Templates");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Attackers");

            migrationBuilder.DropTable(
                name: "PlanAttackers");

            migrationBuilder.DropTable(
                name: "Targets");

            migrationBuilder.DropTable(
                name: "Accounts");
        }
    }
}
