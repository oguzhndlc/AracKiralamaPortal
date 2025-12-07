using System.Linq.Expressions;

namespace AracKiralamaPortal.Repositories
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll(Func<IQueryable<T>, IQueryable<T>> include = null);
        T Get(int id);
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
        void Add(T entity);
        void Remove(T entity);
        void Update(T entity);
    }
}
