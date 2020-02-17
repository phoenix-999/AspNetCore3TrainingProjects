using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SortPaginationFilters.Models;
using SortPaginationFilters.ViewModels.Home;
using SortPaginationFilters.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SortPaginationFilters.Controllers
{
    public class HomeController : Controller
    {
        UsersContext db;

        public HomeController(UsersContext context)
        {
            db = context;
        }

        public IActionResult Index(FiltersViewModel filters, SortOrder sortOrder = SortOrder.UserNameAsc)
        {
            IQueryable<User> users = db.Users.Include(u => u.Company);

            if(!string.IsNullOrEmpty(filters.UserName))
            {
                users = users.Where(u => u.Name.Contains(filters.UserName));
            }

            int[] selectedCompanies = filters.SelectedCompanies;
            if (selectedCompanies != null && selectedCompanies.Count() > 0 && !selectedCompanies.Contains(0))
            {
                users = users.Where(u => selectedCompanies.Contains(u.Company.Id));
            }

            SortUserViewModel sortUserViewModel = new SortUserViewModel(users, sortOrder);
            var sortedUsers = sortUserViewModel.Sort().ToList();
            filters.Companies = new SelectList(db.Companies.ToList(), "Id", "Name");

            var viewModel = new HomeIndexViewModel();
            viewModel.SortUserViewModel = sortUserViewModel;
            viewModel.Filters = filters;
            viewModel.Users = sortedUsers;

            return View(viewModel);
        }
    }
}