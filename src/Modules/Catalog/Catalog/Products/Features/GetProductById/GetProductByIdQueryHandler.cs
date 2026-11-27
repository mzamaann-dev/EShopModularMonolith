using Catalog.Data;
using Catalog.Products.DTOs;
using Catalog.Products.Exceptions;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Shared.CQRS;

namespace Catalog.Products.Features.GetProducts;

public record GetProductByIdQuery(Guid Id) : IQuery<GetProductByIdQueryResult>;
public record GetProductByIdQueryResult(ProductDto productDto);

internal class GetProductByIdQueryHandler : IQueryHandler<GetProductByIdQuery, GetProductByIdQueryResult>
{
    private readonly CatalogDbContext _catalogDbContext;

    public GetProductByIdQueryHandler(CatalogDbContext catalogDbContext)
    {
        _catalogDbContext = catalogDbContext;
    }
    public async Task<GetProductByIdQueryResult> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
    {
        var product = await _catalogDbContext.Products.AsNoTracking().SingleOrDefaultAsync(p => p.Id == query.Id, cancellationToken);
        if (product is null) throw new ProductNotFoundException(query.Id);

        var projectDto = product.Adapt<ProductDto>();
        return new GetProductByIdQueryResult(projectDto);
    }

}
