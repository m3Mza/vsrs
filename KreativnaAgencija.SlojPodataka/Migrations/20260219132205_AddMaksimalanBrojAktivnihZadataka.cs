using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CreativeAgency.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddMaksimalanBrojAktivnihZadataka : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ZadaciDistribucijaView");

            migrationBuilder.AddColumn<int>(
                name: "MaksimalanBrojAktivnihZadataka",
                table: "Korisnici",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Korisnici",
                keyColumn: "KorisnikId",
                keyValue: 1,
                column: "MaksimalanBrojAktivnihZadataka",
                value: 5);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaksimalanBrojAktivnihZadataka",
                table: "Korisnici");

            migrationBuilder.CreateTable(
                name: "ZadaciDistribucijaView",
                columns: table => new
                {
                    Status = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Prioritet = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BrojZadataka = table.Column<int>(type: "int", nullable: false),
                    Procenat = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    ProsecnoDanaDoRoka = table.Column<decimal>(type: "decimal(65,30)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZadaciDistribucijaView", x => new { x.Status, x.Prioritet });
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
