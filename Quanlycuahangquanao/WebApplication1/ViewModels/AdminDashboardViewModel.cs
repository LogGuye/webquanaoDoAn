using System.Collections.Generic;

namespace WebApplication1.ViewModels
{
    public class AdminDashboardViewModel
    {
        // Tổng doanh thu cả năm
        public double TotalRevenue { get; set; }

        // Doanh thu trong tháng hiện tại
        public double MonthlyRevenue { get; set; }

        // Tổng số khách hàng
        public int TotalCustomers { get; set; }

        // Số khách hàng trong tháng
        public int MonthlyCustomers { get; set; }

        // Tổng số đơn hàng
        public int TotalOrders { get; set; }

        // Số đơn hàng trong tháng
        public int MonthlyOrders { get; set; }

        // Doanh thu 4 năm gần nhất: Key = năm, Value = doanh thu
        public Dictionary<int, double?> RevenueByYear { get; set; }

        // Số sản phẩm theo chủ đề: Key = tên chủ đề, Value = số sản phẩm
        public Dictionary<string, int> CategoryProductCounts { get; set; }
        public Dictionary<string, int> CategoryStats { get; set; }
    }
}
