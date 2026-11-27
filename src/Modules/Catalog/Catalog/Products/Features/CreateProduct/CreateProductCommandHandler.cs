using Catalog.Data;
using Catalog.Products.DTOs;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared.CQRS;

namespace Catalog.Products.Features.CreateProduct;

public record CreateProductCommand(ProductDto Product) : ICommand<CreateProductResult>;
public record CreateProductResult(Guid Id);

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(f => f.Product.Name).NotEmpty().WithMessage("Name is required");
        RuleFor(f => f.Product.Category).NotEmpty().WithMessage("Category is required");
        RuleFor(f => f.Product.ImageFile).NotEmpty().WithMessage("ImageFile is required");
        RuleFor(f => f.Product.Price).GreaterThan(0).WithMessage("Price cannot be negative");
    }
}


public class CreateProductCommandHandler : ICommandHandler<CreateProductCommand, CreateProductResult>
{
    private readonly CatalogDbContext _catalogDbContext;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    public CreateProductCommandHandler(CatalogDbContext catalogDbContext, ILogger<CreateProductCommandHandler> logger)
    {
        _catalogDbContext = catalogDbContext;
        _logger = logger;
    }
    public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("CreateProductCommandHandler.Handle called with {@Command}", command);

        var product = CreateNewProduct(command.Product);
        await _catalogDbContext.Products.AddAsync(product, cancellationToken);
        await _catalogDbContext.SaveChangesAsync(cancellationToken);
        return new CreateProductResult(product.Id);
    }

    private static Product CreateNewProduct(ProductDto productDto)
    {
        var product = Product.Create(Guid.NewGuid(), productDto.Name, productDto.Category, productDto.Description, productDto.Price, productDto.ImageFile);

        return product;
    }
}
