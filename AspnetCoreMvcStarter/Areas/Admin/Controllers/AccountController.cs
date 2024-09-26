using BookSale.Management.Application.Abstracts;
using BookSale.Management.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BookSale.Management.UI.Areas.Admin.Controllers
{
  
  public class AccountController : BaseController
  {
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;

    public AccountController(IUserService userService, IRoleService roleService)
    {
      _userService = userService;
      _roleService = roleService;
    }

    public async Task<IActionResult> Index()
    {
      var account = new AccountDTO();
      ViewBag.Roles = await _roleService.GetRolesForDropdownlist();
      return View(account);
    }

    [HttpPost]
    public async Task<IActionResult> GetAccountPagination(RequestDatatable requestDatatable)
    {
      try
      {
        var data = await _userService.GetUserByPagination(requestDatatable);
        return Json(data);
      }
      catch
      {
        return Json(new { error = "Có lỗi xảy ra khi tải dữ liệu" });
      }
    }

    [HttpGet]
    public async Task<IActionResult> SaveData(string? id)
    {
      AccountDTO accountDto = !string.IsNullOrEmpty(id) ? await _userService.GetUserById(id) : new AccountDTO();
      ViewBag.Roles = await _roleService.GetRolesForDropdownlist();
      return View(accountDto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveData(AccountDTO accountDTO)
    {
      try
      {
        if (!ModelState.IsValid)
        {
          ViewBag.Roles = await _roleService.GetRolesForDropdownlist();
          return View(accountDTO);
        }

        // Kiểm tra và xử lý file Avatar
        if (accountDTO.Avatar != null)
        {
          var validFormats = new[] { ".jpg", ".jpeg", ".png" };
          var extension = Path.GetExtension(accountDTO.Avatar.FileName).ToLowerInvariant();

          if (!validFormats.Contains(extension))
          {
            ModelState.AddModelError("Avatar", "Chỉ chấp nhận định dạng JPG, JPEG hoặc PNG.");
          }
          else if (accountDTO.Avatar.Length > 2 * 1024 * 1024)
          {
            ModelState.AddModelError("Avatar", "Ảnh vượt quá kích thước 2MB.");
          }
        }

        if (!ModelState.IsValid)
        {
          ViewBag.Roles = await _roleService.GetRolesForDropdownlist();
          return View(accountDTO);
        }

        // Gửi dữ liệu đến UserService để lưu
        var result = await _userService.Save(accountDTO);

        if (result.Status)
        {
          TempData["SuccessMessage"] = "Tài khoản đã được lưu thành công.";
          return RedirectToAction("Index");
        }

        // Nếu lưu thất bại, trả về lỗi
        ModelState.AddModelError(string.Empty, result.Message);
        ViewBag.Roles = await _roleService.GetRolesForDropdownlist();
        return View(accountDTO);
      }
      catch (Exception ex)
      {
        ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi không mong muốn.");
        return View(accountDTO);
      }
    }



    

    [HttpPost]
    public async Task<IActionResult> DeleteAsync(string id)
    {
      var deleteResult = await _userService.DeleteAsync(id);
      if (deleteResult)
      {
        return Json(new { success = true, message = "Xóa tài khoản thành công." });
      }
      else
      {
        return Json(new { success = false, message = "Xóa tài khoản thất bại." });
      }
    }
  }
}
