using LogsAndStrategy.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
