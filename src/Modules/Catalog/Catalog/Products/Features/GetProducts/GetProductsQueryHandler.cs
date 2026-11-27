using Catalog.Data;
using Catalog.Products.DTOs;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Shared.CQRS;
using Shared.Pagination;

namespace Catalog.Products.Features.GetProducts;

public record GetProductsQuery(PaginatedRequest Request) : IQuery<GetProductsQueryResult>;
public record GetProductsQueryResult(PaginatedResult<ProductDto> Products);

internal class GetProductsQueryHandler : IQueryHandler<GetProductsQuery, GetProductsQueryResult>
{
    private readonly CatalogDbContext _catalogDbContext;

    public GetProductsQueryHandler(CatalogDbContext catalogDbContext)
    {
        _catalogDbContext = catalogDbContext;
    }

    public async Task<GetProductsQueryResult> Handle(GetProductsQuery query, CancellationToken cancellationToken)
    {
        var pageNo = query.Request.PageNo;
        var pageSize = query.Request.PageSize;

        var totalCount = await _catalogDbContext.Products.LongCountAsync(cancellationToken);

        var products = await _catalogDbContext.Products
                            .AsNoTracking()
                            .Skip(pageNo - 1 * pageSize)
                            .Take(pageSize)
                            .OrderBy(p => p.Name)
                            .ToListAsync(cancellationToken);

        var projectDtos = products.Adapt<List<ProductDto>>();

        return new GetProductsQueryResult(new PaginatedResult<ProductDto>(pageNo, pageSize, totalCount, projectDtos));
    }

}
