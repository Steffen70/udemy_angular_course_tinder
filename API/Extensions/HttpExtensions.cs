using System.Text.Json;
using API.Helpers.Pagination;
using AutoMapper;
using Microsoft.AspNetCore.Http;

namespace API.Extensions
{
    public static class HttpExtensions
    {
        public static PagedList<TList> AddPaginationHeader<THeader, TList, TParams>(this HttpResponse response, PagedList<TList> pagedList, IMapper mapper, TParams paginationParams)
            where TParams : PaginationParams
            where THeader : PaginationHeader, new()
        {
            THeader paginationHeader = new THeader();
            paginationHeader.Populate(pagedList);

            paginationHeader = mapper.Map(paginationParams, paginationHeader);

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