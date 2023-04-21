﻿using System.Linq.Expressions;

namespace Portal.Domain.Interfaces.Common;

public interface IRepository<T> where T : class
{
    public void Insert(T entity);
    public void Update(T entity);
    public void Delete(T entity);
    public void Delete(Expression<Func<T, bool>> where);
    public bool TryGetById(object? id, out T? entity);
    public int Count();
    public IEnumerable<T> GetList(
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        string[]? includeProperties = null,
        int skip = 0,
        int take = 0,
        string fields = "");
    public IEnumerable<T> GetAll();
    public IEnumerable<T> GetMany(Expression<Func<T, bool>> where);
    public bool Any(Expression<Func<T, bool>> where);
}