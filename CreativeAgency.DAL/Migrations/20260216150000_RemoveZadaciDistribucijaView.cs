using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CreativeAgency.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RemoveZadaciDistribucijaView : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS ZadaciDistribucijaView;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE VIEW ZadaciDistribucijaView AS
                SELECT 
                    z.Status,
                    z.Prioritet,
                    COUNT(*) AS BrojZadataka,
                    AVG(CASE 
                        WHEN z.DatumZavrsetka IS NOT NULL THEN DATEDIFF(z.DatumZavrsetka, z.DatumKreiranja)
                        ELSE NULL 
                    END) AS ProsecnoVremeZavrsetka
                FROM Zadaci z
                GROUP BY z.Status, z.Prioritet;
            ");
        }
    }
}
