using Microsoft.AspNetCore.Mvc;
using Test.UOL.Web.Entities;
using Test.UOL.Web.Interfaces;
using Test.UOL.Web.Services;
using Test.UOL.Web.Stores;
using Test.UOL.Web.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Configurações de serviços
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ICartStore, CartStore>();
builder.Services.AddSingleton<ICartService, CartService>();
builder.Services.AddSingleton<ICartItemService, CartItemService>();

builder.Services.AddSingleton<CartTotalCalculator>();

#region Dependências de Cupom
var cupomPath = Path.Combine(builder.Environment.ContentRootPath, "files", "cupom.json");
builder.Services.AddSingleton<ICupomProvider>(_ => new JsonCupomProvider(cupomPath));
builder.Services.AddSingleton<IDiscountCalculator, DiscountCalculator>();
builder.Services.AddSingleton<ICupomService, CupomService>();

builder.Services.AddSingleton<ICartTotalCalculator>(sp =>
    new CartTotalWithCupomCalculator(
        sp.GetRequiredService<CartTotalCalculator>(),
        sp.GetRequiredService<ICupomProvider>(),
        sp.GetRequiredService<IDiscountCalculator>()));
#endregion

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

#region Cart
var group = app.MapGroup("cart").WithTags("Cart").WithOpenApi();
group.MapPost("", ([FromServices] ICartService cartService) =>
{
    try
    {
        var cart = cartService.CreateCart();
        return Results.CreatedAtRoute("GetCart", new { id = cart.Id }, cart);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { Error = ex.Message });
    }
})
.WithName("CreateCart")
.Produces<Cart>(StatusCodes.Status201Created)  
.Produces(StatusCodes.Status400BadRequest);    

group.MapGet("{id:guid}", ([FromRoute] Guid id, [FromServices] ICartService cartService) =>
{
    var cart = cartService.GetCartById(id);
    return Results.Ok(cart);
})
.WithName("GetCart")
.Produces<Cart>(StatusCodes.Status200OK)  
.Produces(StatusCodes.Status400BadRequest);
#endregion

#region Cart Items
var itemGroup = app.MapGroup("cart/{id:guid}/items").WithTags("CartItem").WithOpenApi();

itemGroup.MapPost("", (
    [FromRoute] Guid id, 
    [FromBody] CartItemRequest request, 
    [FromServices] ICartItemService cartItemService,
    [FromServices] ICartStore cartStore) => 
{
    try
    {
        var product = new Product(
            request.Product.Id,
            request.Product.Name,
            request.Product.Price
        );
        var cartItem = new CartItem(product, request.Quantity);
        cartItemService.PutItemInCart(id, cartItem);
        // cartItemService.PutItemInCart(id, request);        
        return Results.Ok();
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { Error = ex.Message });
    }
})
.WithName("AddCartItem")
.Produces(StatusCodes.Status200OK)  
.Produces(StatusCodes.Status400BadRequest);

itemGroup.MapDelete("{idItem:guid}", ([FromRoute] Guid id, [FromRoute] Guid idItem, [FromServices] ICartItemService cartItemService) =>
{
    try
    {
        cartItemService.DeleteItem(id, idItem);
        return Results.Ok();
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { Error = ex.Message });
    }
})
.WithName("ChangeCartItem")
.Produces(StatusCodes.Status200OK)  
.Produces(StatusCodes.Status400BadRequest);


itemGroup.MapGet("", ([FromRoute] Guid id, [FromServices] ICartItemService cartItemService) =>
{
    var cartItems = cartItemService.GetCartItems(id);
    return Results.Ok(cartItems);
})
.WithName("GetCartItems")
.Produces<IEnumerable<CartItem>>(StatusCodes.Status200OK)  
.Produces(StatusCodes.Status400BadRequest);
#endregion

#region  Cupom
var cupomGroup = app.MapGroup("cart/{id:guid}/cupom").WithTags("Cupom").WithOpenApi();

cupomGroup.MapPost("", (
    [FromRoute] Guid id,
    [FromBody] CupomRequest body,
    [FromServices] ICupomService cupomService,
    [FromServices] ICartStore cartStore) =>
{
    try
    {
        if (body is null || string.IsNullOrWhiteSpace(body.Code))
            return Results.BadRequest(new { Error = ErrorMessages.CupomInvalid });

        cupomService.ApplyCupomToCart(id, body.Code.Trim());
        var updatedCart = cartStore.GetCartById(id);
        return Results.Ok(updatedCart);
    }
    catch (CupomException ex)
    {
        return Results.BadRequest(new { Error = ex.Message });
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { Error = ex.Message });
    }
    catch (Exception)
    {        
        return Results.StatusCode(StatusCodes.Status500InternalServerError);
    }
})
.WithName("ApplyCupom")
.Produces<Cart>(StatusCodes.Status200OK) 
.Produces(StatusCodes.Status400BadRequest);

cupomGroup.MapDelete("", (
    [FromRoute] Guid id,
    [FromServices] ICupomService cupomService,
    [FromServices] ICartStore cartStore) =>
{
    try
    {
        cupomService.RemoveCupomFromCart(id);
        var updatedCart = cartStore.GetCartById(id);
        return Results.Ok(updatedCart);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { Error = ex.Message });
    }
    catch (Exception)
    {        
        return Results.StatusCode(StatusCodes.Status500InternalServerError);
    }
})
.WithName("RemoveCupom")
.Produces<Cart>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest);
#endregion

app.Run();

public sealed record CupomRequest(string Code);
public record ProductRequest(Guid Id, string Name, decimal Price);
public record CartItemRequest(ProductRequest Product, int Quantity);