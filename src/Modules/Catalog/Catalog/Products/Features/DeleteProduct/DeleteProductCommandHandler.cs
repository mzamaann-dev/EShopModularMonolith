using Catalog.Data;
using Catalog.Products.Exceptions;
using FluentValidation;
using Shared.CQRS;

namespace Catalog.Products.Features.DeleteProduct;

public record DeleteProductCommand(Guid Id) : ICommand<DeleteProductResult>;

public record DeleteProductResult(bool IsSuccess);

public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductCommandValidator()
    {
        RuleFor(f => f.Id).NotEmpty().WithMessage("Id is required");
    }
}

internal class DeleteProductCommandHandler : ICommandHandler<DeleteProductCommand, DeleteProductResult>
{
    private readonly CatalogDbContext _catalogDbContext;

    public DeleteProductCommandHandler(CatalogDbContext catalogDbContext)
    {
        _catalogDbContext = catalogDbContext;
    }
    public async Task<DeleteProductResult> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
    {
        var product = await _catalogDbContext.Products.FindAsync([command.Id], cancellationToken);

        if (product is null) { throw new ProductNotFoundException(command.Id); }

        _catalogDbContext.Products.Remove(product);
        await _catalogDbContext.SaveChangesAsync(cancellationToken);
        return new DeleteProductResult(true);
    }
}
