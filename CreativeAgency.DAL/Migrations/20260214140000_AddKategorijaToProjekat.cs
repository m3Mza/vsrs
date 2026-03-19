using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CreativeAgency.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddKategorijaToProjekat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Kategorija",
                table: "Projekti",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Sajt")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Kategorija",
                table: "Projekti");
        }
    }
}
