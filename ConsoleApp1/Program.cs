using LogsAndStrategy.Models;
using System;
using System.Linq;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            CanEqualConversionType();
        }

        public static void CanEqualConversionType()
        {
            var periods = new ComparsionList<Period>
            {
                new Period{Day = 1},
                new Period{Day = 2},
                new Period{Day = 3}
            };

            var shedule = new Shedule { Periods = periods };
            using (var ctx = new AppDbContext())
            {
                ctx.Shedules.Add(shedule);
                ctx.SaveChanges();
            }


            using (var ctx = new AppDbContext())
            {
                var savedShedule = ctx.Shedules.Where(s => s.Id == shedule.Id).Single();
                savedShedule.Periods.Add(new Period { Day = 4 });
                //Console.WriteLine(ctx.Entry(savedShedule.Periods).State);
                ctx.SaveChanges();
            }

            using (var ctx = new AppDbContext())
            {
                var savedShedule = ctx.Shedules.Where(s => s.Id == shedule.Id).Single();
                var addedPeriod = savedShedule.Periods.Where(p => p.Day == 4).FirstOrDefault();
                Console.WriteLine(addedPeriod);
            }
        }
    }
}
