using Championship_Control_System.DataAccess;
using Championship_Control_System.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Championship_Control_System.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _db;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _db = _context.Set<T>();
        }

        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            var result = await _db.AddAsync(entity, cancellationToken);
            return result.Entity;
        }

        public void Update(T entity)
        {
            _db.Update(entity);
        }

        public void Delete(T entity)
        {
            _db.Remove(entity);
        }

      
        public async Task<IEnumerable<T>> GetAsync(
            Expression<Func<T, bool>>? expression = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Func<IQueryable<T>, IQueryable<T>>? include = null, 
            bool tracked = true,
            CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = _db;

            if (expression is not null)
                query = query.Where(expression);

            if (include is not null)
            {
                query = include(query);
            }
            if (orderBy is not null)
            {
                return await orderBy(query).ToListAsync(cancellationToken);
            }

            if (!tracked)
                query = query.AsNoTracking();

            return await query.ToListAsync(cancellationToken);
        }

       
        public async Task<T?> GetOneAsync(
            Expression<Func<T, bool>>? expression = null,
            Func<IQueryable<T>, IQueryable<T>>? include = null, 
            bool tracked = true,
            CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = _db;

            if (expression is not null)
                query = query.Where(expression);

            if (include is not null)
            {
                query = include(query);
            }

            if (!tracked)
                query = query.AsNoTracking();

            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task CommitAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}