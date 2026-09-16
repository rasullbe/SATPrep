using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SATPrep.Api.DTOs;

public class PaginatedDto<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Skip { get; set; }
    public int Take { get; set; }
    public bool HasMore => Skip + Take < TotalCount;

    public static PaginatedDto<T> From(IEnumerable<T> items, int totalCount, int skip, int take)
    {
        return new PaginatedDto<T>
        {
            Items = new List<T>(items),
            TotalCount = totalCount,
            Skip = skip,
            Take = take
        };
    }
}