using App.Application.Common.Interfaces.Services;
using App.Domain.Entities;
using App.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace App.Infrastructure.Persistance.Seed
{
    public class DatabaseSeeder
    {
        private const string BloggerEmail = "sara.hassan@activeblog.com";
        private const string ReaderEmail = "omar.khaled@activeblog.com";
        private const string DemoPassword = "AHMEDali2003?";

        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var context = scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

            var roleService = scope.ServiceProvider
                .GetRequiredService<IRoleService>();

            var userService = scope.ServiceProvider
                .GetRequiredService<IIdentityService>();

            var config = scope.ServiceProvider
                .GetRequiredService<IConfiguration>();

            // Apply migrations first
            await context.Database.MigrateAsync();

            // Seed roles
            await SeedRolesAsync(roleService);

            // Seed admin
            await SeedAdminAsync(
                userService,
                config,
                roleService);

            // Seed demo data
            await SeedDemoDataAsync(
                context,
                userService,
                config);
        }

        private static async Task SeedRolesAsync(IRoleService roleService)
        {
            const string roleName = "admin";

            if (await roleService.IsRoleExistsAsync(roleName))
                return;

            await roleService.CreateRoleAsync(
                roleName,
                "initial admin for system.");
        }

        private static async Task SeedAdminAsync(
            IIdentityService userService,
            IConfiguration config,
            IRoleService roleService)
        {
            string adminEmail = config["CompanyInfo:email"]!;

            var existingAdmin =
                await userService.GetUserByEmailAsync(adminEmail);

            if (existingAdmin is not null)
                return;

            var admin = new User
            {
                FName = "Ahmed",
                LName = "Ali",
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userService.CreateUserAsync(
                admin,
                config["CompanyInfo:password"]!);

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(e => e.Description));

                throw new Exception(
                    $"Failed to create admin user: {errors}");
            }

            await roleService.AssignRoleToUserAsync(
                admin,
                "admin");
        }

        private static async Task SeedDemoDataAsync(
            AppDbContext context,
            IIdentityService identityService,
            IConfiguration config)
        {

            var admin = await identityService.GetUserByEmailAsync(
                config["CompanyInfo:email"]!);

            if (admin is null)
                throw new Exception(
                    "Admin user must exist before seeding demo data.");

            var blogger =
                await GetOrCreateUserAsync(
                    identityService,
                    BloggerEmail,
                    "Sara",
                    "Hassan");

            var reader =
                await GetOrCreateUserAsync(
                    identityService,
                    ReaderEmail,
                    "Omar",
                    "Khaled");


            var categories =
                await SeedCategoriesAsync(context);


            await SeedTagsAsync(
                context,
                categories);


            var blogs =
                await SeedBlogsAsync(
                    context,
                    admin,
                    blogger,
                    categories);

            var comments =
                await SeedCommentsAsync(
                    context,
                    admin,
                    blogger,
                    reader,
                    blogs);


            await SeedInteractionsAsync(
                context,
                admin,
                blogger,
                reader,
                blogs,
                comments);
        }

        private static async Task<User> GetOrCreateUserAsync(
            IIdentityService identityService,
            string email,
            string fName,
            string lName)
        {
            var existingUser =
                await identityService.GetUserByEmailAsync(email);

            if (existingUser is not null)
                return existingUser;

            var user = new User
            {
                FName = fName,
                LName = lName,
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                IsActive = true
            };

            var result =
                await identityService.CreateUserAsync(
                    user,
                    DemoPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(e => e.Description));

                throw new Exception(
                    $"Failed to seed demo user {email}: {errors}");
            }

            return user;
        }

        private static async Task<Dictionary<string, Category>>
            SeedCategoriesAsync(AppDbContext context)
        {
            string[] names =
            [
                "Technology",
                "Travel",
                "Food",
                "Health",
                "Lifestyle",
                "Business"
            ];

            var categories = await context.Categories
                .IgnoreQueryFilters()
                .Where(c => names.Contains(c.CategoryName))
                .GroupBy(c => c.CategoryName)
                .ToDictionaryAsync(
                    g => g.Key,
                    g => g.First());

            foreach (var name in names)
            {
                if (categories.ContainsKey(name))
                    continue;
                var category = new Category
                {
                    CategoryName = name
                };

                context.Categories.Add(category);

                categories[name] = category;
            }
            await context.SaveChangesAsync();

            return categories;
        }
        private static async Task SeedTagsAsync(
            AppDbContext context,
            Dictionary<string, Category> categories)
        {
            var tags =
                new (string CategoryName, string Name)[]
                {
                    ("Technology", "dotnet"),
                    ("Technology", "architecture"),
                    ("Technology", "cqrs"),
                    ("Technology", "mediatr"),

                    ("Travel", "egypt"),
                    ("Travel", "hidden-gems"),
                    ("Travel", "adventure"),

                    ("Food", "street-food"),
                    ("Food", "egyptian-cuisine"),
                    ("Food", "recipes"),

                    ("Health", "fitness"),
                    ("Health", "nutrition"),

                    ("Lifestyle", "productivity"),
                    ("Lifestyle", "minimalism"),

                    ("Business", "startups"),
                    ("Business", "marketing"),
                };

            foreach (var (CategoryName, Name) in tags)
            {
                var categoryId =
                    categories[CategoryName].Id;

                var exists = await context.Tags
                    .AnyAsync(t =>
                        t.Name == Name &&
                        t.CategoryId == categoryId);

                if (exists)
                    continue;

                context.Tags.Add(
                    new Tag
                    {
                        Name = Name,
                        CategoryId = categoryId
                    });
            }
            await context.SaveChangesAsync();
        }

        private static async Task<List<Blog>> SeedBlogsAsync(
            AppDbContext context,
            User admin,
            User blogger,
            Dictionary<string, Category> categories)
        {
            var blogDefinitions =
                new[]
                {
                    new
                    {
                        Title =
                            "Getting Started with Clean Architecture in .NET 8",

                        Image =
                            "uploads/blogs/clean-architecture.jpg",

                        Category =
                            "Technology",

                        UserId =
                            admin.Id
                    },

                    new
                    {
                        Title =
                            "A Practical Guide to CQRS with MediatR",

                        Image =
                            "uploads/blogs/cqrs-mediatr.jpg",

                        Category =
                            "Technology",

                        UserId =
                            admin.Id
                    },

                    new
                    {
                        Title =
                            "10 Hidden Gems to Visit in Egypt",

                        Image =
                            "uploads/blogs/egypt-hidden-gems.jpg",

                        Category =
                            "Travel",

                        UserId =
                            blogger.Id
                    },

                    new
                    {
                        Title =
                            "The Ultimate Guide to Egyptian Street Food",

                        Image =
                            "uploads/blogs/egyptian-street-food.jpg",

                        Category =
                            "Food",

                        UserId =
                            blogger.Id
                    }
                };

            var blogs = new List<Blog>();

            foreach (var definition in blogDefinitions)
            {
                var existingBlog =
                    await context.Blogs
                        .IgnoreQueryFilters()
                        .FirstOrDefaultAsync(b =>
                            b.Title == definition.Title);

                if (existingBlog is not null)
                {
                    blogs.Add(existingBlog);
                    continue;
                }

                var blog = new Blog
                {
                    Title = definition.Title,
                    Image = definition.Image,
                    CategoryId =
                        categories[definition.Category].Id,
                    UserId = definition.UserId
                };

                context.Blogs.Add(blog);

                blogs.Add(blog);
            }

            await context.SaveChangesAsync();


            await SeedContentBlocksAsync(
                context,
                blogs);

            return blogs;
        }

        private static async Task SeedContentBlocksAsync(
            AppDbContext context,
            List<Blog> blogs)
        {

            var cleanArchitecture = blogs.First(b =>
                b.Title ==
                "Getting Started with Clean Architecture in .NET 8");

            await AddContentBlockIfNotExistsAsync(
                context,
                cleanArchitecture.Id,
                ContentBlockType.Heading,
                1,
                "Why Clean Architecture?");

            await AddContentBlockIfNotExistsAsync(
                context,
                cleanArchitecture.Id,
                ContentBlockType.Text,
                2,
                "Clean Architecture keeps your business logic independent of frameworks, databases, and delivery mechanisms. That independence is what makes the solution easy to test and safe to evolve.");

            await AddContentBlockIfNotExistsAsync(
                context,
                cleanArchitecture.Id,
                ContentBlockType.Quote,
                3,
                "Dependencies point inward: everything references the domain, and the domain references nothing.");

            await AddContentBlockIfNotExistsAsync(
                context,
                cleanArchitecture.Id,
                ContentBlockType.Code,
                4,
                "builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();");

            await AddContentBlockIfNotExistsAsync(
                context,
                cleanArchitecture.Id,
                ContentBlockType.Text,
                5,
                "Start by splitting the solution into Domain, Application, Infrastructure, and API projects, then let the dependency rule guide every reference you add.");

            var cqrsGuide = blogs.First(b =>
                b.Title ==
                "A Practical Guide to CQRS with MediatR");

            await AddContentBlockIfNotExistsAsync(
                context,
                cqrsGuide.Id,
                ContentBlockType.Heading,
                1,
                "Commands vs Queries");

            await AddContentBlockIfNotExistsAsync(
                context,
                cqrsGuide.Id,
                ContentBlockType.Text,
                2,
                "Commands change state and return almost nothing, while queries return data and change nothing. Separating them keeps every handler focused on a single responsibility.");

            await AddContentBlockIfNotExistsAsync(
                context,
                cqrsGuide.Id,
                ContentBlockType.Code,
                3,
                "var blog = await _mediator.Send(new GetBlogByIdQuery(id), cancellationToken);");

            await AddContentBlockIfNotExistsAsync(
                context,
                cqrsGuide.Id,
                ContentBlockType.Heading,
                4,
                "Cross-Cutting with Pipeline Behaviors");

            await AddContentBlockIfNotExistsAsync(
                context,
                cqrsGuide.Id,
                ContentBlockType.Text,
                5,
                "Validation, logging, and performance measuring all live in pipeline behaviors, so handlers stay clean and every request gets the same guarantees.");



            var hiddenGems = blogs.First(b =>
                b.Title ==
                "10 Hidden Gems to Visit in Egypt");

            await AddContentBlockIfNotExistsAsync(
                context,
                hiddenGems.Id,
                ContentBlockType.Heading,
                1,
                "Beyond the Pyramids");

            await AddContentBlockIfNotExistsAsync(
                context,
                hiddenGems.Id,
                ContentBlockType.Text,
                2,
                "From the desert springs of Siwa Oasis to the painted tombs of Abydos, Egypt rewards travelers who wander past the classic itinerary.");

            await AddContentBlockIfNotExistsAsync(
                context,
                hiddenGems.Id,
                ContentBlockType.Image,
                3,
                "uploads/blogs/siwa-oasis.jpg");

            await AddContentBlockIfNotExistsAsync(
                context,
                hiddenGems.Id,
                ContentBlockType.Quote,
                4,
                "The best destinations are the ones you have to work a little to reach.");

            var streetFood = blogs.First(b =>
                b.Title ==
                "The Ultimate Guide to Egyptian Street Food");

            await AddContentBlockIfNotExistsAsync(
                context,
                streetFood.Id,
                ContentBlockType.Heading,
                1,
                "A Food Lover's Route Through Cairo");

            await AddContentBlockIfNotExistsAsync(
                context,
                streetFood.Id,
                ContentBlockType.Text,
                2,
                "Koshari, foul, taameya, and hawawshi: a full day of eating through Downtown Cairo costs less than a coffee anywhere else.");

            await AddContentBlockIfNotExistsAsync(
                context,
                streetFood.Id,
                ContentBlockType.Image,
                3,
                "uploads/blogs/koshari.jpg");

            await AddContentBlockIfNotExistsAsync(
                context,
                streetFood.Id,
                ContentBlockType.Text,
                4,
                "End the route at a traditional ahwa with a glass of mint tea and a game of backgammon.");
        }

        private static async Task AddContentBlockIfNotExistsAsync(
            AppDbContext context,
            Guid blogId,
            ContentBlockType type,
            int order,
            string data)
        {
            var exists = await context.ContentBlocks
                .AnyAsync(x =>
                    x.BlogId == blogId &&
                    x.Type == type &&
                    x.Order == order);

            if (exists)
                return;

            context.ContentBlocks.Add(
                new ContentBlock
                {
                    BlogId = blogId,
                    Type = type,
                    Order = order,
                    Data = data
                });

            await context.SaveChangesAsync();
        }


        private static async Task<List<Comment>> SeedCommentsAsync(
            AppDbContext context,
            User admin,
            User blogger,
            User reader,
            List<Blog> blogs)
        {
            var comments = new List<Comment>();

            var dependencyQuestion =
                await GetOrCreateCommentAsync(
                    context,
                    blogs[0].Id,
                    reader.Id,
                    "This finally made the dependency rule click for me. Thanks!");

            comments.Add(dependencyQuestion);

            var dependencyAnswer =
                await GetOrCreateCommentAsync(
                    context,
                    blogs[0].Id,
                    admin.Id,
                    "Glad it helped! The next post covers validation behaviors in detail.");

            if (dependencyAnswer.ParentCommentId == null)
            {
                dependencyAnswer.ParentComment =
                    dependencyQuestion;

                await context.SaveChangesAsync();
            }

            comments.Add(dependencyAnswer);

            var smallApiQuestion =
                await GetOrCreateCommentAsync(
                    context,
                    blogs[0].Id,
                    blogger.Id,
                    "Would you recommend this structure for a small API too, or is it overkill?");

            comments.Add(smallApiQuestion);

            var siwaComment =
                await GetOrCreateCommentAsync(
                    context,
                    blogs[2].Id,
                    reader.Id,
                    "Adding Siwa Oasis to my travel list immediately!");

            comments.Add(siwaComment);

            return comments;
        }

        private static async Task<Comment> GetOrCreateCommentAsync(
            AppDbContext context,
            Guid blogId,
            Guid userId,
            string content)
        {
            var existing =
                await context.Comments
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(c =>
                        c.BlogId == blogId &&
                        c.UserId == userId &&
                        c.CommentContent == content);

            if (existing is not null)
                return existing;

            var comment = new Comment
            {
                BlogId = blogId,
                UserId = userId,
                CommentContent = content
            };

            context.Comments.Add(comment);

            await context.SaveChangesAsync();

            return comment;
        }

        private static async Task SeedInteractionsAsync(
            AppDbContext context,
            User admin,
            User blogger,
            User reader,
            List<Blog> blogs,
            List<Comment> comments)
        {


            await AddLikeIfNotExistsAsync(
                context,
                blogs[0].Id,
                reader.Id);

            await AddLikeIfNotExistsAsync(
                context,
                blogs[1].Id,
                reader.Id);

            await AddLikeIfNotExistsAsync(
                context,
                blogs[1].Id,
                blogger.Id);

            await AddLikeIfNotExistsAsync(
                context,
                blogs[2].Id,
                reader.Id);

            await AddLikeIfNotExistsAsync(
                context,
                blogs[2].Id,
                admin.Id);

            await AddLikeIfNotExistsAsync(
                context,
                blogs[3].Id,
                admin.Id);

            await AddBookmarkIfNotExistsAsync(
                context,
                blogs[0].Id,
                reader.Id);

            await AddBookmarkIfNotExistsAsync(
                context,
                blogs[2].Id,
                reader.Id);

            await AddFollowIfNotExistsAsync(
                context,
                admin.Id,
                reader.Id);

            await AddFollowIfNotExistsAsync(
                context,
                admin.Id,
                blogger.Id);

            await AddFollowIfNotExistsAsync(
                context,
                blogger.Id,
                reader.Id);

            await AddNotificationIfNotExistsAsync(
                context,
                reader.Id,
                admin.Id,
                comments[0].CommentContent,
                $"api/comments/{comments[0].Id}",
                false);

            await AddNotificationIfNotExistsAsync(
                context,
                admin.Id,
                reader.Id,
                comments[1].CommentContent,
                $"api/comments/{comments[1].Id}",
                true);


            await AddReportIfNotExistsAsync(
                context,
                reader.Id,
                blogs[3].Id,
                ReportReason.Copyright,
                "Some photos in this post look like they were taken from my gallery without permission.",
                ReportStatus.Pending,
                null);

            await AddReportIfNotExistsAsync(
                context,
                blogger.Id,
                blogs[1].Id,
                ReportReason.Other,
                "The MediatR code sample throws on .NET 7.",
                ReportStatus.Resolved,
                admin.Id);
        }


        private static async Task AddLikeIfNotExistsAsync(
            AppDbContext context,
            Guid blogId,
            Guid userId)
        {
            var exists = await context.Likes.AnyAsync(x =>
                x.BlogId == blogId &&
                x.UserId == userId);

            if (exists)
                return;

            context.Likes.Add(
                new Like
                {
                    BlogId = blogId,
                    UserId = userId
                });

            await context.SaveChangesAsync();
        }


        private static async Task AddBookmarkIfNotExistsAsync(
            AppDbContext context,
            Guid blogId,
            Guid userId)
        {
            var exists = await context.Bookmarks.AnyAsync(x =>
                x.BlogId == blogId &&
                x.UserId == userId);

            if (exists)
                return;

            context.Bookmarks.Add(
                new Bookmark
                {
                    BlogId = blogId,
                    UserId = userId
                });

            await context.SaveChangesAsync();
        }


        private static async Task AddFollowIfNotExistsAsync(
            AppDbContext context,
            Guid bloggerId,
            Guid followerId)
        {
            var exists = await context.Follows.AnyAsync(x =>
                x.BloggerId == bloggerId &&
                x.FollowerId == followerId);

            if (exists)
                return;

            context.Follows.Add(
                new Follow
                {
                    BloggerId = bloggerId,
                    FollowerId = followerId
                });

            await context.SaveChangesAsync();
        }

        private static async Task AddNotificationIfNotExistsAsync(
            AppDbContext context,
            Guid senderId,
            Guid receiverId,
            string message,
            string link,
            bool isRead)
        {
            var exists = await context.Notifications.AnyAsync(x =>
                x.SenderId == senderId &&
                x.ReceiverId == receiverId &&
                x.Message == message &&
                x.Link == link);

            if (exists)
                return;

            context.Notifications.Add(
                new Notification
                {
                    SenderId = senderId,
                    ReceiverId = receiverId,
                    Message = message,
                    IsRead = isRead,
                    Link = link
                });

            await context.SaveChangesAsync();
        }
        private static async Task AddReportIfNotExistsAsync(
            AppDbContext context,
            Guid reporterId,
            Guid blogId,
            ReportReason reason,
            string description,
            ReportStatus status,
            Guid? reviewerId)
        {
            var exists = await context.Reports.AnyAsync(x =>
                x.ReporterId == reporterId &&
                x.BlogId == blogId &&
                x.Description == description);

            if (exists)
                return;

            var report = new Report
            {
                ReporterId = reporterId,
                BlogId = blogId,
                Reason = reason,
                Description = description,
                Status = status,
                ReviewerId = reviewerId
            };

            context.Reports.Add(report);

            await context.SaveChangesAsync();
        }
    }
}

