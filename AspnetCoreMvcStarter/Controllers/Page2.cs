using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace BookSale.Management.UI.Controllers;

public class Page2 : Controller
{
  public IActionResult Index() => View();
}
