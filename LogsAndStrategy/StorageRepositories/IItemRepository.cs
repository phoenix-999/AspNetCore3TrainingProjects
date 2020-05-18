using LogsAndStrategy.Models;
using Microsoft.AspNetCore.Http.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogsAndStrategy.StorageRepositories
{
    public interface IItemRepository
    {
        Task<IList<Item>> GetAll();
        Item GetItem(string name);
        Task<Item> AddItem(Item item);
        Task<Item> AddItem(string name);
        Task<int> AddItems(params Item[] items);
        Task<Tag> AddTag(string label, string itemName);
        Task<Item> RemoveItem(string name);
    }
}
