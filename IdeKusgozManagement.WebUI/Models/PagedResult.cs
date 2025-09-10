using Newtonsoft.Json;

namespace IdeKusgozManagement.WebUI.Models
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Data { get; set; } = Enumerable.Empty<T>();

        public int TotalCount { get; set; }

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalPages { get; set; }

        public bool HasNext { get; set; }

        public bool HasPrevious { get; set; }
    }
}