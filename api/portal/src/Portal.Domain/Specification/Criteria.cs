﻿using System.Linq.Expressions;

namespace Portal.Domain.Specification;

public class Criteria<T> where T : class
{
    public Expression<Func<T, bool>>? Filter { get; private set; } = null;
    public Func<IQueryable<T>, IOrderedQueryable<T>>? OrderBy { get; private set; } = null;
    public string? Cursor { get; set; }
    public bool IsAscending { get; private set; } = true;
    public int Skip { get; private set; } = 0;
    public int Take { get; private set; } = 0;
    public string Fields { get; private set; } = string.Empty;
    public string[]? IncludeProperties { get; private set; } = null;
}