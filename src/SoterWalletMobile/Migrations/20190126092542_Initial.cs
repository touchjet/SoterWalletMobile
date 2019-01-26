using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SoterWalletMobile.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Coins",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CoinName = table.Column<string>(nullable: true),
                    CoinShortcut = table.Column<string>(nullable: true),
                    AddressType = table.Column<uint>(nullable: false),
                    MaxfeeKb = table.Column<ulong>(nullable: false),
                    AddressTypeP2sh = table.Column<uint>(nullable: false),
                    AddressTypeP2wpkh = table.Column<uint>(nullable: false),
                    AddressTypeP2wsh = table.Column<uint>(nullable: false),
                    SignedMessageHeader = table.Column<string>(nullable: true),
                    Bip44AccountPath = table.Column<uint>(nullable: false),
                    Forkid = table.Column<uint>(nullable: false),
                    Decimals = table.Column<uint>(nullable: false),
                    ContractAddress = table.Column<byte[]>(nullable: true),
                    GasLimit = table.Column<byte[]>(nullable: true),
                    XpubMagic = table.Column<uint>(nullable: false),
                    XprvMagic = table.Column<uint>(nullable: false),
                    Segwit = table.Column<bool>(nullable: false),
                    ForceBip143 = table.Column<bool>(nullable: false),
                    CurveName = table.Column<string>(nullable: true),
                    CashaddrPrefix = table.Column<string>(nullable: true),
                    Bech32Prefix = table.Column<string>(nullable: true),
                    Decred = table.Column<bool>(nullable: false),
                    VersionGroupId = table.Column<uint>(nullable: false),
                    XpubMagicSegwitP2sh = table.Column<uint>(nullable: false),
                    XpubMagicSegwitNative = table.Column<uint>(nullable: false),
                    Enabled = table.Column<bool>(nullable: false),
                    LastNetworkUpdate = table.Column<DateTime>(nullable: false),
                    BlockHeight = table.Column<ulong>(nullable: false),
                    TotalBalance = table.Column<ulong>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CoinId = table.Column<int>(nullable: false),
                    Purpose = table.Column<uint>(nullable: false),
                    CoinType = table.Column<uint>(nullable: false),
                    Account = table.Column<uint>(nullable: false),
                    Change = table.Column<uint>(nullable: false),
                    AddressIndex = table.Column<uint>(nullable: false),
                    AddressString = table.Column<string>(nullable: true),
                    Balance = table.Column<ulong>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Addresses_Coins_CoinId",
                        column: x => x.CoinId,
                        principalTable: "Coins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AddressId = table.Column<int>(nullable: false),
                    BlockHeight = table.Column<long>(nullable: false),
                    Received = table.Column<DateTime>(nullable: false),
                    Hash = table.Column<string>(nullable: true),
                    Amount = table.Column<long>(nullable: false),
                    Fee = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_CoinId",
                table: "Addresses",
                column: "CoinId");

            migrationBuilder.CreateIndex(
                name: "IX_Coins_CoinName",
                table: "Coins",
                column: "CoinName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Coins_CoinShortcut",
                table: "Coins",
                column: "CoinShortcut",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_AddressId",
                table: "Transactions",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_Hash",
                table: "Transactions",
                column: "Hash",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "Coins");
        }
    }
}
