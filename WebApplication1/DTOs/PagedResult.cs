namespace WebApplication1.DTOs
{
    // Sử dụng Generic <T> để sau này có thể dùng class này cho việc phân trang cả Bài viết (Blog) hay Báo giá
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; } // Tổng số sản phẩm trong DB
        public int PageSize { get; set; }   // Số sản phẩm trên 1 trang
        public int CurrentPage { get; set; } // Trang hiện tại
        public int TotalPages { get; set; }  // Tổng số trang
    }
}