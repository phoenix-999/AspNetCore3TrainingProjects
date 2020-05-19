using LogsAndStrategy.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace LogsAndStrategy.Tests
{
    public class SheduleTests
    {
        [Fact]
        public void CanConversion()
        {
            var periods = new ComparsionList<Period>
            {
                new Period{Day = 1},
                new Period{Day = 2},
                new Period{Day = 3}
            };

            var shedule = new Shedule { Periods = periods };
            using (var ctx = new AppContextTest(true))
            {
                ctx.Shedules.Add(shedule);
                ctx.SaveChanges();
            }

            Assert.NotEqual(default, shedule.Id);
            Assert.NotEmpty(shedule.Periods);

            using (var ctx = new AppContextTest())
            {
                var savedShedule = ctx.Shedules.Where(s => s.Id == shedule.Id).Single();
                Assert.Equal(periods.Count, savedShedule.Periods.Count);
            }
        }

        [Fact]
        public void CanEqualConversionType()
        {
            var periods = new ComparsionList<Period>
            {
                new Period{Day = 1},
                new Period{Day = 2},
                new Period{Day = 3}
            };

            var shedule = new Shedule { Periods = periods, ByteProperty = new byte[2] { 1, 2} };
            using (var ctx = new AppContextTest(true))
            {
                ctx.Shedules.Add(shedule);
                ctx.SaveChanges();
            }

            Assert.NotEqual(default, shedule.Id);
            Assert.NotEmpty(shedule.Periods);

            using (var ctx = new AppContextTest())
            {
                var savedShedule = ctx.Shedules.Where(s => s.Id == shedule.Id).Single();
                savedShedule.Periods.Add(new Period { Day = 4 });
                savedShedule.ByteProperty[1] = 3;
                ctx.SaveChanges();
            }

            using (var ctx = new AppContextTest())
            {
                var savedShedule = ctx.Shedules.Where(s => s.Id == shedule.Id).Single();
                var addedPeriod = savedShedule.Periods.Where(p => p.Day == 4).FirstOrDefault();
                Assert.NotNull(addedPeriod);
                Assert.NotEqual(3, savedShedule.ByteProperty[1]);
            }
        }
    }
}
