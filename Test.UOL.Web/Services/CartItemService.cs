using Test.UOL.Web.Interfaces;
using Test.UOL.Web.Entities;

namespace Test.UOL.Web.Services;

public class CartItemService : ICartItemService
{
    private readonly ICartStore _cartStore;
    private readonly ICartTotalCalculator _cartTotalCalculator;

    public CartItemService(ICartStore cartStore, ICartTotalCalculator cartTotalCalculator)
    {
        _cartStore = cartStore;
        _cartTotalCalculator = cartTotalCalculator;
    }


    public Cart PutItemInCart(Guid IdCart, CartItem cartItem)
    {
        if (cartItem == null)
            throw new ArgumentNullException(nameof(cartItem));
        if (cartItem.Product == null)
            throw new ArgumentNullException(nameof(cartItem.Product));


        var cart = _cartStore.GetCartById(IdCart);
        if (cart == null)
        {
            throw new ArgumentException("Cart not found", nameof(IdCart));
        }
        var cartItems = cart.CartItems;
        var existing = cartItems.FirstOrDefault(i => i.Product.Id == cartItem.Product.Id);
        if (existing != null)
        {

            existing.ChangeQuantity(cartItem.Quantity);
            if (cartItem.Quantity <= 0)
            {
                cartItems.Remove(existing);
            }

        }
        else
        {
            if (cartItem.Quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than zero", nameof(cartItem.Quantity));
            }

            var product = new Product(cartItem.Product.Id, cartItem.Product.Name, cartItem.Product.Price);
            var item = new CartItem(product, cartItem.Quantity);
            cart.CartItems.Add(item);
        }


        cart.TotalAmount = _cartTotalCalculator.CalculateTotal(cart);

        return cart;

    }

    public Cart DeleteItem(Guid IdCart, Guid IdCartItem)
    {
        var cart = _cartStore.GetCartById(IdCart);
        if (cart == null)
        {
            throw new ArgumentException("Cart not found", nameof(IdCart));
        }
        var cartItems = cart.CartItems;
        var existing = cartItems.FirstOrDefault(i => i.Id == IdCartItem);
        if (existing == null)
        {
            throw new ArgumentException("Cart item not found", nameof(IdCartItem));
        }

        cartItems.Remove(existing);
        cart.TotalAmount = _cartTotalCalculator.CalculateTotal(cart);

        return cart;
    }


    public IEnumerable<CartItem> GetCartItems(Guid IdCart)
    {
        return _cartStore.GetCartById(IdCart)?.CartItems ?? Enumerable.Empty<CartItem>();
    }

}

