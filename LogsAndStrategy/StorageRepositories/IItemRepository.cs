using LogsAndStrategy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogsAndStrategy.StorageRepositories
{
    public interface IItemRepository
    {
        Task<IList<Item>> GetAll();
        Task<Item> Add(Item item);
    }
}
