using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookSale.Management.Application.DTOs;

namespace BookSale.Management.UI.Models
{
    public class BookForSiteModel
    {
        public int TotalRecords { get; set; }
        public int CurrentDisplayItems { get; set; }
        public bool IsDisableButton { get; set; }

        public IEnumerable<BookDTO> Books { get; set; }
    }
}