using App.Application.Blogs.Commands.CreateBlog;
using App.Application.Blogs.Commands.DeleteBlog;
using App.Application.Blogs.Commands.UpdateBlog;
using App.Application.Blogs.Queries.GetAllBlogs;
using App.Application.Blogs.Queries.GetBlogById;
using App.Application.Common.Exceptions;
using App.Application.Common.Interfaces.Services;
using App.Application.Common.Interfaces.UnitOfWork;
using App.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace App.Test.Handlers
{
    [TestFixture]
    public class BlogHandlerTest
    {
        private Mock<IUnitOfWork> _unitOfWork = null!;
        private Mock<ICurrentUserService> _currentUserService = null!;
        private Mock<ILogger<CreateBlogHandler>> _createLogger = null!;
        private Mock<ILogger<DeleteBlogHandler>> _deleteLogger = null!;
        private CreateBlogHandler _createHandler = null!;
        private DeleteBlogHandler _deleteHandler = null!;
        private Mock<ILogger<UpdateBlogHandler>> _updateLogger = null!;
        private UpdateBlogHandler _updateHandler = null!;
        private Mock<ILogger<GetAllBlogsHandler>> _getAllBlogsLogger = null!;
        private Mock<ILogger<GetBlogByIdHandler>> _getBlogByIdLogger = null!;

        private GetAllBlogsHandler _getAllBlogsHandler = null!;
        private GetBlogByIdHandler _getBlogByIdHandler = null!;
        [SetUp]
        public void Setup()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _currentUserService = new Mock<ICurrentUserService>();
            _createLogger = new Mock<ILogger<CreateBlogHandler>>();
            _deleteLogger = new Mock<ILogger<DeleteBlogHandler>>();
            _createHandler = new CreateBlogHandler(_unitOfWork.Object, _currentUserService.Object, _createLogger.Object);
            _deleteHandler = new DeleteBlogHandler(_unitOfWork.Object, _currentUserService.Object, _deleteLogger.Object);
            _updateLogger = new Mock<ILogger<UpdateBlogHandler>>();
            _updateHandler = new UpdateBlogHandler(
                _unitOfWork.Object,
                _currentUserService.Object,
                _updateLogger.Object);
            _getAllBlogsLogger = new Mock<ILogger<GetAllBlogsHandler>>();
            _getBlogByIdLogger = new Mock<ILogger<GetBlogByIdHandler>>();

            _getAllBlogsHandler = new GetAllBlogsHandler(
                _unitOfWork.Object,
                _getAllBlogsLogger.Object);

            _getBlogByIdHandler = new GetBlogByIdHandler(
                _unitOfWork.Object,
                _getBlogByIdLogger.Object);
        }
        [Test]
        [Category("Blog")]
        public async Task EnsureThatCreateBlogHandlerReturnsSuccessWhenValidRequest()
        {
            var command = new CreateBlogCommand
            {
                Title = "Valid Title",
                CategoryId = Guid.NewGuid(),
                ImagePath = "image.jpg"
            };
            var userId = Guid.NewGuid();
            _currentUserService.Setup(x => x.UserId).Returns(userId);
            _unitOfWork.Setup(x => x.Blogs.AddAsync(It.IsAny<Blog>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _unitOfWork.Setup(x => x.CompleteAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);
            var result = await _createHandler.Handle(command, CancellationToken.None);
            Assert.That(result, Is.EqualTo(1));
        }
        [Test]
        [Category("Blog")]
        public async Task EnsureThatCreateBlogHandlerThrowsNotFoundExceptionWhenUserIdIsNull()
        {
            var command = new CreateBlogCommand
            {
                Title = "Valid Title",
                CategoryId = Guid.NewGuid(),
                ImagePath = "image.jpg"
            };
            _currentUserService.Setup(x => x.UserId).Returns((Guid?)null);
            await Assert.ThatAsync(async () => await _createHandler.Handle(command, CancellationToken.None), Throws.TypeOf<NotFoundException>());
        }
        [Test]
        [Category("Blog")]
        public async Task EnsureThatDeleteBlogHandlerReturnsSuccessWhenValidRequest()
        {
            var blogId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var command = new DeleteBlogCommand(blogId);

            var blog = new Blog
            {
                Id = blogId,
                Title = "Test",
                CategoryId = Guid.NewGuid(),
                UserId = userId
            };

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(userId);

            _unitOfWork
                .Setup(x => x.Blogs.GetByIdAsync(command.BlogId))
                .ReturnsAsync(blog);

            _unitOfWork
                .Setup(x => x.Blogs.Delete(blog));

            _unitOfWork
                .Setup(x => x.CompleteAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await _deleteHandler.Handle(command);

            Assert.That(result, Is.EqualTo(1));
        }
        [Test]
        [Category("Blog")]
        public async Task EnsureThatDeleteBlogHandlerThrowsNotFoundExceptionWhenUserIdIsNull()
        {
            var command = new DeleteBlogCommand(Guid.NewGuid());
            _currentUserService.Setup(x => x.UserId).Returns((Guid?)null);
            await Assert.ThatAsync(async () => await _deleteHandler.Handle(command, CancellationToken.None), Throws.TypeOf<NotFoundException>());
        }
        [Test]
        [Category("Blog")]
        public async Task EnsureThatDeleteBlogHandlerThrowsForbiddenExceptionWhenUserIdIsNull()
        {
            var command = new DeleteBlogCommand(Guid.NewGuid());
            var userId = Guid.NewGuid();
            _currentUserService.Setup(x => x.UserId).Returns(userId);
            var blog = new Blog
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
            };
            _unitOfWork.Setup(x => x.Blogs.GetByIdAsync(command.BlogId))
                .ReturnsAsync(blog);

            await Assert.ThatAsync(async () => await _deleteHandler.Handle(command, CancellationToken.None), Throws.TypeOf<ForbiddenException>());
        }
        [Test]
        [Category("Blog")]
        public async Task EnsureThatUpdateBlogHandlerReturnsSuccessWhenValidRequest()
        {
            var blogId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();

            var command = new UpdateBlogCommand
            {
                BlogId = blogId,
                Title = "Updated Title",
                ImagePath = "updated-image.jpg",
                CategoryId = categoryId
            };

            var blog = new Blog
            {
                Id = blogId,
                Title = "Old Title",
                Image = "old-image.jpg",
                CategoryId = Guid.NewGuid(),
                UserId = userId
            };

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(userId);

            _unitOfWork
                .Setup(x => x.Blogs.GetByIdAsync(command.BlogId))
                .ReturnsAsync(blog);

            _unitOfWork
                .Setup(x => x.Blogs.Update(blog));

            _unitOfWork
                .Setup(x => x.CompleteAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await _updateHandler.Handle(
                command,
                CancellationToken.None);

            Assert.That(result, Is.EqualTo(1));
        }
        [Test]
        [Category("Blog")]
        public async Task EnsureThatUpdateBlogHandlerThrowsNotFoundExceptionWhenUserIdIsNull()
        {
            var command = new UpdateBlogCommand
            {
                BlogId = Guid.NewGuid(),
                Title = "Updated Title",
                ImagePath = "image.jpg",
                CategoryId = Guid.NewGuid()
            };

            _currentUserService
                .Setup(x => x.UserId)
                .Returns((Guid?)null);

            await Assert.ThatAsync(
                async () => await _updateHandler.Handle(
                    command,
                    CancellationToken.None),
                Throws.TypeOf<NotFoundException>());
        }
        [Test]
        [Category("Blog")]
        public async Task EnsureThatUpdateBlogHandlerThrowsNotFoundExceptionWhenBlogDoesNotExist()
        {
            var blogId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var command = new UpdateBlogCommand
            {
                BlogId = blogId,
                Title = "Updated Title",
                ImagePath = "image.jpg",
                CategoryId = Guid.NewGuid()
            };

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(userId);

            _unitOfWork
                .Setup(x => x.Blogs.GetByIdAsync(command.BlogId))
                .ReturnsAsync((Blog?)null);

            await Assert.ThatAsync(
                async () => await _updateHandler.Handle(
                    command,
                    CancellationToken.None),
                Throws.TypeOf<NotFoundException>());
        }
        [Test]
        [Category("Blog")]
        public async Task EnsureThatUpdateBlogHandlerThrowsForbiddenExceptionWhenUserDoesNotOwnBlog()
        {
            var blogId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var command = new UpdateBlogCommand
            {
                BlogId = blogId,
                Title = "Updated Title",
                ImagePath = "image.jpg",
                CategoryId = Guid.NewGuid()
            };

            var blog = new Blog
            {
                Id = blogId,
                Title = "Old Title",
                CategoryId = Guid.NewGuid(),
                UserId = Guid.NewGuid() // Different user
            };

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(userId);

            _unitOfWork
                .Setup(x => x.Blogs.GetByIdAsync(command.BlogId))
                .ReturnsAsync(blog);

            await Assert.ThatAsync(
                async () => await _updateHandler.Handle(
                    command,
                    CancellationToken.None),
                Throws.TypeOf<ForbiddenException>());
        }
        [Test]
        [Category("Blog")]
        public async Task EnsureThatGetAllBlogsHandlerReturnsBlogsWhenBlogsExist()
        {
            var userId = Guid.NewGuid();
            var blogId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();

            var user = new User
            {
                Id = userId,
                FName = "Ahmed",
                LName = "Ali",
                Image = "user.jpg"
            };

            var blogs = new List<Blog>
            {
                new ()
                {
                    Id = blogId,
                    Title = "Test Blog",
                    CategoryId = categoryId,
                    Image = "blog.jpg",
                    UserId = userId,
                    User = user
                }
            };

            _unitOfWork
                .Setup(x => x.Blogs.FindAllAsync(
                    It.IsAny<System.Linq.Expressions.Expression<Func<Blog, bool>>>(),
                    It.IsAny<System.Linq.Expressions.Expression<Func<Blog, object>>[]>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(blogs);

            var query = new GetAllBlogsQuery();

            var result = await _getAllBlogsHandler.Handle(
                query,
                CancellationToken.None);

            Assert.That(result.Count, Is.EqualTo(1));

        }
        [Test]
        [Category("Blog")]
        public async Task EnsureThatGetAllBlogsHandlerReturnsEmptyListWhenNoBlogsExist()
        {
            var blogs = new List<Blog>();

            _unitOfWork
                .Setup(x => x.Blogs.FindAllAsync(
                    It.IsAny<System.Linq.Expressions.Expression<Func<Blog, bool>>>(),
                    It.IsAny<System.Linq.Expressions.Expression<Func<Blog, object>>[]>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(blogs);

            var query = new GetAllBlogsQuery();

            var result = await _getAllBlogsHandler.Handle(
                query,
                CancellationToken.None);

            Assert.That(result, Is.Not.Null.And.Empty);
        }
        [Test]
        [Category("Blog")]
        public async Task EnsureThatGetBlogByIdHandlerReturnsBlogWhenBlogExists()
        {
            var blogId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var categoryId = Guid.NewGuid();

            var user = new User
            {
                Id = userId,
                FName = "Ahmed",
                LName = "Ali",
                Image = "user.jpg"
            };

            var blog = new Blog
            {
                Id = blogId,
                Title = "Test Blog",
                CategoryId = categoryId,
                Image = "blog.jpg",
                UserId = userId,
                User = user
            };

            var query = new GetBlogByIdQuery(blogId);

            _unitOfWork
                .Setup(x => x.Blogs.FindAsync(
                    It.IsAny<System.Linq.Expressions.Expression<Func<Blog, bool>>>(),
                    It.IsAny<System.Linq.Expressions.Expression<Func<Blog, object>>[]>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(blog);

            var result = await _getBlogByIdHandler.Handle(
                query,
                CancellationToken.None);

            Assert.That(result, Is.Not.Null);
        }
        [Test]
        [Category("Blog")]
        public async Task EnsureThatGetBlogByIdHandlerThrowsNotFoundExceptionWhenBlogDoesNotExist()
        {
            var blogId = Guid.NewGuid();

            var query = new GetBlogByIdQuery(blogId);

            _unitOfWork
                .Setup(x => x.Blogs.FindAsync(
                    It.IsAny<System.Linq.Expressions.Expression<Func<Blog, bool>>>(),
                    It.IsAny<System.Linq.Expressions.Expression<Func<Blog, object>>[]>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((Blog?)null);

            await Assert.ThatAsync(
                 async () => await _getBlogByIdHandler.Handle(
                     query,
                     CancellationToken.None),
                 Throws.TypeOf<NotFoundException>());
        }
    }

}
