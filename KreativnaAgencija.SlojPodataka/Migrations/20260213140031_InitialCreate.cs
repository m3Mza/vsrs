using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CreativeAgency.DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Korisnici",
                columns: table => new
                {
                    KorisnikId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    KorisnickoIme = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LozinkaHash = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Ime = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Prezime = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DatumKreiranja = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DatumPoslednjegPrijave = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    JeAktivan = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Korisnici", x => x.KorisnikId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Projekti",
                columns: table => new
                {
                    ProjekatId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Naziv = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Opis = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DatumPocetka = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DatumZavrsetka = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DatumKreiranja = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DatumAzuriranja = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    KreiraoKorisnikId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projekti", x => x.ProjekatId);
                    table.ForeignKey(
                        name: "FK_Projekti_Korisnici_KreiraoKorisnikId",
                        column: x => x.KreiraoKorisnikId,
                        principalTable: "Korisnici",
                        principalColumn: "KorisnikId",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Zadaci",
                columns: table => new
                {
                    ZadatakId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ProjekatId = table.Column<int>(type: "int", nullable: false),
                    Naslov = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Opis = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Prioritet = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RokIzvrsenja = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DatumKreiranja = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DatumAzuriranja = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DodeljenKorisnikId = table.Column<int>(type: "int", nullable: true),
                    KreiraoKorisnikId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zadaci", x => x.ZadatakId);
                    table.ForeignKey(
                        name: "FK_Zadaci_Korisnici_DodeljenKorisnikId",
                        column: x => x.DodeljenKorisnikId,
                        principalTable: "Korisnici",
                        principalColumn: "KorisnikId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Zadaci_Korisnici_KreiraoKorisnikId",
                        column: x => x.KreiraoKorisnikId,
                        principalTable: "Korisnici",
                        principalColumn: "KorisnikId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Zadaci_Projekti_ProjekatId",
                        column: x => x.ProjekatId,
                        principalTable: "Projekti",
                        principalColumn: "ProjekatId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Korisnici",
                columns: new[] { "KorisnikId", "DatumKreiranja", "DatumPoslednjegPrijave", "Email", "Ime", "JeAktivan", "KorisnickoIme", "LozinkaHash", "Prezime" },
                values: new object[] { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "admin@creativeagency.com", "Admin", true, "admin", "$2a$11$vBwNZ0QyQJ7cK9qX8YZN7.sYdJx8J0kY6kQ0Wx8kQwO8yX8yX8yX8", "Korisnik" });

            migrationBuilder.InsertData(
                table: "Projekti",
                columns: new[] { "ProjekatId", "DatumAzuriranja", "DatumKreiranja", "DatumPocetka", "DatumZavrsetka", "KreiraoKorisnikId", "Naziv", "Opis", "Status" },
                values: new object[] { 1, null, new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, "Primer Projekta", "Ovo je primer projekta za demonstraciju", "Aktivan" });

            migrationBuilder.InsertData(
                table: "Zadaci",
                columns: new[] { "ZadatakId", "DatumAzuriranja", "DatumKreiranja", "DodeljenKorisnikId", "KreiraoKorisnikId", "Naslov", "Opis", "Prioritet", "ProjekatId", "RokIzvrsenja", "Status" },
                values: new object[] { 1, null, new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, "Primer Zadatka", "Ovo je primer zadatka", "Srednji", 1, null, "NaCekanju" });

            migrationBuilder.CreateIndex(
                name: "IX_Korisnici_Email",
                table: "Korisnici",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Korisnici_KorisnickoIme",
                table: "Korisnici",
                column: "KorisnickoIme",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projekti_KreiraoKorisnikId",
                table: "Projekti",
                column: "KreiraoKorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_Zadaci_DodeljenKorisnikId",
                table: "Zadaci",
                column: "DodeljenKorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_Zadaci_KreiraoKorisnikId",
                table: "Zadaci",
                column: "KreiraoKorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_Zadaci_ProjekatId",
                table: "Zadaci",
                column: "ProjekatId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Zadaci");

            migrationBuilder.DropTable(
                name: "Projekti");

            migrationBuilder.DropTable(
                name: "Korisnici");
        }
    }
}
