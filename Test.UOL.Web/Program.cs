using Microsoft.AspNetCore.Mvc;
using Test.UOL.Web.Interfaces;
using Test.UOL.Web.Services;
using FluentValidation;
using Test.UOL.Web.Stores;
using Test.UOL.Web.Services.CartService;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

// Configurações de serviços
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ICartStore, CartStore>();
builder.Services.AddSingleton<IValidator<CartItemDto>, CartServiceValidator>();
builder.Services.AddSingleton<ICartService, CartService>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Endpoints
app.MapPost("/cart", ( [FromServices] ICartService cartService) =>
{
    try
    {
        var Id = cartService.NewCart();
        return Results.Ok(Id);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { Error = ex.Message });
    }
})
.WithName("NewCart")
.WithOpenApi();

app.MapPost("/cart/{id}/items", ([FromRoute] Guid id, [FromBody] CartItemDto request, [FromServices] ICartService cartService) =>
{
    try
    {
        cartService.AddItem(id, request);
        return Results.Ok();
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { Error = ex.Message });
    }
})
.WithName("AddCartItem")
.WithOpenApi();

app.MapPut("/cart/{id}/items/{idItem}", ([FromRoute] Guid id, [FromRoute] Guid idItem, [FromBody] int quantity, [FromServices] ICartService cartService) =>
{
    try
    {
        cartService.ChangeItem(id, idItem, quantity);
        return Results.Ok();
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { Error = ex.Message });
    }
})
.WithName("ChangeCartItem")
.WithOpenApi();

app.MapGet("/cart/{id}items", ([FromRoute] Guid id, [FromServices] ICartService cartService) =>
{
    var cartItems = cartService.GetCartItems(id);

    return Results.Ok(cartItems);
})
.WithName("GetCartItems")
.WithOpenApi();

app.MapGet("/cart/{id}/total", ([FromRoute] Guid id, [FromServices] ICartService cartService) =>
{
    var total = cartService.CalculateTotal(id);
    return Results.Ok(new { Total = total });
})
.WithName("GetCartTotal")
.WithOpenApi();

app.Run();
