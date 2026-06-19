using Microsoft.EntityFrameworkCore;
using ViveroLosFrutales.Application.Common;

namespace ViveroLosFrutales.Infrastructure.Data;

internal static class RepositoryHelpers
{
    public static async Task<PagedResult<T>> ToPagedAsync<T>(this IQueryable<T> query, SearchRequest request, CancellationToken cancellationToken)
    {
        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 100 : Math.Min(request.PageSize, 100);
        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return new PagedResult<T> { Items = items, Total = total, Page = page, PageSize = pageSize };
    }
}
