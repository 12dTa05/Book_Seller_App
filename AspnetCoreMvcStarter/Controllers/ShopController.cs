using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookSale.Management.Application.Abstracts;
using BookSale.Management.Application.DTOs;
using BookSale.Management.Domain.Setting;
using BookSale.Management.UI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BookSale.Management.UI.Controllers
{
    public class ShopController : Controller
    {
        private readonly IGenreService _genreService;
        private readonly IBookService _bookService;
        public ShopController(IGenreService genreService, IBookService bookService)
        {
            _genreService = genreService;
            _bookService = bookService;
        }

        public async Task<IActionResult> Home(int idx = 1)
        {
            
            int pageSize = CommonConstant.BookPageSize;

            
            ViewBag.CurrentPageIndex = idx;

            var result = await _bookService.GetBooksForSiteAsync(idx, pageSize);

            var books = result.Item1;
            ViewBag.TotalRecords = result.Item2;

            return View(books);
        }
        public async Task<IActionResult> Index(int g = 0, int idx = 1)
        {
            var genres = _genreService.GetGenresListForSiteAsync();
            int pageSize = CommonConstant.BookPageSize;
            
            ViewBag.Genres = genres;
            ViewBag.CurrentGenre = g;
            ViewBag.CurrentPageIndex = idx;

            var result = await _bookService.GetBooksForSiteAsync(g, idx, pageSize);
            
            var books = result.Item1;
            ViewBag.TotalRecords = result.Item2;
            
            return View(books);
        }

        [HttpGet]
        public async Task<IActionResult> GetBooksByPagination(int genre, int pageIndex)
        {
            int pageSize = CommonConstant.BookPageSize;
            var result = await _bookService.GetBooksForSiteAsync(genre, pageIndex, pageSize);
            
            var books = result.Item1;
            var totalRecords = result.Item2;
            var currentDisplayItems = totalRecords - (pageIndex * pageSize) <= 0 
                ? totalRecords 
                : pageIndex * pageSize;
            var isDisableButton = totalRecords - (pageIndex * pageSize) <= 0;

            return Json(new BookForSiteModel
            {
                TotalRecords = totalRecords,
                CurrentDisplayItems = currentDisplayItems,
                IsDisableButton = isDisableButton,
                Books = books
            });
        }
    }
}