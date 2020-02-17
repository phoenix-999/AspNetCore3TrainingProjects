using SortPaginationFilters.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SortPaginationFilters.Infrastructure;

namespace SortPaginationFilters.ViewModels.Home
{
    public class SortUserViewModel
    {
        public SortOrder UserNameOrder { get; private set; }
        public SortOrder CompanyNameOrder { get; private set; }
        public SortOrder UserAgeOrder { get; set; }
        public SortOrder Current { get; }

        public IEnumerable<User> Items { get; private set; }

        public SortUserViewModel(IEnumerable<User> items, SortOrder sortOrder)
        {
            Current = sortOrder;
            Items = items;
            SetSortOrders();
        }

        private void SetSortOrders()
        {
            UserNameOrder = Current == SortOrder.UserNameAsc ? SortOrder.UserNameDesc : SortOrder.UserNameAsc;
            UserAgeOrder = Current == SortOrder.UserAgeAsc ? SortOrder.UserAgeDesc : SortOrder.UserAgeAsc;
            CompanyNameOrder = Current == SortOrder.CompanyNameAsc ? SortOrder.CompanyNameDesc : SortOrder.CompanyNameAsc;
        }
        public IEnumerable<User> Sort()
        {
            Items = Current switch
            {
                SortOrder.UserAgeAsc => Items.OrderBy(i => i.Age),
                SortOrder.UserAgeDesc => Items.OrderByDescending(i => i.Age),
                SortOrder.CompanyNameAsc => Items.OrderBy(i => i.Company.Name),
                SortOrder.CompanyNameDesc => Items.OrderByDescending(i => i.Company.Name),
                SortOrder.UserNameDesc => Items.OrderByDescending(i => i.Name),
                _ => Items.OrderBy(i => i.Name)
            };
            return Items;
        }
    }
}
