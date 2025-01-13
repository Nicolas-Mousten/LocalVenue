using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using LocalVenue.Core;
using LocalVenue.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace LocalVenue.Services;

public class GenericCRUDService<T>
    where T : class
{
    private readonly IDbContextFactory<VenueContext> contextFactory;

    public GenericCRUDService(IDbContextFactory<VenueContext> contextFactory)
    {
        this.contextFactory = contextFactory;
    }

    private IQueryable<T> IncludeProperties(params Expression<Func<T, object>>[] includes)
    {
        var context = contextFactory.CreateDbContext();

        IQueryable<T> query = context.Set<T>();

        if (includes != null)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }
        return query;
    }

    private string FirstCharToUpper(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }
        return $"{char.ToUpper(input[0])}{input[1..]}";
    }

    public async Task<PagedList<T>> GetItems(
        int page,
        int pageSize,
        string? searchParameter,
        string searchProperty,
        params Expression<Func<T, object>>[]? includes
    )
    {
        searchProperty = FirstCharToUpper(searchProperty);
        IQueryable<T> query = IncludeProperties(includes!);

        var property =
            typeof(T).GetProperty(searchProperty)
            ?? throw new ArgumentNullException(
                $"Property '{searchProperty}' does not exist on type '{typeof(T).Name}'"
            );

        if (!string.IsNullOrEmpty(searchParameter))
        {
            query = query.Where(a =>
                EF.Property<string>(a, searchProperty).Contains(searchParameter)
            );
        }

        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        var nextPage = items.Count == pageSize ? (int?)page + 1 : null;

        return new PagedList<T>
        {
            Count = items.Count,
            Results = items,
            Next = nextPage,
        };
    }

    public async Task<T> GetItem(long id, params Expression<Func<T, object>>[]? includes)
    {
        IQueryable<T> query = IncludeProperties(includes!);

        var keyProperty =
            typeof(T)
                .GetProperties()
                .FirstOrDefault(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Any())
            ?? typeof(T)
                .GetProperties()
                .FirstOrDefault(p => p.Name.EndsWith("Id", StringComparison.OrdinalIgnoreCase))
            ?? throw new ArgumentNullException($"No key property found on type '{typeof(T).Name}'");

        var item = await query.SingleOrDefaultAsync(e =>
            EF.Property<long>(e, keyProperty.Name) == id
        );

        if (item == null)
        {
            throw new KeyNotFoundException($"Item with key '{id}' not found.");
        }

        return item;
    }

    public async Task<T> GetItem(
        string searchParameter,
        string searchProperty,
        params Expression<Func<T, object>>[]? includes
    )
    {
        searchProperty = FirstCharToUpper(searchProperty);
        IQueryable<T> query = IncludeProperties(includes!);

        var property =
            typeof(T).GetProperty(searchProperty)
            ?? throw new ArgumentNullException(
                $"Property '{searchProperty}' does not exist on type '{typeof(T).Name}'"
            );

        var items = await query.ToListAsync();
        var item = items.FirstOrDefault(a =>
            property.GetValue(a)?.ToString()?.ToLower().Contains(searchParameter.ToLower()) == true
        );
        return item != null ? item : throw new ArgumentNullException(nameof(item));
    }

    public async Task<T> AddItem(T item, params Expression<Func<T, object>>[]? includes)
    {
        await using var _context = await contextFactory.CreateDbContextAsync();

        IQueryable<T> query = IncludeProperties(includes!);

        var keyProperty =
            typeof(T)
                .GetProperties()
                .FirstOrDefault(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Any())
            ?? typeof(T)
                .GetProperties()
                .FirstOrDefault(p => p.Name.EndsWith("Id", StringComparison.OrdinalIgnoreCase))
            ?? throw new ArgumentNullException($"No key property found on type '{typeof(T).Name}'");

        item.GetType().GetProperty(keyProperty.Name)?.SetValue(item, 0);

        _context.Set<T>().Add(item);
        await _context.SaveChangesAsync();

        return item;
    }

    public async Task<T> UpdateItem(T item, params Expression<Func<T, object>>[]? includes)
    {
        await using var _context = await contextFactory.CreateDbContextAsync();

        IQueryable<T> query = IncludeProperties(includes!);

        var keyProperty =
            typeof(T)
                .GetProperties()
                .FirstOrDefault(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Any())
            ?? typeof(T)
                .GetProperties()
                .FirstOrDefault(p => p.Name.EndsWith("Id", StringComparison.OrdinalIgnoreCase))
            ?? throw new ArgumentNullException($"No key property found on type '{typeof(T).Name}'");

        var keyValue = keyProperty.GetValue(item);
        var dbItem = await GetItem((long)keyValue!, includes);
        if (dbItem == null)
        {
            throw new KeyNotFoundException($"Item with key '{keyValue}' not found.");
        }

        _context.Entry(dbItem).CurrentValues.SetValues(item);
        _context.Entry(dbItem).State = EntityState.Modified;

        await _context.SaveChangesAsync();
        return dbItem;
    }

    public async Task<T> DeleteItem(long id, params Expression<Func<T, object>>[]? includes)
    {
        await using var _context = await contextFactory.CreateDbContextAsync();

        var item = await GetItem(id, includes);
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }
        _context.Set<T>().Remove(item);
        await _context.SaveChangesAsync();

        return item;
    }
}
