using Catalog.Data;
using Catalog.Products.DTOs;
using Catalog.Products.Exceptions;
using FluentValidation;
using Shared.CQRS;

namespace Catalog.Products.Features.UpdateProduct;

public record UpdateProductCommand(ProductDto Product) : ICommand<UpdateProductResult>;
public record UpdateProductResult(bool IsSuccess);

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(f => f.Product.Id).NotEmpty().WithMessage("Id is required");
        RuleFor(f => f.Product.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(f => f.Product.Price).GreaterThan(0).WithMessage("Price cannot be 0 or negative").LessThan(decimal.MaxValue).WithMessage("Price cannot be more than max value");
    }
}

internal class UpdateProductCommandHandler : ICommandHandler<UpdateProductCommand, UpdateProductResult>
{
    private readonly CatalogDbContext _catalogDbContext;

    public UpdateProductCommandHandler(CatalogDbContext catalogDbContext)
    {
        _catalogDbContext = catalogDbContext;
    }
    public async Task<UpdateProductResult> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        var product = await _catalogDbContext.Products.FindAsync([command.Product.Id], cancellationToken);

        if (product is null) { throw new ProductNotFoundException(command.Product.Id); }

        UpdateProductWithNewValues(product, command.Product);
        _catalogDbContext.Products.Update(product);
        await _catalogDbContext.SaveChangesAsync(cancellationToken);

        return new UpdateProductResult(true);

    }

    private static void UpdateProductWithNewValues(Product product, ProductDto productDto)
    {
        product.Update(productDto.Name, productDto.Category, productDto.Description, productDto.ImageFile, productDto.Price);
    }
}
