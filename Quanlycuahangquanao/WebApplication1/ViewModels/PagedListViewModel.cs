using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.ViewModels
{
    public class PagedListViewModel<T>
    {
        public IEnumerable<T> Items { get; set; }            // Danh sách phần tử trên trang hiện tại
        public int CurrentPage { get; set; }                 // Trang hiện tại (1-based)
        public int PageSize { get; set; }                    // Số phần tử mỗi trang
        public int TotalItems { get; set; }                  // Tổng số phần tử
        public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}