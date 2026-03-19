using System.Linq.Expressions;

namespace CreativeAgency.Repository
{
    public interface IRepozitorijum<T> where T : class
    {
        Task<T?> PreuzmiPoIdAsync(int id);
        Task<IEnumerable<T>> PreuzmiSveAsync();
        Task<IEnumerable<T>> PronadjiAsync(Expression<Func<T, bool>> uslov);
        Task<T> DodajAsync(T entitet);
        Task AzurirajAsync(T entitet);
        Task ObrisiAsync(T entitet);
        Task<bool> PostojiAsync(Expression<Func<T, bool>> uslov);
        Task<int> PrebrојAsync();
    }
}
