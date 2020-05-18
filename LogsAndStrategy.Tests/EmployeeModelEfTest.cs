using LogsAndStrategy.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using NLog.LayoutRenderers.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using Xunit;

namespace LogsAndStrategy.Tests
{
    public class EmployeeModelEfTest
    {
        [Fact]
        public void CanNotAddEnityWithCustomKeyInIdentity()
        {
            Employee emp = new Employee() { EmployeeId = 10, EmployeeName = "First" };
            using (var ctx = new AppContextTest(true))
            {
                ctx.Employees.Add(emp);
                Assert.Throws<DbUpdateException>(() => ctx.SaveChanges());
            }
        }

        [Fact]
        public void CanAddEnityWithCustomKeyInIdentityWithIDENTITY_INSERT()
        {
            Employee emp = new Employee() { EmployeeId = 10, EmployeeName = "First" };
            using (var ctx = new AppContextTest(true))
            {
                ctx.Employees.Add(emp);

                ctx.Database.OpenConnection();
                ctx.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Employees ON");
                ctx.SaveChanges();
                ctx.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Employees OFF");
            }

            Employee emp2;
            using (var ctx = new AppContextTest())
            {
                emp2 = ctx.Employees.FirstOrDefault(e => e.EmployeeId == 10);
            }

            Assert.NotNull(emp2);
            Assert.Equal(10, emp2.EmployeeId);
            Assert.Equal("First", emp2.EmployeeName);
        }

        [Fact]
        public void CanNotUpdateWhenAddedEntityWithSameKey()
        {
            var emp1 = new Employee() { EmployeeName = "First" };
            using(var ctx = new AppContextTest(true))
            {
                ctx.Employees.Add(emp1);
                ctx.SaveChanges();
            }

            using (var ctx = new AppContextTest())
            {
                ctx.Employees.Add(new Employee { EmployeeId = emp1.EmployeeId});
                ctx.Database.OpenConnection();
                ctx.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Employees ON");
                //ctx.SaveChanges();
                Assert.Throws<DbUpdateException>(()=>ctx.SaveChanges());
                ctx.Database.ExecuteSqlRaw("SET IDENTITY_INSERT dbo.Employees OFF");
            }

        }

        [Fact]
        public void CanNotChangeAlternateKeyAfterCreate()
        {
            var emp = new Employee { EmployeeName = "First", PassportId = "First_PassportId" };
            using(var ctx = new AppContextTest(true))
            {
                ctx.Employees.Add(emp);
                ctx.SaveChanges();
            }
            Assert.NotEqual(default, emp.EmployeeId);

            using(var ctx = new AppContextTest())
            {
                var emp2 = ctx.Employees.Where(e => e.EmployeeId == emp.EmployeeId).Single();
                emp2.PassportId = "Fake";
                Assert.Throws<InvalidOperationException>(() => ctx.SaveChanges());
            }
        }

        [Fact]
        public void GeneratedGuidAutomaticalyIfAlternateKeyAndSetValueGeneratedOnAdd()
        {
            var emp = new Employee();
            using(var ctx = new AppContextTest(true))
            {
                ctx.Employees.Add(emp);
                ctx.SaveChanges();
            }

            Assert.NotEqual(default, emp.EmployeeId);
            Assert.NotEqual(default, emp.Guid);
            using(var ctx = new AppContextTest())
            {
                Assert.False(ctx.Entry(emp).Metadata.GetProperty("Guid").IsNullable);
            }
        }

        [Fact]
        public void ClientHasHighestPriorityGenerateValueWithGuidAutomaticalyGenerateOnAdd()
        {
            var guid = Guid.NewGuid();
            var emp = new Employee() { Guid = guid };
            using (var ctx = new AppContextTest(true))
            {
                ctx.Employees.Add(emp);
                ctx.SaveChanges();
            }

            Assert.NotEqual(default, emp.EmployeeId);
            Assert.Equal(guid, emp.Guid);
        }

        [Fact]
        public void PropertyHasNullIfSetValueGeneratedOnAddOrUpdateAndNotSetValue()
        {
            var emp = new Employee();
            using (var ctx = new AppContextTest(true))
            {
                ctx.Employees.Add(emp);
                ctx.SaveChanges();
            }
            
            Assert.Null(emp.LastPayRaise);
        }

        [Fact]
        public void NullIfSetValueGeneratedOnAddOrUpdateAndSetValue()
        {
            var emp = new Employee();
            var startDt = DateTime.Now;
            emp.LastPayRaise = startDt;
            using (var ctx = new AppContextTest(true))
            {
                ctx.Employees.Add(emp);
                ctx.SaveChanges();
            }

            //Assert.Equal(startDt, emp.LastPayRaise);//Если включить ValueGeneratedOnUpdate()
            Assert.Null(emp.LastPayRaise);

            var newDt = DateTime.Now;
            emp.LastPayRaise = newDt;
            using (var ctx = new AppContextTest())
            {
                ctx.Employees.Update(emp);
                ctx.SaveChanges();
            }
            //Assert.Equal(startDt, emp.LastPayRaise);//Если включить ValueGeneratedOnUpdate()
            Assert.Null(emp.LastPayRaise);

        }

        [Fact]
        public void PropertyHasDefaultValue()
        {
            var emp = new Employee();
            using(var ctx = new AppContextTest(true))
            {
                ctx.Employees.Add(emp);
                ctx.SaveChanges();
            }

            Assert.Equal("No description", emp.Description);
        }

        [Fact]
        public void PropertyHasNotDefaultValueOnUpdateValueToNull()
        {
            var emp = new Employee();
            using (var ctx = new AppContextTest(true))
            {
                ctx.Employees.Add(emp);
                ctx.SaveChanges();
            }
            Assert.Equal("No description", emp.Description);

            emp.Description = null;
            using (var ctx = new AppContextTest())
            {
                ctx.Employees.Update(emp);
                ctx.SaveChanges();
            }
            Assert.Null(emp.Description);
        }

        [Fact]
        public void PropertyHasComputedValue()
        {
            var emp = new Employee { EmployeeName = "Name", EmployeeLastName = "LastName" };
            using (var ctx = new AppContextTest(true))
            {
                ctx.Employees.Add(emp);
                ctx.SaveChanges();
            }

            Assert.Equal("Name, LastName", emp.EmployeeFullName);
        }

        [Fact]
        public void CanHandleDbConcurencyException()
        {
            var emp = new Employee() { RowVersion="First"};
            using(var ctx = new AppContextTest(true))
            {
                ctx.Employees.Add(emp);
                ctx.SaveChanges();
            }

            Assert.NotEqual(default, emp.EmployeeId);

            using(var ctx = new AppContextTest())
            {
                var receivedEmp = ctx.Employees.Single(e => e.EmployeeId == emp.EmployeeId);

                ctx.Database.OpenConnection();
                ctx.Database.ExecuteSqlRaw("update Employees set RowVersion='Second_1'  where EmployeeId={0}", emp.EmployeeId);

                receivedEmp.RowVersion = "Second_2";

                bool saved = false;
                while (!saved)
                {
                    try
                    {
                        //var ex = Assert.Throws<DbUpdateConcurrencyException>(() => ctx.SaveChanges());
                        //if(ex != null)
                        //    throw ex;
                        ctx.SaveChanges();
                        saved = true;
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        foreach(var entry in ex.Entries)
                        {
                            var proposedValues = entry.CurrentValues;
                            var dbValues = entry.GetDatabaseValues();

                            foreach(var property in proposedValues.Properties)
                            {
                                var proposedValue = proposedValues[property];
                                var dbValue = dbValues[property];
                                if(property.Name == "RowVersion")
                                    Assert.Equal("Second_1", dbValue);
                                //Logic handle
                                //proposedValues[property] = dbValue;
                            }
                            entry.OriginalValues.SetValues(dbValues);                            
                        }
                    }
                }

                Assert.True(saved);
            }

            using (var ctx = new AppContextTest())
            {
                var receivedEmp = ctx.Employees.Single(e => e.EmployeeId == emp.EmployeeId);
                Assert.Equal("Second_2", receivedEmp.RowVersion);
            }
        }
    }
}
