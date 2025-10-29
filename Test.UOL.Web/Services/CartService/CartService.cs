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

        public decimal CalculateTotal(Guid Id)
        {
            var cartItems = _cartStore.GetCartItems();

            var total = cartItems.Sum(item => item.Product.Price * item.Quantity);

            return total;
        }


        public void AddItem(CartItemDto cartItem)
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

            var cartItems = _cartStore.GetCartItems();
            var existing = cartItems.FirstOrDefault(i => i.Product.Id == cartItem.Product.ProductId);
            if (existing != null)
                ChangeItem(existing.Id, existing.Quantity + cartItem.Quantity);
            else
                _cartStore.AddItem(
                    new CartItem(
                        new Product(cartItem.Product.ProductId, cartItem.Product.Name, cartItem.Product.Price)
                        , cartItem.Quantity
                    )
                );
        }

        public void ChangeItem(Guid Id, int quantity)
        {
            _cartStore.ChangeItem(Id, quantity);
        }

        public void DeleteItem(Guid Id, int quantity)
        {
            _cartStore.DeleteItem(Id, quantity);
        }
        public IEnumerable<CartItem> GetCartItems()
        {
            return _cartStore.GetCartItems();
        }

    }
}
