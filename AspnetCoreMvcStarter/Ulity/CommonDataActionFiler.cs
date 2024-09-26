using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using BookSale.Management.Domain.Setting;
using BookSale.Management.UI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace BookSale.Management.UI.Ulity
{
    public class CommonDataActionFiler : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var carts = context.HttpContext.Session.Get<List<CartModel>>(CommonConstant.SessionCart);
            
            if(carts is not null)
            {
                var controller = context.Controller as Controller;
                controller.ViewData["NumberCart"] = carts.Count;
            }
        }
    
        
    }
    public class SiteAreaConvention : IControllerModelConvention {
        public void Apply(ControllerModel controller) {
            var areaAttribute = controller.Attributes.OfType<AreaAttribute>().FirstOrDefault();
            if(string.IsNullOrEmpty(areaAttribute?.RouteValue)){
                controller.Filters.Add(new CommonDataActionFiler());
            }
        }
    }
}