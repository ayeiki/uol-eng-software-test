using Test.UOL.Web.Interfaces;
using Test.UOL.Web.Entities;
using FluentValidation;

namespace Test.UOL.Web.Services.CartService
{
    public class CartService : ICartService
    {
        private readonly ICartStore _cartStore;
        private readonly IValidator<CartItemDto> _validator;
        public CartService(ICartStore cartStore, IValidator<CartItemDto> validator)
        {
            _cartStore = cartStore;
            _validator = validator;
        }

        public decimal CalculateTotal(Guid IdCart)
        {
            var cartItems = GetCartItems(IdCart);

            var total = cartItems.Sum(item => item.Product.Price * item.Quantity);

            return total;
        }

        public Guid NewCart()
        {
            return _cartStore.NewCart().Id;
        }


        public void AddItem(Guid IdCart, CartItemDto cartItem)
        {
            if (cartItem == null)
                throw new ArgumentNullException(nameof(cartItem));
            if (cartItem.Product == null)
                throw new ArgumentNullException(nameof(cartItem.Product));

            var validationResult = _validator.Validate(cartItem);
            if (!validationResult.IsValid)
            {
                var errorMsg = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new ArgumentException(errorMsg);
            }

            var cartItems = GetCartItems(IdCart);
            var existing = cartItems.FirstOrDefault(i => i.Product.Id == cartItem.Product.ProductId);
            if (existing != null)
                ChangeItem(IdCart, existing.Id, existing.Quantity + cartItem.Quantity);
            else
                _cartStore.AddItem(
                    IdCart,
                    new CartItem(
                        new Product(cartItem.Product.ProductId, cartItem.Product.Name, cartItem.Product.Price)
                        , cartItem.Quantity
                    )
                );
        }

        public void ChangeItem(Guid IdCart, Guid IdCartItem, int quantity)
        {
            _cartStore.ChangeItem(IdCart, IdCartItem, quantity);
        }

        public void DeleteItem(Guid IdCart, Guid IdCartItem, int quantity)
        {
            _cartStore.DeleteItem(IdCart, IdCartItem);
        }
        public IEnumerable<CartItem> GetCartItems(Guid IdCart)
        {
            return _cartStore.GetCartItems(IdCart);
        }

    }
}
