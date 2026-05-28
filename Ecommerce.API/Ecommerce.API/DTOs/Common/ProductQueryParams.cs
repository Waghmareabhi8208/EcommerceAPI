namespace Ecommerce.API.DTOs.Common
{
    public class ProductQueryParams : PaginationParams
    {
        public string? Search { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool? InStock { get; set; }
        public string? SortBy { get; set; }
    }
}
