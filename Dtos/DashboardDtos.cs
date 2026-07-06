namespace TheNoir.Api.Dtos;

public record DailyRevenue(DateOnly Date, int RevenueVnd);

public record ProductSales(string ProductName, int QuantitySold, int RevenueVnd);

public record CategoryRevenue(string CategoryName, int RevenueVnd);

public record DashboardStatsResponse(
    int TotalRevenueVnd,
    int TotalOrders,
    List<DailyRevenue> RevenueByDay,
    List<ProductSales> TopProducts,
    List<CategoryRevenue> RevenueByCategory);
