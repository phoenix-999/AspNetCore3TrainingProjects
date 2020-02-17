using Microsoft.AspNetCore.Mvc.Rendering;
using SortPaginationFilters.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SortPaginationFilters.ViewModels.Home
{
    public class FiltersViewModel
    {
        public SelectList Companies { get; set; }
        public string UserName { get; set; }
        public int[] SelectedCompanies { get; set; }
        public FiltersViewModel() {}

        public FiltersViewModel(List<Company> companies)
        {
            Companies = new SelectList(companies, "Id", "Name");
        }
    }
}
