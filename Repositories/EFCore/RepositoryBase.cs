using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;
using System.Linq.Expressions;

namespace Repositories.EFCore;
public abstract class RepositoryBase<Type> : IRepositoryBase<Type> where Type : class {
    protected readonly RepositoryContext context;

    public RepositoryBase(RepositoryContext context) {
        this.context = context;
    }

    public void Create(Type entity) {
        this.context.Set<Type>().Add(entity);
    }

    public void Delete(Type entity) {
        this.context.Set<Type>().Remove(entity);
    }

    public IQueryable<Type> FindAll(Boolean trackChanges) {
        return trackChanges ? this.context.Set<Type>() : this.context.Set<Type>().AsNoTracking();
    }

    public IQueryable<Type> FindByCondition(Expression<Func<Type, Boolean>> expression, Boolean trackChanges) {
        return trackChanges ? this.context.Set<Type>().Where(expression) : this.context.Set<Type>().Where(expression).AsNoTracking();
    }

    public void Update(Type entity) {
        this.context.Set<Type>().Update(entity);
    }
}