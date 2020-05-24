using LogsAndStrategy.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Transactions;
using Xunit;
using static LogsAndStrategy.Models.AppDbContext;

namespace LogsAndStrategy.Tests
{
    public class BlogModelEfTests
    {
        [Fact]
        public void CanNullRefernceWithoutCascade()
        {
            var blog = new Blog { BlogName = "Name 1" };
            var post1 = new Post { PostName = "Post 1", Blog = blog };
            var post2 = new Post { PostName = "Post 2", Blog = blog };
            using(var ctx = new AppContextTest(true))
            {
                ctx.Blogs.Add(blog);
                ctx.Posts.AddRange(post1, post2);
                ctx.SaveChanges();
            }
            Assert.NotEqual(default, post1.PostId);
            Assert.Equal(blog.BlogId, post1.BlogId);

            Assert.NotEqual(default, post2.PostId);
            Assert.Equal(blog.BlogId, post2.BlogId);

            using(var ctx = new AppContextTest())
            {
                var savedBlog = ctx.Blogs.First();
                var firstPost = ctx.Posts.Where(p => p.BlogId == savedBlog.BlogId).First();
                ctx.Blogs.Remove(savedBlog);
                Assert.Throws<DbUpdateException>(()=>ctx.SaveChanges());
            }

            using (var ctx = new AppContextTest())
            {
                var savedBlog = ctx.Blogs.Include(b => b.Posts).First();
                ctx.Blogs.Remove(savedBlog);
                ctx.SaveChanges();
            }
        }

        [Fact]
        public void CanNullRefernceIfSetNull()
        {
            ActionBuilder setNullOption = modelBuilder =>
            {
                modelBuilder.Entity<Blog>().HasMany(b => b.Posts).WithOne(p => p.Blog).OnDelete(DeleteBehavior.SetNull);
            };

            var blog = new Blog { BlogName = "Name 1" };
            var post1 = new Post { PostName = "Post 1", Blog = blog };
            var post2 = new Post { PostName = "Post 2", Blog = blog };

            AppDbContext.EventActionBuilder += setNullOption;
            using (var ctx = new AppContextTest(true))
            {
                ctx.Blogs.Add(blog);
                ctx.Posts.AddRange(post1, post2);
                ctx.SaveChanges();
            }
            Assert.NotEqual(default, post1.PostId);
            Assert.Equal(blog.BlogId, post1.BlogId);

            Assert.NotEqual(default, post2.PostId);
            Assert.Equal(blog.BlogId, post2.BlogId);

            using (var ctx = new AppContextTest())
            {
                var savedBlog = ctx.Blogs.First();
                var firstPost = ctx.Posts.Where(p => p.PostId == post1.PostId).First();
                ctx.Blogs.Remove(savedBlog);
                ctx.SaveChanges();
            }

            Post remainingPost;
            using (var ctx = new AppContextTest())
            {
                remainingPost = ctx.Posts.Include(p => p.Blog).Where(p => p.PostId == post2.PostId).Single();
            }

            Assert.Null(remainingPost.BlogId);
            Assert.Null(remainingPost.Blog);
        }

        [Fact]
        public void CanHasProtectedField()
        {
            var blog = new Blog { BlogName = "Blog 1" };
            using(var ctx = new AppContextTest(true))
            {
                ctx.Blogs.Add(blog);
                ctx.SaveChanges();
            }

            Assert.Equal("Author", blog.GetAuthor());
        }

        [Fact]
        public void CanHasReadOnlyProperty()
        {
            var blog = new Blog { BlogName = "Blog 1" };
            using (var ctx = new AppContextTest(true))
            {
                ctx.Blogs.Add(blog);
                ctx.SaveChanges();
            }

            Assert.Equal("Title", blog.Title);
        }

        [Fact]
        public void CanHasPrivateSetProperty()
        {
            var blog = new Blog { BlogName = "Blog 1" };
            blog.SetYear(1900);
            using (var ctx = new AppContextTest(true))
            {
                ctx.Blogs.Add(blog);
                ctx.SaveChanges();
            }

            Assert.Equal(1900, blog.Year);
        }

        [Fact]
        public void CanHasReadOnlyField()
        {
            var blog = new Blog { BlogName = "Blog 1" };
            blog.SetYear(1900);
            using (var ctx = new AppContextTest(true))
            {
                ctx.Blogs.Add(blog);
                ctx.SaveChanges();
            }

            Assert.Equal(blog.BlogId, blog.GetId());
        }

        [Fact]
        public void ParamsContructHasPriority()//False
        {
            var blog = new Blog { BlogName = "Blog 1" };
            blog.SetYear(1900);
            using (var ctx = new AppContextTest(true))
            {
                ctx.Blogs.Add(blog);
                ctx.SaveChanges();
                ctx.Database.OpenConnection();
                ctx.Database.ExecuteSqlRaw($"update Blogs set Era='Current' where BlogId = {blog.BlogId}");
            }

            Blog savedBlog = null;
            using (var ctx = new AppContextTest())
            {
                savedBlog = ctx.Blogs.Where(b => b.BlogId == blog.BlogId).Single();
            }

            Assert.Equal("Current", savedBlog.Era);
            Assert.True(savedBlog.Token);
        }

        [Fact]
        public void CanNotInjectionServiceInConstruct()
        {
            var blog = new Blog { BlogName = "Blog 1" };
            blog.SetYear(1900);
            using (var ctx = new AppContextTest(true))
            {
                ctx.Blogs.Add(blog);
                ctx.SaveChanges();
            }

            Blog savedBlog = null;
            using (var ctx = new AppContextTest())
            {
                savedBlog = ctx.Blogs.Where(b => b.BlogId == blog.BlogId).Single();
            }

            Assert.Null(savedBlog.GetContext());
        }

        [Fact]
        public void CanReadKeylessEntity()
        {
            BlogExtension blogExtension = null;
            using (var ctx = new AppContextTest(true))
            {
                ctx.CreateViews();
                blogExtension = ctx.BlogExtensions.FirstOrDefault();
            }

            Assert.NotNull(blogExtension);
            Assert.NotEqual(default, blogExtension.CountBlogs);
        }

        [Fact]
        public void CalculateOnUpperLevel()
        {
            var blog = new Blog { BlogName = "Blog 1" };
            using(var ctx = new AppContextTest(true))
            {
                ctx.Blogs.Add(blog);
                ctx.SaveChanges();
            }

            Blog savedBlog = null;
            using (var ctx = new AppContextTest())
            {
                //Подготовленный запрос будет кешироваться вместе с ссылкой на экземпляры Blog
                //Деьали: https://docs.microsoft.com/ru-ru/ef/core/querying/client-eval#potential-memory-leak-in-client-evaluation
                savedBlog = ctx.Blogs.AsNoTracking().Where(b => b.BlogId == blog.BlogId).Select(b => b.InceremntCount()).Single();
            }
            Assert.Equal(1, savedBlog.Count);

            using (var ctx = new AppContextTest())
            {
                Assert.Throws<InvalidOperationException>(() =>
                savedBlog = ctx.Blogs.AsNoTracking().Where(b => b.BlogId == blog.BlogId && b.InceremntCount().Count > 0).Single()
                );
            }
        }

        [Fact]
        public void CalculateOnNonUpperLevel()
        {
            var blog = new Blog { BlogName = "Blog 1" };
            using (var ctx = new AppContextTest(true))
            {
                ctx.Blogs.Add(blog);
                ctx.SaveChanges();
            }

            Blog savedBlog = null;
            using (var ctx = new AppContextTest())
            {
                
                savedBlog = ctx.Blogs.AsNoTracking().AsEnumerable().Where(b => b.BlogId == blog.BlogId && b.InceremntCount().Count > 0).Single();
            }
            Assert.Equal(1, savedBlog.Count);
        }

        [Fact]
        public void CanCashQueryResultInChangeTracking()
        {
            var blog = new Blog { BlogName = "Blog 1" };
            using (var ctx = new AppContextTest(true))
            {
                ctx.Blogs.Add(blog);
                ctx.SaveChanges();
            }

            Blog savedBlog = null;
            using (var ctx = new AppContextTest())
            {
                savedBlog = ctx.Blogs.AsEnumerable().Where(b => b.BlogId == blog.BlogId).Single();
                ctx.Database.OpenConnection();
                ctx.Database.ExecuteSqlRaw(@$"update Blogs set BlogName='Blog 2' where BlogId={savedBlog.BlogId}");
                var savedBlogSeconQuery = ctx.Blogs.AsEnumerable().Where(b => b.BlogId == blog.BlogId).Single();
                Assert.Equal("Blog 1", savedBlog.BlogName);
                Assert.Equal("Blog 1", savedBlogSeconQuery.BlogName);
            }
        }

        [Fact]
        public void CanCancelCashQueryResultInChangeTracking()
        {
            var blog = new Blog { BlogName = "Blog 1" };
            using (var ctx = new AppContextTest(true))
            {
                ctx.Blogs.Add(blog);
                ctx.SaveChanges();
            }

            Blog savedBlog = null;
            using (var ctx = new AppContextTest())
            {
                //Отмена отслеживания и кеширования результатов
                ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking; //или DbSet<T>.AsNoTracking
                savedBlog = ctx.Blogs.AsEnumerable().Where(b => b.BlogId == blog.BlogId).Single();
                ctx.Database.OpenConnection();
                ctx.Database.ExecuteSqlRaw(@$"update Blogs set BlogName='Blog 2' where BlogId={savedBlog.BlogId}");
                var savedBlogSeconQuery = ctx.Blogs.AsEnumerable().Where(b => b.BlogId == blog.BlogId).Single();
                Assert.Equal("Blog 1", savedBlog.BlogName);
                Assert.Equal("Blog 2", savedBlogSeconQuery.BlogName);
            }
        }

        [Fact]
        public void CanAutoReferenceProperty()
        {
            var blog = new Blog {
                BlogName = "Blog 1" ,
                Posts=new List<Post> {
                    new Post { PostName = "Post 1" }, new Post { PostName = "Post 2" }
                }
            };
            using (var ctx = new AppContextTest(true))
            {
                ctx.Blogs.Add(blog);
                ctx.SaveChanges();
            }

            Blog savedBlog = null;
            using (var ctx = new AppContextTest())
            {
                var posts = ctx.Posts.ToList();
                Assert.Equal(2, posts.Count);
                savedBlog = ctx.Blogs.Where(b => b.BlogId == blog.BlogId).Single();
                Assert.Equal(2, savedBlog.Posts.Count);
            }
        }

        //[Fact]
        //public void CanIncludeFilter() //Ef Core 5
        //{
        //    var blog = new Blog
        //    {
        //        BlogName = "Blog 1",
        //        Posts = new List<Post> {
        //            new Post { PostName = "Post 1" }, new Post { PostName = "Post 2" }
        //        }
        //    };
        //    using (var ctx = new AppContextTest(true))
        //    {
        //        ctx.Blogs.Add(blog);
        //        ctx.SaveChanges();
        //    }

        //    Blog savedBlog = null;
        //    using (var ctx = new AppContextTest())
        //    {
        //        savedBlog = ctx.Blogs.Include(b => b.Posts.Where(p => p.PostName.Contains("1"))).Where(b => b.BlogId == blog.BlogId).Single();
        //        Assert.Single(savedBlog.Posts);
        //        Assert.Equal("Post 1", savedBlog.Posts[0].PostName);
        //    }
        //}

        [Fact]
        public void CanModifiedOneEntity()
        {
            var blog = new Blog { BlogName = "Blog 1" };
            var posts = new List<Post> { new Post { PostName = "Post 1" } };
            blog.Posts = posts;
            using(var ctx = new AppContextTest(true))
            {
                ctx.Entry(blog).State = EntityState.Added;
                ctx.SaveChanges();
            }

            Blog savedBlog = null;
            using(var ctx = new AppContextTest())
            {
                savedBlog = ctx.Blogs.Include(b => b.Posts).Where(b => b.BlogId == blog.BlogId).Single();
            }
            Assert.Empty(savedBlog.Posts);
        }

        [Fact]
        public void CanAddRelatedEntityInCollectionWithoutRedefinitionValueComparerOnTrackingEntity()
        {
            var blog = new Blog { BlogName = "Blog 1", Bytes = new byte[2] { 1 , 1} };
            var posts = new List<Post> { new Post { PostName = "Post 1" } };
            blog.Posts = posts;
            using (var ctx = new AppContextTest(true))
            {
                ctx.Blogs.Add(blog);
                ctx.SaveChanges();
            }

            Blog savedBlog = null;
            using (var ctx = new AppContextTest())
            {
                savedBlog = ctx.Blogs.Include(b => b.Posts).Where(b => b.BlogId == blog.BlogId).Single();
                savedBlog.Posts.Add(new Post { PostName = "Post 2" });
                savedBlog.Bytes[0] = 2;
                ctx.SaveChanges();
            }
            Assert.Equal(2, savedBlog.Posts.Count);

            using (var ctx = new AppContextTest())
            {
                savedBlog = ctx.Blogs.Where(b => b.BlogId == blog.BlogId).Single();
                ctx.SaveChanges();
            }
            Assert.NotEqual(2, savedBlog.Bytes[0]);
        }

        [Fact]
        public void CanChangeRelatedEntity()
        {
            var blog = new Blog { BlogName = "Blog 1", Bytes = new byte[2] { 1, 1 } };
            Blog newBlog = new Blog { BlogName = "Blog 2" };
            var posts = new List<Post> { new Post { PostName = "Post 1" } };
            blog.Posts = posts;
            using (var ctx = new AppContextTest(true))
            {
                ctx.Blogs.AddRange(blog, newBlog);
                ctx.SaveChanges();
            }

            posts.Add(new Post { PostName = "Post 2" });
            using (var ctx = new AppContextTest())
            {
                ctx.Attach(newBlog);
                newBlog.Posts = posts;
                //каждый элемент newBlog.Posts провреяеться на наличие ключа сгенеррованного БД
                //Если ключ существует - update, иначе - add
                ctx.SaveChanges();
            }

            Assert.Equal(2, newBlog.Posts.Count);
        }

        [Fact]
        public void CanWorkInTransactionScopeForManyStore()
        {
            Blog blog = new Blog { BlogName = "Blog 1" };
            #region Sqlite
            if (!File.Exists(@"C:\TestDB.db"))
            {
                SQLiteConnection.CreateFile(@"C:\TestDB.db");
            }

            using (SQLiteConnection Connect = new SQLiteConnection(@"Data Source=C:\TestDB.db; Version=3;")) // в строке указывается к какой базе подключаемся
            {
                // строка запроса, который надо будет выполнить
                string commandText = "Drop TABLE IF EXISTS [dbTableName]";
                SQLiteCommand Command = new SQLiteCommand(commandText, Connect);
                Connect.Open(); // открыть соединение
                Command.ExecuteNonQuery(); // выполнить запрос
                Connect.Close();
            }

            using (SQLiteConnection Connect = new SQLiteConnection(@"Data Source=C:\TestDB.db; Version=3;")) // в строке указывается к какой базе подключаемся
            {
                // строка запроса, который надо будет выполнить
                string commandText = "CREATE TABLE IF NOT EXISTS [dbTableName] ( [id] INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL, [str] VARCHAR(10) )";
                SQLiteCommand Command = new SQLiteCommand(commandText, Connect);
                Connect.Open(); // открыть соединение
                Command.ExecuteNonQuery(); // выполнить запрос
                Connect.Close();
            }

            #endregion
            try
            {
                using (var transaction = new TransactionScope())
                {

                    using (var ctx = new AppContextTest(true))
                    {
                        ctx.Blogs.Add(blog);
                        ctx.SaveChanges();
                    }

                    #region SqLite
                    
                    using (SQLiteConnection Connect = new SQLiteConnection(@"Data Source=C:\TestDB.db; Version=3;")) // в строке указывается к какой базе подключаемся
                    {
                        string commandText = "insert into [dbTableName] ([str]) values ('test')";
                        SQLiteCommand Command = new SQLiteCommand(commandText, Connect);
                        Connect.Open(); // открыть соединение
                        Command.ExecuteNonQuery();
                        Connect.Close(); // закрыть соединение
                    }

                    #endregion

                    throw new InvalidCastException();
                    transaction.Complete();
                }
            }
            catch (InvalidCastException) { }

            #region SqLite

            using (SQLiteConnection Connect = new SQLiteConnection(@"Data Source=C:\TestDB.db; Version=3;")) // в строке указывается к какой базе подключаемся
            {
                Connect.Open();
                SQLiteCommand Command = new SQLiteCommand
                {
                    Connection = Connect,
                    CommandText = @"SELECT str FROM [dbTableName]"
                };
                SQLiteDataReader sqlReader = Command.ExecuteReader();
                string str = "";
                while (sqlReader.Read())
                {
                    str += sqlReader["str"].ToString().TrimStart().TrimEnd();
                }
                Connect.Close();
                Assert.Empty(str);
            }
            #endregion

            using (var ctx = new AppContextTest(true))
            {
                var savedBlog = ctx.Blogs.Where(b => b.BlogId == blog.BlogId).SingleOrDefault();
                Assert.Null(savedBlog);
            }
        }
    }
}
