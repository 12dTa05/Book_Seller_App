using BookSale.Management.Application.Abstracts;
using BookSale.Management.Application.DTOs;
using BookSale.Management.Application.DTOs.ViewsModel;
using BookSale.Management.Application.Services;
using BookSale.Management.Domain.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace BookSale.Management.UI.Areas.Admin.Controllers
{
    public class BookController : BaseController
    {
        private readonly IBookService _bookService;
        private readonly IGenreService _genreService;
        private readonly IImageService _imageService;
        public BookController(IBookService bookService, IGenreService genreService, IImageService imageService)
        {
            _bookService = bookService;
            _genreService = genreService;
            _imageService = imageService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> SaveData(int id)
        {
            try {
                var bookVM = new BookViewModel();
                var genres = await _genreService.GetGenresForDropdownlistAsync();
                ViewBag.Genres = genres;

                string code = await _bookService.GenerateCodeAsync();
                bookVM.Code = code;

                if(id != 0){
                    bookVM = await _bookService.GetBooksByIdAsync(id);
                    if (bookVM == null) {
                        return RedirectToAction("Index");
                    }
                }

                return View(bookVM);
            }
            catch (Exception ex) {
                // Log error
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveData(BookViewModel bookViewModel)
        {
            if (ModelState.IsValid)
            {
                var result = await _bookService.SaveAsync(bookViewModel);

                if (result.Status)
                {
                    if (bookViewModel.Image != null)
                    {
                        await _imageService.SaveImage(
                            new List<IFormFile> { bookViewModel.Image }, 
                            "img/books", 
                            $"{result.Data}.png"
                        );
                    }
                    return RedirectToAction("Index", "Book");
                }
                ModelState.AddModelError("error", result.Message);
            }
            
            var genres = await _genreService.GetGenresForDropdownlistAsync();
            ViewBag.Genres = genres;
            
            return View(bookViewModel);
        }

        public IActionResult Test()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetBooksPagination(RequestDatatable requestDatatable)
        {
            var result = await _bookService.GetBooksByPaginationAsync(requestDatatable);
            return Json(result);
        }
        [HttpGet]
        public async Task<IActionResult> GenerateCodeBook(){
            string result = await _bookService.GenerateCodeAsync();
            return Json(result);
        }
    }
}
