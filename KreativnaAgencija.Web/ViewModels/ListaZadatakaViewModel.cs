using CreativeAgency.Models;

namespace CreativeAgency.Web.ViewModels
{
    public class ListaZadatakaViewModel
    {
        public IEnumerable<Zadatak> Zadaci { get; set; } = new List<Zadatak>();
        public int? ProjekatId { get; set; }
        public string? NazivProjekta { get; set; }
    }
}
