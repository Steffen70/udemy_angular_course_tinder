using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace API.Helpers.Pagination
{
    public class PagedList<T> : List<T>
    {
        public PagedList()
        {
        }

        public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            PageSize = pageSize;
            TotalCount = count;
            AddRange(items);
        }

        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int itemsPerPage)
        {
            var count = await source.CountAsync();
            var items = await source
                .Skip((pageNumber - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToListAsync();

            return new PagedList<T>(items, count, pageNumber, itemsPerPage);
        }

        public static PagedList<T> Create(IEnumerable<T> source, int pageNumber, int itemsPerPage)
        {
            var count = source.Count();
            var items = source
                .Skip((pageNumber - 1) * itemsPerPage)
                .Take(itemsPerPage).AsEnumerable();

            return new PagedList<T>(items, count, pageNumber, itemsPerPage);
        }
    }
}