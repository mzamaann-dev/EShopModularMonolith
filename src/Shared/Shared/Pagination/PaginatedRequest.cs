namespace Shared.Pagination;

public record PaginatedRequest(int PageNo = 1, int PageSize = 10);
