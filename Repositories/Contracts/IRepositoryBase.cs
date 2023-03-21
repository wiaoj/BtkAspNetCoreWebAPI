using System.Linq.Expressions;

namespace Repositories.Contracts;
public interface IRepositoryBase<Type> {
    // CRUD
    IQueryable<Type> FindAll(Boolean trackChanges);
    IQueryable<Type> FindByCondition(Expression<Func<Type, Boolean>> expression, Boolean trackChanges);
    void Create(Type entity);
    void Update(Type entity);
    void Delete(Type entity);
}