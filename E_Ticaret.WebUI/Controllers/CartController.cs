using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_Ticaret.Business.Abstarct;
using E_Ticaret.DataAccess.Abstract;
using E_Ticaret.Entities.Models;
using E_Ticaret.WebUI.Identity;
using E_Ticaret.WebUI.Models;
using IyzipayCore;
using IyzipayCore.Model;
using IyzipayCore.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace E_Ticaret.WebUI.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private ICartService _cartService;
        private UserManager<ApplicationUser> _userManager;
        private IOrderService _orderService;

        public CartController(ICartService cartService, IOrderService orderService, UserManager<ApplicationUser> userManager)
        {
            _cartService = cartService;
            _userManager = userManager;
            _orderService = orderService;
        }
        public IActionResult Index()
        {
            var cart = _cartService.GetCartByUserId(_userManager.GetUserId(User));//Buradaki User bilgisine göre UserId gelecek
            return View(new CartModel()
            {
                CartId = cart.Id,
                CartItems = cart.CartItems.Select(i => new CartItemModel()
                {
                    CartItemId = i.Id,
                    ProductId = i.Product.Id,
                    Name = i.Product.Name,
                    Price = (decimal)i.Product.Price,
                    ImageUrl = i.Product.ImageUrl,
                    Quantity = i.Quantity
                }).ToList()

            });
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity)//sepete ekleme
        {
            _cartService.AddToCart(_userManager.GetUserId(User), productId, quantity);//burada 3 parametre verdik.userId,urunId,Miktar
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult DeteteFromCart(int productId)//sepetten silme
        {
            _cartService.DeleteFromCart(_userManager.GetUserId(User), productId);
            return RedirectToAction("Index");
        }

        public IActionResult Checkout()
        {
            var cart = _cartService.GetCartByUserId(_userManager.GetUserId(User));
            var orderModel = new OrderModel();
            orderModel.CartModel = new CartModel()
            {
                CartId = cart.Id,
                CartItems = cart.CartItems.Select(i => new CartItemModel()
                {
                    CartItemId = i.Id,
                    ProductId = i.Product.Id,
                    Name = i.Product.Name,
                    Price = (decimal)i.Product.Price,
                    ImageUrl = i.Product.ImageUrl,
                    Quantity = i.Quantity
                }).ToList()

            };
            return View(orderModel);
        }

        [HttpPost]
        public IActionResult Checkout(OrderModel model)
        {

            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);//userıd yi aldım
                var cart = _cartService.GetCartByUserId(userId);
                model.CartModel = new CartModel()
                {
                    CartId = cart.Id,
                    CartItems = cart.CartItems.Select(i => new CartItemModel()
                    {
                        CartItemId = i.Id,
                        ProductId = i.Product.Id,
                        Name = i.Product.Name,
                        Price = (decimal)i.Product.Price,
                        ImageUrl = i.Product.ImageUrl,
                        Quantity = i.Quantity

                    }).ToList()
                };
                //odeme
                var payment = PaymentProcess(model);
                if (payment.Status == "success")
                {
                    SaveOrder(model, payment, userId);
                    //ClearCart(userId);
                    ClearCart(cart.Id.ToString());
                    return View("Success");
                }
            }
            return View(model);
        }

        private void ClearCart(string cartId)
        {
            _cartService.ClearCart(cartId);
        }

        private void SaveOrder(OrderModel model, Payment payment, string userId)//Kaydedilecek bilgiler
        {
            var order = new Order();
            order.OrderNumber = new Random().Next(111111, 999999).ToString();//random 6 basamklı sayı üret ve aralığı bu
            order.OrderState = EnumOrderState.Complated;
            order.PaymentTypes = EnumPaymentTypes.CreditCart;
            order.PaymentId = payment.PaymentId;
            order.ConversationId = payment.ConversationId;
            order.OrderDate = new DateTime();
            order.FirsName = model.FirsName;
            order.LastName = model.LastName;
            order.Email = model.Email;
            order.Phone = model.Phone;
            order.Address = model.Address;
            order.UserId = userId;

            foreach (var item in model.CartModel.CartItems)
            {
                var orderItem = new OrderItem()
                {
                    Price = item.Price,
                    Quantity = item.Quantity,
                    ProductId = item.ProductId
                };
                order.OrderItems.Add(orderItem);
            }
            _orderService.Create(order);

        }

        private Payment PaymentProcess(OrderModel model)
        {
            Options options = new Options();
            options.ApiKey = "sandbox-wNSiluMlNNF1GvOtNAc0AVo3xsDqx9Me";//uygulama yyaınlanırken buralar da gerçek api key ve adres olacak
            options.SecretKey = "sandbox-brs1xuecMhTRvlf0FBzMi6s4OjoL3o5F";
            options.BaseUrl = "https://sandbox-api.iyzipay.com";

            CreatePaymentRequest request = new CreatePaymentRequest();
            request.Locale = Locale.TR.ToString();
            request.ConversationId = Guid.NewGuid().ToString();//burada benzersiz bir sayı ürettik
            request.Price = model.CartModel.TotalPrice().ToString().Split(",")[0]; ;//Fiyatta hata almamak için 0.index teki elemanı aldık.Yani virgüllü kısımı
            request.PaidPrice = model.CartModel.TotalPrice().ToString().Split(",")[0]; ;
            request.Currency = Currency.TRY.ToString();
            request.Installment = 1;
            request.BasketId = model.CartModel.CartId.ToString();
            request.PaymentChannel = PaymentChannel.WEB.ToString();
            request.PaymentGroup = PaymentGroup.PRODUCT.ToString();

            PaymentCard paymentCard = new PaymentCard();
            paymentCard.CardHolderName = model.CardName;
            paymentCard.CardNumber = model.CardNumber;
            paymentCard.ExpireMonth = model.ExpirationMonth;
            paymentCard.ExpireYear = model.ExpirationYear;
            paymentCard.Cvc = model.Cvv;
            paymentCard.RegisterCard = 0;
            request.PaymentCard = paymentCard;

            //paymentCard.CardHolderName = "John Doe";
            //paymentCard.CardNumber = "5528790000000008";
            //paymentCard.ExpireMonth = "12";
            //paymentCard.ExpireYear = "2030";
            //paymentCard.Cvc = "123";

            Buyer buyer = new Buyer();
            buyer.Id = "BY789";
            buyer.Name = "John";
            buyer.Surname = "Doe";
            buyer.GsmNumber = "+905350000000";
            buyer.Email = "email@email.com";
            buyer.IdentityNumber = "74300864791";
            buyer.LastLoginDate = "2015-10-05 12:43:35";
            buyer.RegistrationDate = "2013-04-21 15:12:09";
            buyer.RegistrationAddress = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
            buyer.Ip = "85.34.78.112";
            buyer.City = "Istanbul";
            buyer.Country = "Turkey";
            buyer.ZipCode = "34732";
            request.Buyer = buyer;

            Address shippingAddress = new Address();
            shippingAddress.ContactName = "Jane Doe";
            shippingAddress.City = "Istanbul";
            shippingAddress.Country = "Turkey";
            shippingAddress.Description = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
            shippingAddress.ZipCode = "34742";
            request.ShippingAddress = shippingAddress;

            Address billingAddress = new Address();
            billingAddress.ContactName = "Jane Doe";
            billingAddress.City = "Istanbul";
            billingAddress.Country = "Turkey";
            billingAddress.Description = "Nidakule Göztepe, Merdivenköy Mah. Bora Sok. No:1";
            billingAddress.ZipCode = "34742";
            request.BillingAddress = billingAddress;

            List<BasketItem> basketItems = new List<BasketItem>();
            BasketItem basketItem;

            foreach (var item in model.CartModel.CartItems)
            {
                basketItem = new BasketItem();
                basketItem.Id = item.ProductId.ToString();
                basketItem.Name = item.Name;
                basketItem.Category1 = "Phone";
                basketItem.ItemType = BasketItemType.PHYSICAL.ToString();
                basketItem.Price = item.Price.ToString().Split(",")[0];

                basketItems.Add(basketItem);
            }

            request.BasketItems = basketItems;

            return Payment.Create(request, options);

        }

        public IActionResult GetOrders()//Siparişlerin Listelenmesi
        {
            var orders = _orderService.GetOrders(_userManager.GetUserId(User));
            var orderListModel = new List<OrderListModel>();//Class içinde class yazmıştım. Burada 2 sinede geçiş yaptım
            OrderListModel orderModel;

            foreach (var item in orders)
            {
                orderModel = new OrderListModel();
                orderModel.OrderId = item.Id;
                orderModel.OrderNumber = item.OrderNumber;
                orderModel.OrderDate = item.OrderDate;
                orderModel.OrderNote = item.OrderNote;
                orderModel.Phone = item.Phone;
                orderModel.FirsName = item.FirsName;
                orderModel.LastName = item.LastName;
                orderModel.Email = item.Email;
                orderModel.City = item.City;

                orderModel.OrderItems = item.OrderItems.Select(i => new OrdertItemModel()
                {
                    OrderItemId = i.Id,
                    Name = i.Product.Name,
                    Price = i.Price,
                    Quantity = i.Quantity,
                    ImageUrl = i.Product.ImageUrl

                }).ToList();

                orderListModel.Add(orderModel);

            }
            return View(orderListModel);
        }

    }
}
