using GreenTrace.Server.Common;
using GreenTrace.Server.Common.Auth;
using GreenTrace.Server.Common.Data;
using Microsoft.EntityFrameworkCore;

namespace GreenTrace.Server.Features.Products;

public class ProductHandler
{
    public static async Task<CallResult> Handle(
        CreateProduct input,
        AppDbContext db,
        AuthContext authContext,
        CancellationToken cancellation)
    {
        if (!authContext.isLoggedIn) return CallResult.NotAuthenticated();
        if (!authContext.hasPermission(Permissions.ProductsCreate)) return CallResult.NotPermitted();

        if (!authContext.isPlatformAdmin && input.organizationId != authContext.organizationId)
            return CallResult.Error("You can only create products in your own organization");

        if (input.domainId.HasValue)
        {
            var domain = await db.domains.FindAsync([input.domainId.Value], cancellation);
            if (domain == null) return CallResult.Error("Domain not found");
            if (domain.organizationId != input.organizationId)
                return CallResult.Error("Domain does not belong to this organization");
        }

        if (input.sku != null)
        {
            var skuExists = await db.products
                .AnyAsync(p => p.organizationId == input.organizationId && p.sku == input.sku, cancellation);
            if (skuExists) return CallResult.Error("A product with this SKU already exists");
        }

        var product = new Product
        {
            organizationId = input.organizationId,
            domainId = input.domainId,
            name = input.name,
            sku = input.sku,
            unitOfMeasure = input.unitOfMeasure,
            description = input.description,
            createdBy = authContext.email ?? "system",
            updatedBy = authContext.email ?? "system"
        };

        db.products.Add(product);
        await db.SaveChangesAsync(cancellation);

        return CallResult.Ok("Product created");
    }

    public static async Task<CallResult> Handle(
        UpdateProduct input,
        AppDbContext db,
        AuthContext authContext,
        CancellationToken cancellation)
    {
        if (!authContext.isLoggedIn) return CallResult.NotAuthenticated();
        if (!authContext.hasPermission(Permissions.ProductsUpdate)) return CallResult.NotPermitted();

        var product = await db.products.FindAsync([input.id], cancellation);
        if (product == null) return CallResult.Error("Product not found");

        if (!authContext.isPlatformAdmin && product.organizationId != authContext.organizationId)
            return CallResult.NotPermitted();

        if (input.name != null) product.name = input.name;
        if (input.unitOfMeasure != null) product.unitOfMeasure = input.unitOfMeasure;
        if (input.description != null) product.description = input.description;
        if (input.isActive.HasValue) product.isActive = input.isActive.Value;

        if (input.sku != null && input.sku != product.sku)
        {
            var skuExists = await db.products
                .AnyAsync(p => p.organizationId == product.organizationId && p.sku == input.sku && p.id != input.id, cancellation);
            if (skuExists) return CallResult.Error("A product with this SKU already exists");
            product.sku = input.sku;
        }

        product.updatedBy = authContext.email ?? "system";
        await db.SaveChangesAsync(cancellation);

        return CallResult.Ok("Product updated");
    }

    public static async Task<CallResult> Handle(
        DeactivateProduct input,
        AppDbContext db,
        AuthContext authContext,
        CancellationToken cancellation)
    {
        if (!authContext.isLoggedIn) return CallResult.NotAuthenticated();
        if (!authContext.hasPermission(Permissions.ProductsDelete)) return CallResult.NotPermitted();

        var product = await db.products.FindAsync([input.id], cancellation);
        if (product == null) return CallResult.Error("Product not found");

        if (!authContext.isPlatformAdmin && product.organizationId != authContext.organizationId)
            return CallResult.NotPermitted();

        product.isActive = false;
        product.updatedBy = authContext.email ?? "system";
        await db.SaveChangesAsync(cancellation);

        return CallResult.Ok("Product deactivated");
    }
}
