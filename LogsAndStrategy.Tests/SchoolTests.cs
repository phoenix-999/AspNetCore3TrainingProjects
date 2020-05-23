using LogsAndStrategy.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace LogsAndStrategy.Tests
{
    public class SchoolTests
    {
        [Fact]
        public void CanLoadDerivedType()
        {
            var student = new Student { Name = "Student 1" };
            var school = new School { Name = "School 1", Students = new List<Student> { student } };
            var person = new Person { Name = "Person 1" };
            using (var ctx = new AppContextTest(true))
            {
                ctx.Schools.Add(school);
                ctx.People.Add(person);
                ctx.SaveChanges();
            }

            School savedSchool = null;
            Student savedStudent = null;
            using (var ctx = new AppContextTest())
            {
                savedSchool = ctx.Schools.Include(s => s.Students).Where(s => s.Id == school.Id).Single();
                savedStudent = ctx.People.Include("School").Where(s => s.Id == student.Id).Single() as Student;
            }
            Assert.NotNull(savedSchool);
            Assert.Single(savedSchool.Students);
            Assert.Equal(savedSchool, savedStudent.School);

            Assert.Equal(student.Id, savedStudent.Id);
        }

        [Fact]
        public void CanOperationWithoutLoad()
        {
            var student1 = new Student { Name = "Student 1" };
            var student2 = new Student { Name = "Student 2" };
            var student31 = new Student { Name = "Student 31" };
            var school = new School { Name = "School 1", Students = new List<Student> { student1, student2, student31 } };
            var person = new Person { Name = "Person 1" };
            using (var ctx = new AppContextTest(true))
            {
                ctx.Schools.Add(school);
                ctx.People.Add(person);
                ctx.SaveChanges();
            }

            School savedSchool = null;
            using (var ctx = new AppContextTest())
            {
                savedSchool = ctx.Schools.Where(s => s.Id == school.Id).Single();
                savedSchool.Students = null;
                var countStudents = ctx.Entry(savedSchool).Collection(s => s.Students).Query().Where(s => s.Name.Contains("1")).Count();
                Assert.Equal(2, countStudents);
                Assert.Empty(savedSchool.Students);//При операциях явной загрузки без загрузки данных(например Count), коллекция будет создана без элемнтов
            }

            using (var ctx = new AppContextTest())
            {
                savedSchool = ctx.Schools.Where(s => s.Id == school.Id).Single();
                savedSchool.Students = null;
                var countStudents = ctx.People.Count();
                Assert.NotEqual(default, countStudents);
                Assert.Null(savedSchool.Students);//Коллекция будет равна null если её не создать вручную(например в конструкторе типа)
            }

        }

        [Fact]
        public void CanLazyLoad()
        {
            var student1 = new Student { Name = "Student 1" };
            var student2 = new Student { Name = "Student 2" };
            var student31 = new Student { Name = "Student 31" };
            var school = new School { Name = "School 1", Students = new List<Student> { student1, student2, student31 } };
            var person = new Person { Name = "Person 1" };
            using (var ctx = new AppContextTest(true))
            {
                ctx.Schools.Add(school);
                ctx.People.Add(person);
                ctx.SaveChanges();
            }

            School savedSchool = null;
            using (var ctx = new AppContextTest())
            {
                savedSchool = ctx.Schools.Where(s => s.Id == school.Id).Single();
                var countStudents = savedSchool.Students.ToList().Count();
                Assert.Equal(3, countStudents);
            } 
        }

        //[Fact]
        //public void CanLazyLoadWithILazyLoadConstructParam()//True if .UseLazyLoadingProxies(useLazyLoadingProxies: false); - По умолчанию true
        //{
        //    var blog = new Blog { BlogName = "Blog 1" };
        //    var school = new School { Name = "School 1", Blog = blog };
            
        //    using (var ctx = new AppContextTest(true))
        //    {
        //        ctx.Schools.Add(school);
        //        ctx.Blogs.Add(blog);
        //        ctx.SaveChanges();
        //    }

        //    School savedSchool = null;
        //    using (var ctx = new AppContextTest())
        //    {
        //        savedSchool = ctx.Schools.Where(s => s.Id == school.Id).Single();
        //        Assert.NotNull(savedSchool.Blog);
        //        Assert.Equal("Blog 1", savedSchool.Blog.BlogName);
        //    }
        //}

        //[Fact]
        //public void CanLazyLoadWithoutProxyType()//True if .UseLazyLoadingProxies(useLazyLoadingProxies: false) - По умолчанию true
        //{
        //    var student1 = new Student { Name = "Student 1" };
        //    var student2 = new Student { Name = "Student 2" };
        //    var student31 = new Student { Name = "Student 31" };
        //    var school = new School { Name = "School 1", Students = new List<Student> { student1, student2, student31 } };
        //    var person = new Person { Name = "Person 1" };
        //    using (var ctx = new AppContextTest(true))
        //    {
        //        ctx.Schools.Add(school);
        //        ctx.People.Add(person);
        //        ctx.SaveChanges();
        //    }

        //    School savedSchool = null;
        //    using (var ctx = new AppContextTest())
        //    {
        //        savedSchool = ctx.Schools.Where(s => s.Id == school.Id).Single();
        //        var student = savedSchool.Students.ToList().FirstOrDefault();
        //        Assert.NotNull(student);
        //        Assert.IsAssignableFrom<Student>(student);
        //        Assert.IsType<Student>(student);
        //    }
        //}
    }
}
