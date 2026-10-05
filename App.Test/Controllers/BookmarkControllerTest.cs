using Active_Blog_Service_API.Controllers;
using App.Application.Bookmarks.Commands.CreateBookmark;
using App.Application.Bookmarks.Commands.DeleteBookmark;
using App.Application.Bookmarks.Dto;
using App.Application.Bookmarks.Queries.GetAllBookmarksOfUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace App.Test.Controllers
{
    [TestFixture]
    public class BookmarkControllerTest
    {
        private Mock<IMediator> _mediator = null!;
        private BookmarkController _controller = null!;

        [SetUp]
        public void Setup()
        {
            _mediator = new Mock<IMediator>();
            _controller = new BookmarkController(_mediator.Object);
        }

        [Test]
        [Category("Bookmark")]
        public async Task EnsureThatIndexReturnsOkResultWhenBookmarksExist()
        {
            var bookmarks = new List<BookmarkDto>
            {
                new(Guid.NewGuid()),
                new(Guid.NewGuid())
            };

            _mediator
                .Setup(x => x.Send(
                    It.IsAny<GetAllBookmarksOfUserQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(bookmarks);

            var result = await _controller.Index();

            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }

        [Test]
        [Category("Bookmark")]
        public async Task EnsureThatIndexReturnsNoContentWhenNoBookmarksExist()
        {
            _mediator
                .Setup(x => x.Send(
                    It.IsAny<GetAllBookmarksOfUserQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            var result = await _controller.Index();

            Assert.That(result, Is.TypeOf<NoContentResult>());
        }

        [Test]
        [Category("Bookmark")]
        public async Task EnsureThatAddBookmarkReturnsNoContentWhenBookmarkIsAdded()
        {
            var blogId = Guid.NewGuid();

            _mediator
                .Setup(x => x.Send(
                    It.IsAny<CreateBookmarkCommand>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _controller.AddBookmark(blogId);

            Assert.That(result, Is.TypeOf<NoContentResult>());
        }

        [Test]
        [Category("Bookmark")]
        public async Task EnsureThatDeleteBookmarkReturnsNoContentWhenBookmarkIsDeleted()
        {
            var blogId = Guid.NewGuid();

            _mediator
                .Setup(x => x.Send(
                    It.IsAny<DeleteBookmarkCommand>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _controller.DeleteBookmark(blogId);

            Assert.That(result, Is.TypeOf<NoContentResult>());
        }
    }

}
