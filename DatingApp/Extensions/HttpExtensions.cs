using DatingApp.Helpers;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace DatingApp.Extensions
{
    public static class HttpExtensions
    {
        public static void AddPaginationHeader<T>(this HttpResponse response, PagedList<T> data)
        {
            var paginationHeader = new PaginationHeader(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // تبدیل نام‌ها به camelCase
            };

            response.Headers.Append("Pagination", JsonSerializer.Serialize(paginationHeader, jsonOptions));
            response.Headers.Append("Access-Control-Expose-Headers", "Pagination"); // اجازه دسترسی در فرانت‌اند
        }
    }
}
