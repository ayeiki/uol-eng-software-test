using Microsoft.AspNetCore.Mvc;
using Test.UOL.Web.Interfaces;
using Test.UOL.Web.Services;
using FluentValidation;
using Test.UOL.Web.Stores;
using Test.UOL.Web.Services.CartService;

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
app.MapPost("/cart/items", ([FromBody] CartItemDto request, [FromServices] ICartService cartService) =>
{
    try
    {
        cartService.AddItem(request);
        return Results.Ok();
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { Error = ex.Message });
    }
})
.WithName("AddCartItem")
.WithOpenApi();

app.MapGet("/cart/items", ([FromServices] Guid id, [FromServices] ICartService cartService) =>
{
    var cartItems = cartService.GetCartItems();

    return Results.Ok(cartItems);
})
.WithName("GetCartItems")
.WithOpenApi();

app.MapGet("/cart/total", ([FromServices] Guid id, [FromServices] ICartService cartService) =>
{
    var total = cartService.CalculateTotal(id);
    return Results.Ok(new { Total = total });
})
.WithName("GetCartTotal")
.WithOpenApi();

app.Run();
