using LogsAndStrategy.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NLog.Fluent;
using System;
using System.Collections.Generic;
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
            return await _ctx.Items.ToListAsync();
        }

        public async virtual Task<Item> Add(Item item)
        {

            //using (var transaction = new TransactionScope()) //Ошибка при выбранной стратегии устойчевого выполнения 
            //{
            //    await _ctx.Items.AddAsync(item);
            //    await _ctx.SaveChangesAsync();
            //    transaction.Complete();
            //}


            //Решение 1
            //Но не учтена ошибка при фиксации транзакции (напр. прерывание соединения, а данные уже сохранены в бд)
            var strategy = _ctx.Database.CreateExecutionStrategy();
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
                    context.SaveChanges(acceptAllChangesOnSuccess: false); //Асинхронная операция выбросить ошибку
                },
                verifySucceeded: context =>
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
    }
}
