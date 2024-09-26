using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookSale.Management.UI.Models
{
    public class CartModel
    {
        public string BookCode { get; set; }
        public int Quantity { get; set; }
    }
}