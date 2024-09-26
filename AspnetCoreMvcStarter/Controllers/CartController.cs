using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookSale.Management.Application.Abstracts;
using BookSale.Management.Domain.Setting;
using BookSale.Management.UI.Models;
using BookSale.Management.UI.Ulity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookSale.Management.UI.Controllers
{

    public class CartController : Controller
    {
        
        private readonly IBookService _bookService;
        public CartController(IBookService bookService)
        {
            _bookService = bookService;
        }
        public async Task<IActionResult> Index()
        {
            var carts = HttpContext.Session.Get<List<CartModel>>(CommonConstant.SessionCart);

            if (carts is not null)
            {
                ViewData["NumberCart"] = carts.Count;
                var codes = carts.Select(x => x.BookCode).ToArray();

                var books = await _bookService.GetBooksByListCodeAsync(codes);

                books = books.Select(book =>
                {
                    var item = carts.FirstOrDefault(x => x.BookCode == book.Code);
                    if (item is not null)
                    {
                        book.Quantity = item.Quantity;
                    }
                    return book;
                });

                return View(books);
            }
            return View();
        }
        [HttpPost]
        public IActionResult UpdateQuantity(string bookCode, int quantity)
        {
            try
            {
                var carts = HttpContext.Session.Get<List<CartModel>>(CommonConstant.SessionCart) ?? new List<CartModel>();
                var cartItem = carts.FirstOrDefault(x => x.BookCode == bookCode);

                if (cartItem != null)
                {
                    cartItem.Quantity = quantity;
                    HttpContext.Session.Set(CommonConstant.SessionCart, carts);
                    return Json(new { success = true });
                }

                return Json(new { success = false });
            }
            catch
            {
                return Json(new { success = false });
            }
        }
        [HttpPost]
        public IActionResult Add(CartModel cart)
        {
            try
            {
                var carts = HttpContext.Session.Get<List<CartModel>>(CommonConstant.SessionCart) ?? new List<CartModel>();

                var cartExist = carts.FirstOrDefault(x => x.BookCode == cart.BookCode);
                if (cartExist != null)
                {
                    cartExist.Quantity += cart.Quantity;
                }
                else
                {
                    carts.Add(cart);
                }

                HttpContext.Session.Set(CommonConstant.SessionCart, carts);
                return Json(carts.Count);
            }
            catch (Exception)
            {
                return Json(-1);
            }
        }
        [HttpPost]
        [HttpPost]
        public IActionResult Update([FromBody] List<CartModel> cartItems)  // Thêm [FromBody]
        {
            try
            {
                if (cartItems != null && cartItems.Any())
                {
                    var carts = HttpContext.Session.Get<List<CartModel>>(CommonConstant.SessionCart);
                    if (carts != null)
                    {
                        foreach (var item in carts)
                        {
                            var updateItem = cartItems.FirstOrDefault(x => x.BookCode == item.BookCode);
                            if (updateItem != null)
                            {
                                item.Quantity = updateItem.Quantity;
                            }
                        }
                        HttpContext.Session.Set(CommonConstant.SessionCart, carts);
                        return Json(true);
                    }
                }
                return Json(false);
            }
            catch (Exception)
            {
                return Json(false);
            }
        }
        [HttpPost]
        public IActionResult Remove(string code)
        {
            try
            {
                var carts = HttpContext.Session.Get<List<CartModel>>(CommonConstant.SessionCart) ?? new List<CartModel>();
                var cartItem = carts.FirstOrDefault(x => x.BookCode == code);

                if (cartItem != null)
                {
                    carts.Remove(cartItem);
                    HttpContext.Session.Set(CommonConstant.SessionCart, carts);
                    return Json(new { success = true });
                }

                return Json(new { success = false });
            }
            catch
            {
                return Json(new { success = false });
            }
        }
    }
}