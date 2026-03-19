using CreativeAgency.Models;

namespace CreativeAgency.Web.ViewModels
{
    public class DetaljiProjektaViewModel
    {
        public Projekat Projekat { get; set; } = null!;
        public IEnumerable<Zadatak> Zadaci { get; set; } = new List<Zadatak>();
        public bool MozeUredjivati { get; set; }
    }
}
