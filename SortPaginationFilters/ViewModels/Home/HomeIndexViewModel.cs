using SortPaginationFilters.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SortPaginationFilters.ViewModels.Home
{
    public class HomeIndexViewModel
    {
        public IEnumerable<User> Users { get; set; }
        public SortUserViewModel SortUserViewModel { get; set; }
        public FiltersViewModel Filters { get; set; }
    }
}
