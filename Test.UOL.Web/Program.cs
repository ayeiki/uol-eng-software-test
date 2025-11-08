using Microsoft.AspNetCore.Mvc;
using Test.UOL.Web.Entities;
using Test.UOL.Web.Interfaces;
using Test.UOL.Web.Services;
using Test.UOL.Web.Stores;

var builder = WebApplication.CreateBuilder(args);

// Configurações de serviços
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ICartStore, CartStore>();
builder.Services.AddSingleton<ICartService, CartService>();
builder.Services.AddSingleton<ICartTotalCalculator, CartTotalCalculator>();
builder.Services.AddSingleton<ICartCouponService, CartCouponService>();
builder.Services.AddSingleton<ICartItemService, CartItemService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


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




var itemGroup = app.MapGroup("cart/{id:guid}/items").WithTags("CartItem").WithOpenApi();


itemGroup.MapPost("", ([FromRoute] Guid id, [FromBody] CartItem request, [FromServices] ICartItemService cartItemService) =>
{
    try
    {
        cartItemService.PutItemInCart(id, request);
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


var couponGroup = app.MapGroup("cart/{id:guid}/coupon").WithTags("CartCoupon").WithOpenApi();

couponGroup.MapPost("{couponKey}", ([FromRoute] Guid id, [FromRoute] string couponKey,
    [FromServices] ICartService cartService,
    [FromServices] ICartCouponService couponCalculator) =>
{
    try
    {
        var cart = cartService.GetCartById(id);

        var cartCoupon = couponCalculator.ApplyCouponDiscount(cart, couponKey);

        return Results.Ok(cartCoupon);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { Error = ex.Message });
    }
})
.WithName("ApplyCoupon")
.Produces<Cart>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest);

app.Run();