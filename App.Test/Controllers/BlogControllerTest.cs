using Active_Blog_Service_API.Controllers;
using App.Application.Blogs.Commands.CreateBlog;
using App.Application.Blogs.Commands.DeleteBlog;
using App.Application.Blogs.Commands.UpdateBlog;
using App.Application.Blogs.Dto;
using App.Application.Blogs.HttpRequests;
using App.Application.Blogs.Queries.GetAllBlogs;
using App.Application.Blogs.Queries.GetBlogById;
using App.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace App.Test.Controllers
{
    [TestFixture]
    public class BlogControllerTest
    {
        private Mock<IMediator> _mediator = null!;
        private BlogController _controller = null!;
        [SetUp]
        public void Setup()
        {
            _mediator = new Mock<IMediator>();
            _controller = new BlogController(_mediator.Object);
        }
        [Test]
        [Category("Blog")]
        public async Task EnsureThatIndexReturnsOkResultWhenBlogsExist()
        {
            var blogs = new List<BlogDetailsDto>
            {
                new() {
                    BlogId = Guid.NewGuid(),
                    Title = "Blog 1",
                    CategoryId = Guid.NewGuid(),
                    BlogImage = "test1.png",
                    CreatedDate = DateTime.UtcNow,
                    UserName = "Ahmed Ali" ,
                    UserImage = "user1.png"},
                new() {
                    BlogId = Guid.NewGuid(),
                    Title = "Blog 2",
                    CategoryId = Guid.NewGuid(),
                    BlogImage = "test2.png",
                    CreatedDate = DateTime.UtcNow ,
                    UserName = "Sayed Ali",
                    UserImage = "user2.png"}
            };
            _mediator.Setup(b => b.Send(It.IsAny<GetAllBlogsQuery>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(blogs);

            var result = await _controller.Index();

            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }
        [Test]
        [Category("Blog")]
        public async Task EnsureThatIndexReturnsNoContentWhenNoBlogsExist()
        {
            _mediator.Setup(b => b.Send(It.IsAny<GetAllBlogsQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);
            var result = await _controller.Index();

            Assert.That(result, Is.TypeOf<NoContentResult>());
        }
        [Test]
        [Category("Blog")]
        public async Task EnsureThatDetailsReturnsOkResultWhenBlogExists()
        {
            var blogId = Guid.NewGuid();
            var blog = new BlogDetailsDto
            {
                BlogId = blogId,
                Title = "Blog 1",
                CategoryId = Guid.NewGuid(),
                BlogImage = "test1.png",
                CreatedDate = DateTime.UtcNow,
                UserName = "Ahmed Ali",
                UserImage = "user1.png"
            };
            _mediator.Setup(b => b.Send(It.IsAny<GetBlogByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(blog);
            var result = await _controller.Details(blogId);
            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }
        [Test]
        [Category("Blog")]
        public async Task EnsureThatDetailsReturnsNoContentWhenBlogDoesNotExist()
        {
            var blogId = Guid.NewGuid();
            _mediator.Setup(b => b.Send(It.IsAny<GetBlogByIdQuery>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync((BlogDetailsDto?)null);
            var result = await _controller.Details(blogId);
            Assert.That(result, Is.TypeOf<NoContentResult>());
        }

        [Test]
        [Category("Blog")]
        public async Task EnsureThatAddBlogReturnsOkWhenBlogIsAdded()
        {
            var command = new CreateBlogCommand
            {
                Title = "New Blog",
                CategoryId = Guid.NewGuid(),
                ImagePath = "test.png"
            };
            _mediator.Setup(b => b.Send(It.IsAny<CreateBlogCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);
            var result = await _controller.AddBlog(command);

            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }
        [Test]
        [Category("Blog")]
        public async Task EnsureThatAddBlogReturnsBadRequestWhenBlogIsNotAdded()
        {
            var command = new CreateBlogCommand
            {
                Title = "",
                CategoryId = Guid.Empty,
                ImagePath = ""
            };
            _mediator.Setup(b => b.Send(It.IsAny<CreateBlogCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);
            var result = await _controller.AddBlog(command);

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }
        [Test]
        [Category("Blog")]
        public async Task EnsureThatEditBlogReturnsOkWhenBlogIsUpdated()
        {
            var blogId = Guid.NewGuid();
            var command = new UpdateBlogRequest
            {
                Title = "Updated Blog",
                CategoryId = Guid.NewGuid(),
                ImagePath = "updated.png"
            };
            _mediator.Setup(b => b.Send(It.IsAny<UpdateBlogCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);
            var result = await _controller.EditBlog(blogId, command);
            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }
        [Test]
        [Category("Blog")]
        public async Task EnsureThatEditBlogReturnsBadRequestWhenBlogIsNotUpdated()
        {
            var blogId = Guid.NewGuid();
            var command = new UpdateBlogRequest
            {
                Title = "",
                CategoryId = Guid.Empty,
                ImagePath = ""
            };
            _mediator.Setup(b => b.Send(It.IsAny<UpdateBlogCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);
            var result = await _controller.EditBlog(blogId, command);
            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }
        [Test]
        [Category("Blog")]
        public async Task EnsureThatDeleteBlogReturnsOkWhenBlogIsDeleted()
        {
            var blogId = Guid.NewGuid();
            _mediator.Setup(b => b.Send(It.IsAny<DeleteBlogCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);
            var result = await _controller.DeleteBlog(blogId);
            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }
        [Test]
        [Category("Blog")]
        public async Task EnsureThatDeleteBlogReturnsBadRequestWhenBlogIsNotDeleted()
        {
            var blogId = Guid.NewGuid();
            _mediator.Setup(b => b.Send(It.IsAny<DeleteBlogCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);
            var result = await _controller.DeleteBlog(blogId);
            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }


    }

}

