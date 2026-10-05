using App.Application.Bookmarks.Commands.CreateBookmark;
using App.Application.Bookmarks.Commands.DeleteBookmark;
using App.Application.Bookmarks.Queries.GetAllBookmarksOfUser;
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
    public class BookmarkHandlerTest
    {
        private Mock<IMemoryService<Dictionary<Guid, HashSet<Guid>>>> _memoService = null!;
        private Mock<ICurrentUserService> _currentUserService = null!;
        private Mock<IUnitOfWork> _unitOfWork = null!;

        private Mock<ILogger<CreateBookmarkHandler>> _createLogger = null!;
        private Mock<ILogger<DeleteBookmarkHandler>> _deleteLogger = null!;
        private Mock<ILogger<GetAllBookmarksOfUserHandler>> _getAllLogger = null!;

        private CreateBookmarkHandler _createHandler = null!;
        private DeleteBookmarkHandler _deleteHandler = null!;
        private GetAllBookmarksOfUserHandler _getAllHandler = null!;

        [SetUp]
        public void Setup()
        {
            _memoService =
                new Mock<IMemoryService<Dictionary<Guid, HashSet<Guid>>>>();

            _currentUserService = new Mock<ICurrentUserService>();

            _unitOfWork = new Mock<IUnitOfWork>();

            _createLogger = new Mock<ILogger<CreateBookmarkHandler>>();
            _deleteLogger = new Mock<ILogger<DeleteBookmarkHandler>>();
            _getAllLogger = new Mock<ILogger<GetAllBookmarksOfUserHandler>>();

            _createHandler = new CreateBookmarkHandler(
                _memoService.Object,
                _currentUserService.Object,
                _createLogger.Object);

            _deleteHandler = new DeleteBookmarkHandler(
                _memoService.Object,
                _currentUserService.Object,
                _deleteLogger.Object);

            _getAllHandler = new GetAllBookmarksOfUserHandler(
                _unitOfWork.Object,
                _currentUserService.Object,
                _getAllLogger.Object);
        }

        // =========================
        // Create Bookmark
        // =========================

        [Test]
        [Category("Bookmark")]
        public async Task EnsureThatCreateBookmarkHandlerReturnsSuccessWhenValidRequest()
        {
            var userId = Guid.NewGuid();
            var blogId = Guid.NewGuid();

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(userId);

            _memoService
                .Setup(x => x.GetObject("pending-bookmarks-deletions"))
                .Returns((Dictionary<Guid, HashSet<Guid>>?)null);

            _memoService
                .Setup(x => x.GetObject("blog-bookmarks"))
                .Returns((Dictionary<Guid, HashSet<Guid>>?)null);

            var command = new CreateBookmarkCommand(blogId);

            await _createHandler.Handle(
                command,
                CancellationToken.None);

            _memoService.Verify(
                x => x.SetObject(
                    "blog-bookmarks",
                    It.Is<Dictionary<Guid, HashSet<Guid>>>(d =>
                        d.ContainsKey(blogId) &&
                        d[blogId].Contains(userId))),
                Times.Once);
        }

        [Test]
        [Category("Bookmark")]
        public async Task EnsureThatCreateBookmarkHandlerThrowsNotFoundExceptionWhenUserIdIsNull()
        {
            var command = new CreateBookmarkCommand(Guid.NewGuid());

            _currentUserService
                .Setup(x => x.UserId)
                .Returns((Guid?)null);
            try
            {
                await _createHandler.Handle(
                    command,
                    CancellationToken.None);
            }
            catch (NotFoundException ex)
            {
                Assert.That(ex.Message, Is.EqualTo("User not found."));
                return;
            }

        }

        [Test]
        [Category("Bookmark")]
        public async Task EnsureThatCreateBookmarkHandlerThrowsNotFoundExceptionWhenUserIdIsEmpty()
        {
            var command = new CreateBookmarkCommand(Guid.NewGuid());

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(Guid.Empty);
            try
            {
                await _createHandler.Handle(
                    command,
                    CancellationToken.None);
            }
            catch (NotFoundException ex)
            {
                Assert.That(ex.Message, Is.EqualTo("User not found."));
                return;
            }

        }

        [Test]
        [Category("Bookmark")]
        public async Task EnsureThatCreateBookmarkHandlerAddsUserToExistingBookmark()
        {
            var userId = Guid.NewGuid();
            var blogId = Guid.NewGuid();

            var bookmarks = new Dictionary<Guid, HashSet<Guid>>
            {
                [blogId] = []
            };

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(userId);

            _memoService
                .Setup(x => x.GetObject("pending-bookmarks-deletions"))
                .Returns((Dictionary<Guid, HashSet<Guid>>?)null);

            _memoService
                .Setup(x => x.GetObject("blog-bookmarks"))
                .Returns(bookmarks);

            var command = new CreateBookmarkCommand(blogId);

            await _createHandler.Handle(
                command,
                CancellationToken.None);

            Assert.That(
                bookmarks[blogId].Contains(userId),
                Is.True);

            _memoService.Verify(
                x => x.SetObject("blog-bookmarks", bookmarks),
                Times.Once);
        }

        [Test]
        [Category("Bookmark")]
        public async Task EnsureThatCreateBookmarkHandlerRemovesUserFromPendingDeletions()
        {
            var userId = Guid.NewGuid();
            var blogId = Guid.NewGuid();

            var pendingDeletions = new Dictionary<Guid, HashSet<Guid>>
            {
                [blogId] = [userId]
            };

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(userId);

            _memoService
                .Setup(x => x.GetObject("pending-bookmarks-deletions"))
                .Returns(pendingDeletions);

            _memoService
                .Setup(x => x.GetObject("blog-bookmarks"))
                .Returns((Dictionary<Guid, HashSet<Guid>>?)null);

            var command = new CreateBookmarkCommand(blogId);

            await _createHandler.Handle(
                command,
                CancellationToken.None);

            Assert.That(
                pendingDeletions[blogId].Contains(userId),
                Is.False);

            _memoService.Verify(
                x => x.SetObject(
                    "pending-bookmarks-deletions",
                    pendingDeletions),
                Times.Once);
        }

        // =========================
        // Delete Bookmark
        // =========================

        [Test]
        [Category("Bookmark")]
        public async Task EnsureThatDeleteBookmarkHandlerReturnsSuccessWhenBookmarkExists()
        {
            var userId = Guid.NewGuid();
            var blogId = Guid.NewGuid();

            var bookmarks = new Dictionary<Guid, HashSet<Guid>>
            {
                [blogId] = [userId]
            };

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(userId);

            _memoService
                .Setup(x => x.GetObject("blog-bookmarks"))
                .Returns(bookmarks);

            var command = new DeleteBookmarkCommand(blogId);

            await _deleteHandler.Handle(
                command,
                CancellationToken.None);

            Assert.That(
                bookmarks[blogId].Contains(userId),
                Is.False);

            _memoService.Verify(
                x => x.SetObject(
                    "blog-bookmarks",
                    bookmarks),
                Times.Once);
        }

        [Test]
        [Category("Bookmark")]
        public async Task EnsureThatDeleteBookmarkHandlerThrowsNotFoundExceptionWhenUserIdIsNull()
        {
            var command = new DeleteBookmarkCommand(Guid.NewGuid());

            _currentUserService
                .Setup(x => x.UserId)
                .Returns((Guid?)null);
            try
            {
                await _deleteHandler.Handle(
                    command,
                    CancellationToken.None);
            }
            catch (NotFoundException ex)
            {
                Assert.That(ex.Message, Is.EqualTo("User not found."));
                return;
            }

        }

        [Test]
        [Category("Bookmark")]
        public async Task EnsureThatDeleteBookmarkHandlerThrowsNotFoundExceptionWhenUserIdIsEmpty()
        {
            var command = new DeleteBookmarkCommand(Guid.NewGuid());

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(Guid.Empty);
            try
            {
                await _deleteHandler.Handle(
                    command,
                    CancellationToken.None);
            }
            catch (NotFoundException ex)
            {
                Assert.That(ex.Message, Is.EqualTo("User not found."));
                return;
            }
        }

        [Test]
        [Category("Bookmark")]
        public async Task EnsureThatDeleteBookmarkHandlerAddsUserToPendingDeletionsWhenBookmarkDoesNotExist()
        {
            var userId = Guid.NewGuid();
            var blogId = Guid.NewGuid();

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(userId);

            _memoService
                .Setup(x => x.GetObject("blog-bookmarks"))
                .Returns((Dictionary<Guid, HashSet<Guid>>?)null);

            _memoService
                .Setup(x => x.GetObject("pending-bookmark-deletions"))
                .Returns((Dictionary<Guid, HashSet<Guid>>?)null);

            var command = new DeleteBookmarkCommand(blogId);

            await _deleteHandler.Handle(
                command,
                CancellationToken.None);

            _memoService.Verify(
                x => x.SetObject(
                    "pending-bookmark-deletions",
                    It.Is<Dictionary<Guid, HashSet<Guid>>>(d =>
                        d.ContainsKey(blogId) &&
                        d[blogId].Contains(userId))),
                Times.Once);
        }

        [Test]
        [Category("Bookmark")]
        public async Task EnsureThatDeleteBookmarkHandlerAddsUserToExistingPendingDeletions()
        {
            var userId = Guid.NewGuid();
            var blogId = Guid.NewGuid();

            var pendingDeletions =
                new Dictionary<Guid, HashSet<Guid>>
                {
                    [blogId] = []
                };

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(userId);

            _memoService
                .Setup(x => x.GetObject("blog-bookmarks"))
                .Returns((Dictionary<Guid, HashSet<Guid>>?)null);

            _memoService
                .Setup(x => x.GetObject("pending-bookmark-deletions"))
                .Returns(pendingDeletions);

            var command = new DeleteBookmarkCommand(blogId);

            await _deleteHandler.Handle(
                command,
                CancellationToken.None);

            Assert.That(
                pendingDeletions[blogId].Contains(userId),
                Is.True);

            _memoService.Verify(
                x => x.SetObject(
                    "pending-bookmark-deletions",
                    pendingDeletions),
                Times.Once);
        }

        [Test]
        [Category("Bookmark")]
        public async Task EnsureThatDeleteBookmarkHandlerDoesNotRemoveBookmarkWhenUserIsNotBookmarker()
        {
            var userId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();
            var blogId = Guid.NewGuid();

            var bookmarks = new Dictionary<Guid, HashSet<Guid>>
            {
                [blogId] = [otherUserId]
            };

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(userId);

            _memoService
                .Setup(x => x.GetObject("blog-bookmarks"))
                .Returns(bookmarks);

            _memoService
                .Setup(x => x.GetObject("pending-bookmark-deletions"))
                .Returns((Dictionary<Guid, HashSet<Guid>>?)null);

            var command = new DeleteBookmarkCommand(blogId);

            await _deleteHandler.Handle(
                command,
                CancellationToken.None);

            Assert.That(
                bookmarks[blogId].Contains(otherUserId),
                Is.True);

            _memoService.Verify(
                x => x.SetObject(
                    "pending-bookmark-deletions",
                    It.IsAny<Dictionary<Guid, HashSet<Guid>>>()),
                Times.Once);
        }

        // =========================
        // Get All Bookmarks
        // =========================

        [Test]
        [Category("Bookmark")]
        public async Task EnsureThatGetAllBookmarksOfUserHandlerReturnsBookmarksWhenBookmarksExist()
        {
            var userId = Guid.NewGuid();

            var bookmarks = new List<Bookmark>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    BlogId = Guid.NewGuid(),
                    UserId = userId
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    BlogId = Guid.NewGuid(),
                    UserId = userId
                }
            };

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(userId);

            _unitOfWork
                .Setup(x => x.Bookmarks.FindAllAsync(
                    It.IsAny<System.Linq.Expressions.Expression<Func<Bookmark, bool>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(bookmarks);

            var query = new GetAllBookmarksOfUserQuery();

            var result = await _getAllHandler.Handle(
                query,
                CancellationToken.None);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        [Category("Bookmark")]
        public async Task EnsureThatGetAllBookmarksOfUserHandlerReturnsEmptyListWhenNoBookmarksExist()
        {
            var userId = Guid.NewGuid();

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(userId);

            _unitOfWork
                .Setup(x => x.Bookmarks.FindAllAsync(
                    It.IsAny<System.Linq.Expressions.Expression<Func<Bookmark, bool>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            var query = new GetAllBookmarksOfUserQuery();

            var result = await _getAllHandler.Handle(
                query,
                CancellationToken.None);

            Assert.That(result, Is.Not.Null.And.Empty);
        }

        [Test]
        [Category("Bookmark")]
        public async Task EnsureThatGetAllBookmarksOfUserHandlerThrowsNotFoundExceptionWhenUserIdIsNull()
        {
            var query = new GetAllBookmarksOfUserQuery();

            _currentUserService
                .Setup(x => x.UserId)
                .Returns((Guid?)null);

            await Assert.ThatAsync(
                async () => await _getAllHandler.Handle(
                    query,
                    CancellationToken.None),
                Throws.TypeOf<NotFoundException>());
        }

        [Test]
        [Category("Bookmark")]
        public async Task EnsureThatGetAllBookmarksOfUserHandlerThrowsNotFoundExceptionWhenUserIdIsEmpty()
        {
            var query = new GetAllBookmarksOfUserQuery();

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(Guid.Empty);

            await Assert.ThatAsync(
                async () => await _getAllHandler.Handle(
                    query,
                    CancellationToken.None),
                Throws.TypeOf<NotFoundException>());
        }
    }

}
