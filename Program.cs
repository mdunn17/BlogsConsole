using System;
using NLog.Web;
using System.IO;
using System.Linq;

namespace BlogsConsole
{
    class Program
    {
        // create static instance of Logger
        private static NLog.Logger logger = NLogBuilder.ConfigureNLog(Directory.GetCurrentDirectory() + "\\nlog.config").GetCurrentClassLogger();
        static void Main(string[] args)
        {
            logger.Info("Program started");

             try
            {
                string choice = "";
                do{
                    Console.WriteLine("  -MENU-  \n1. Display All Blogs\n2. Add New Blog\n3. Create Post\n4. Display Posts\nEnter 'done' to quit");
                    choice = Console.ReadLine();
                    logger.Info("User choice: " + choice);
                    if (choice == "1")
                    {
                        var db = new BloggingContext();
                        // Display all Blogs from the database
                        var query = db.Blogs.OrderBy(b => b.BlogId);

                        Console.WriteLine("All blogs in the database:");
                        var count = db.Blogs.Count();
                        Console.WriteLine(count + " blogs found.");
                        foreach (var item in query)
                        {
                            Console.WriteLine(item.BlogId +") " + item.Name);
                        }
                    } else if (choice == "2")
                    {
                        // Create and save a new Blog
                        Console.Write("Enter a name for a new Blog: ");
                        string input = Console.ReadLine();
                        if (string.IsNullOrEmpty(input))
                        {
                            logger.Error("Blog name cannot be null");
                        }else
                        {
                            var name = input;
                            var blog = new Blog { Name = name };

                            var db = new BloggingContext();
                            db.AddBlog(blog);
                            logger.Info("Blog added - {name}", name);
                        }
                    } else if (choice == "3")
                    {
                        var db = new BloggingContext();
                        Console.WriteLine("Please enter the blog you wish to post to: ");
                        var query = db.Blogs.OrderBy(b => b.BlogId);
                        foreach (var item in query)
                        {
                            Console.WriteLine(item.BlogId + ") " + item.Name);
                        }
                        
                        string selectBlog = Console.ReadLine();
                        var test = db.Blogs.Select(b => b.BlogId);
                        string test2 = Convert.ToString(test);

                        if(test2.Contains(selectBlog))
                        {
                            try
                            {
                                int idNumber = Int32.Parse(selectBlog);
                                var titles = db.Blogs.Where(b => b.BlogId.Equals(idNumber)).Select(b => b.BlogId);
                                
                                Console.Write("Enter a post title: ");
                                string postInput = Console.ReadLine();
                                if (string.IsNullOrEmpty(postInput))
                                {
                                    logger.Error("Post title cannot be null");
                                }else
                                {
                                    var title = postInput;
                                    Console.WriteLine("Write your post:");
                                    string postContent = Console.ReadLine();
                                    var post = new Post { Title = title, Content = postContent, BlogId = idNumber};

                                    var dp = new BloggingContext();
                                    db.AddPost(post);
                                    logger.Info("Post added - {title}", title);
                                }
                            }
                            catch (Exception exc)
                            {
                                logger.Error(exc.Message);
                            }
                        }
                        else
                        {
                            logger.Error("Blog does not exist.");
                        }
                        

                    } else if (choice == "4")
                    {
                        var db = new BloggingContext();
                        Console.WriteLine("Please enter the blog you wish to view posts from: ");
                        var query = db.Blogs.OrderBy(b => b.BlogId);
                        Console.WriteLine("0) Posts from all blogs");
                        foreach (var item in query)
                        {
                            Console.WriteLine(item.BlogId + ") Posts from " + item.Name);
                        }
                        
                        string selectBlog = Console.ReadLine();
                        var convertBlogId = db.Blogs.Select(b => b.BlogId);
                        string convertedBlogId = Convert.ToString(convertBlogId);
                        if(selectBlog == "0")
                        {
                            var postQuery = db.Posts.OrderBy(p => p.PostId);
                            Console.WriteLine("All posts in the database:");
                            var postCount = db.Posts.Count();
                            Console.WriteLine(postCount + " post(s) found.");
                            foreach (var postItem in postQuery)
                            {
                                Console.WriteLine("Blog: " + postItem.Blog.Name + "\nTitle: " + postItem.Title + "\nContent: " + postItem.Content + "\n");
                            }
                        }
                        else if(convertedBlogId.Contains(selectBlog))
                        {
                            //doesnt work
                            int idNumber = Int32.Parse(selectBlog);
                            var postQuery = db.Posts.Where(b => b.BlogId.Equals(idNumber)).OrderBy(p => p.PostId);
                            Console.WriteLine("All posts in the select blog:");
                            var postCount = db.Posts.Where(b => b.BlogId.Equals(idNumber)).Count();
                            Console.WriteLine(postCount + " post(s) found.");
                            foreach (var postItem in postQuery)
                            {
                                Console.WriteLine("Blog: " + postItem.Blog.Name + "\nTitle: " + postItem.Title + "\nContent: " + postItem.Content + "\n");
                            }
                            
                        }
                        else
                        {
                            logger.Error("Blog does not exist.");
                        }
                    }
                }
                while (choice != "done");
               
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }

            logger.Info("Program ended");
        }
    }
}