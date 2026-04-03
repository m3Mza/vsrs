using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using CreativeAgency.DAL;
using System.Data;
using MySqlConnector;

namespace CreativeAgency.Repository
{
    public class Repozitorijum<T> : IRepozitorijum<T> where T : class
    {
        protected readonly ApplicationDbContext _kontekst;
        protected readonly DbSet<T> _dbSkup;
        protected readonly DBUtils _dbUtils;

        public Repozitorijum(ApplicationDbContext kontekst)
        {
            _kontekst = kontekst;
            _dbSkup = kontekst.Set<T>();
            _dbUtils = new DBUtils(kontekst);
        }

        public virtual async Task<T?> PreuzmiPoIdAsync(int id)
        {
            return await _dbSkup.FindAsync(id);
        }

        public virtual async Task<IEnumerable<T>> PreuzmiSveAsync()
        {
            return await _dbSkup.ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> PronadjiAsync(Expression<Func<T, bool>> uslov)
        {
            return await _dbSkup.Where(uslov).ToListAsync();
        }

        public virtual async Task<T> DodajAsync(T entitet)
        {
            await _dbSkup.AddAsync(entitet);
            await _kontekst.SaveChangesAsync();
            return entitet;
        }

        public virtual async Task AzurirajAsync(T entitet)
        {
            _dbSkup.Update(entitet);
            await _kontekst.SaveChangesAsync();
        }

        public virtual async Task ObrisiAsync(T entitet)
        {
            _dbSkup.Remove(entitet);
            await _kontekst.SaveChangesAsync();
        }

        public virtual async Task<bool> PostojiAsync(Expression<Func<T, bool>> uslov)
        {
            return await _dbSkup.AnyAsync(uslov);
        }

        public virtual async Task<int> PrebrојAsync()
        {
            return await _dbSkup.CountAsync();
        }

        protected async Task<List<TResult>> IzvrsiStoredProceduru<TResult>(string procedureName, params MySqlParameter[] parameters) where TResult : class
        {
            var paramString = string.Join(", ", parameters.Select(p => p.ParameterName));
            var sql = $"CALL {procedureName}({paramString})";
            
            return await _kontekst.Set<TResult>()
                .FromSqlRaw(sql, parameters)
                .ToListAsync();
        }

        protected async Task<TResult?> IzvrsiStoredProceduruJedan<TResult>(string procedureName, params MySqlParameter[] parameters) where TResult : class
        {
            var rezultati = await IzvrsiStoredProceduru<TResult>(procedureName, parameters);
            return rezultati.FirstOrDefault();
        }
    }
}
