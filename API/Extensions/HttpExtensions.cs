using System.Text.Json;
using API.Helpers;
using AutoMapper;
using Microsoft.AspNetCore.Http;

namespace API.Extensions
{
    public static class HttpExtensions
    {
        public static PagedList<T> AddPaginationHeader<T>(this HttpResponse response, PagedList<T> pagedList, IMapper mapper, UserParams userParams)
        {
            var paginationHeader = new PaginationHeader(pagedList.PageNumber, pagedList.PageSize, pagedList.TotalCount, pagedList.TotalPages);
            paginationHeader = mapper.Map(userParams, paginationHeader);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationHeader, options));
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");

            return pagedList;
        }
    }
}