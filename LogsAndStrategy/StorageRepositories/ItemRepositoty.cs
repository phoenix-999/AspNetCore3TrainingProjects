using LogsAndStrategy.Models;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NLog.Fluent;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

namespace LogsAndStrategy.StorageRepositories
{
    public class ItemRepositoty : IItemRepository
    {
        private readonly AppDbContext _ctx;
        private readonly ILogger<ItemRepositoty> _logger;
        public ItemRepositoty(AppDbContext context, ILogger<ItemRepositoty> logger)
        {
            _ctx = context;
            _logger = logger;
        }

        public async virtual Task<IList<Item>> GetAll()
        {
            return await _ctx.Items.Include(i => i.Tags).ToListAsync();
        }

        public virtual Item GetItem(string name)
        {
            return _ctx.Items.Include(i => i.Tags).FirstOrDefault(i => i.Name == name);
        }

        public async virtual Task<Item> AddItem(Item item)
        {

            //using (var transaction = new TransactionScope()) //Ошибка при выбранной стратегии устойчевого выполнения 
            //{
            //    await _ctx.Items.AddAsync(item);
            //    await _ctx.SaveChangesAsync();
            //    transaction.Complete();
            //}


            var strategy = _ctx.Database.CreateExecutionStrategy(); //Можно создать без opt => opt.EnableRetryOnFailure() в конфигурцаии UseSqlServer
            //Решение 1
            //Но не учтена ошибка при фиксации транзакции (напр. прерывание соединения, а данные уже сохранены в бд)
            //await strategy.Execute(async () => 
            //{
            //    using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled)) 
            //    {
            //        await _ctx.Items.AddAsync(item);
            //        await _ctx.SaveChangesAsync();
            //        //Здесь поток уже может быть другим
            //        transaction.Complete();
            //    }
            //});


            //Решение 2 без явного вызова транзакции и с проверкой достоврености транзакции
            //Этот вызов можно включить в переопределение метода SaveChanges контекста
            AppTransaction transaction = new AppTransaction();
            await _ctx.Items.AddAsync(item);

            await _ctx.Transactions.AddAsync(transaction);
            strategy.ExecuteInTransaction(_ctx,
                operation: (context) =>
                {
                    context.SaveChanges(acceptAllChangesOnSuccess: false);//ChangeTracking не участвует в откате транзакции

                },
                verifySucceeded: context => //Вызывается в случае определенной ошибки в первом делегате. Может быть не вызван
                {
                    //Если false - можно описать логику возвращения состояния контекста до очередного запуска транзакции
                    return context.Transactions.AsNoTracking().Any(t => t.Id == transaction.Id);
                });
            //Если выполнения дойдет сюда - значит все сохранено успешно
            _ctx.ChangeTracker.AcceptAllChanges();
            _ctx.Transactions.Remove(transaction);
            _ctx.SaveChanges();

            return item;
        }
    
        public async virtual Task<Item> AddItem(string name)
        {
            var item = _ctx.Add(new Item(name)).Entity;

            var strategy = _ctx.Database.CreateExecutionStrategy();
            var transaction =_ctx.Transactions.Add(new AppTransaction()).Entity;

            await strategy.ExecuteInTransactionAsync<AppDbContext>(_ctx,
                operation: async (context, token) =>
                {
                    await context.SaveChangesAsync(acceptAllChangesOnSuccess: false);
                    //ChangeTracking не участвует в откате транзакции
                },
                verifySucceeded: async (context, token) =>
                {
                    return await context.Transactions.AsNoTracking().AnyAsync(t => t.Id == transaction.Id);
                });
            _ctx.ChangeTracker.AcceptAllChanges();
            _ctx.Transactions.Remove(transaction);
            _ctx.SaveChanges();

            return item;
        }

        public async virtual Task<int> AddItems(params Item[] items)
        {
            _ctx.Items.AddRange(items);
            var transaction = new AppTransaction();
            _ctx.Transactions.Add(transaction);
            int result = 0;
            var strategy = _ctx.Database.CreateExecutionStrategy();
            try
            {
                Debug.WriteLine(_ctx.Entry(transaction).State);//Added
                strategy.ExecuteInTransaction(
                    operation: () =>
                    {
                        result = _ctx.SaveChanges();
                        throw new ConnectionAbortedException();
                    },
                    verifySucceeded: () => _ctx.Transactions.AsNoTracking().Any(t => t.Id == transaction.Id)
                );
            }catch(Exception ex)
            {
                Debug.WriteLine(_ctx.Entry(transaction).State);//Unchanged
            }
            return result;
        }

        public async virtual Task<Tag> AddTag(string label, string itemName)
        {
            var tag = _ctx.Items.Include(i => i.Tags).Single(i => i.Name == itemName).AddTag(label);
            await _ctx.SaveChangesAsync();
            return tag;
        }
    
        public virtual Task<Item> RemoveItem(string name)
        {
            var item = _ctx.Items.FirstOrDefault(i => i.Name == name);
            if(item != null)
            {
                _ctx.Items.Remove(item);
                _ctx.SaveChanges();
            }
            return Task.FromResult(item);
        }
    }
}
