using CreativeAgency.Models;

namespace CreativeAgency.Web.ViewModels
{
    public class ListaProjekataViewModel
    {
        public IEnumerable<Projekat> Projekti { get; set; } = new List<Projekat>();
        public bool MozeUredjivati { get; set; }
        public int UkupnoProjekta { get; set; }
    }
}
