using Cai;
using GreenTrace.Server.Common;

namespace GreenTrace.Server.Features.Products;

[Mutation<CallResult>]
public record CreateProduct(long organizationId, long? domainId, string name, string? sku, string? unitOfMeasure, string? description);

[Mutation<CallResult>]
public record UpdateProduct(long id, string? name, string? sku, string? unitOfMeasure, string? description, bool? isActive);

[Mutation<CallResult>]
public record DeactivateProduct(long id);
