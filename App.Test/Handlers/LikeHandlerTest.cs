using App.Application.Common.Exceptions;
using App.Application.Common.Interfaces.Services;
using App.Application.Common.Interfaces.UnitOfWork;
using App.Application.Likes.Commands.CreateLike;
using App.Application.Likes.Commands.DeleteLike;
using App.Application.Likes.Queries.GetAllBlogLikes;
using App.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace App.Test.Handlers
{
    [TestFixture]
    public class LikeHandlerTest
    {
        private Mock<ICurrentUserService> _currentUserService = null!;
        private Mock<IMemoryService<Dictionary<Guid, HashSet<Guid>>>> _memoService = null!;
        private Mock<IUnitOfWork> _unitOfWork = null!;

        private Mock<ILogger<CreateLikeHandler>> _createLogger = null!;
        private Mock<ILogger<DeleteLikeHandler>> _deleteLogger = null!;
        private Mock<ILogger<GetAllBlogLikesHandler>> _getAllLogger = null!;

        private CreateLikeHandler _createHandler = null!;
        private DeleteLikeHandler _deleteHandler = null!;
        private GetAllBlogLikesHandler _getAllHandler = null!;

        [SetUp]
        public void Setup()
        {
            _currentUserService = new Mock<ICurrentUserService>();

            _memoService =
                new Mock<IMemoryService<Dictionary<Guid, HashSet<Guid>>>>();

            _unitOfWork = new Mock<IUnitOfWork>();

            _createLogger = new Mock<ILogger<CreateLikeHandler>>();
            _deleteLogger = new Mock<ILogger<DeleteLikeHandler>>();
            _getAllLogger = new Mock<ILogger<GetAllBlogLikesHandler>>();

            _createHandler = new CreateLikeHandler(
                _currentUserService.Object,
                _memoService.Object,
                _createLogger.Object);

            _deleteHandler = new DeleteLikeHandler(
                _currentUserService.Object,
                _memoService.Object,
                _deleteLogger.Object);

            _getAllHandler = new GetAllBlogLikesHandler(
                _unitOfWork.Object,
                _getAllLogger.Object);
        }

        // =========================================
        // Create Like
        // =========================================

        [Test]
        [Category("Like")]
        public async Task EnsureThatCreateLikeHandlerReturnsSuccessWhenValidRequest()
        {
            var userId = Guid.NewGuid();
            var blogId = Guid.NewGuid();

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(userId);

            _memoService
                .Setup(x => x.GetObject("pending-like-deletions"))
                .Returns((Dictionary<Guid, HashSet<Guid>>?)null);

            _memoService
                .Setup(x => x.GetObject("blog-likes"))
                .Returns((Dictionary<Guid, HashSet<Guid>>?)null);

            var command = new CreateLikeCommand(blogId);

            await _createHandler.Handle(
                command,
                CancellationToken.None);

            _memoService.Verify(
                x => x.SetObject(
                    "blog-likes",
                    It.Is<Dictionary<Guid, HashSet<Guid>>>(d =>
                        d.ContainsKey(blogId) &&
                        d[blogId].Contains(userId))),
                Times.Once);
        }

        [Test]
        [Category("Like")]
        public async Task EnsureThatCreateLikeHandlerThrowsNotFoundExceptionWhenUserIdIsNull()
        {
            var command = new CreateLikeCommand(Guid.NewGuid());

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
        [Category("Like")]
        public async Task EnsureThatCreateLikeHandlerThrowsNotFoundExceptionWhenUserIdIsEmpty()
        {
            var command = new CreateLikeCommand(Guid.NewGuid());

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
        [Category("Like")]
        public async Task EnsureThatCreateLikeHandlerAddsUserToExistingBlogLikes()
        {
            var userId = Guid.NewGuid();
            var blogId = Guid.NewGuid();

            var likes = new Dictionary<Guid, HashSet<Guid>>
            {
                [blogId] = []
            };

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(userId);

            _memoService
                .Setup(x => x.GetObject("pending-like-deletions"))
                .Returns((Dictionary<Guid, HashSet<Guid>>?)null);

            _memoService
                .Setup(x => x.GetObject("blog-likes"))
                .Returns(likes);

            var command = new CreateLikeCommand(blogId);

            await _createHandler.Handle(
                command,
                CancellationToken.None);

            Assert.That(
                likes[blogId].Contains(userId),
                Is.True);

            _memoService.Verify(
                x => x.SetObject("blog-likes", likes),
                Times.Once);
        }

        [Test]
        [Category("Like")]
        public async Task EnsureThatCreateLikeHandlerRemovesUserFromPendingDeletions()
        {
            var userId = Guid.NewGuid();
            var blogId = Guid.NewGuid();

            var pendingDeletions =
                new Dictionary<Guid, HashSet<Guid>>
                {
                    [blogId] = [userId]
                };

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(userId);

            _memoService
                .Setup(x => x.GetObject("pending-like-deletions"))
                .Returns(pendingDeletions);

            _memoService
                .Setup(x => x.GetObject("blog-likes"))
                .Returns((Dictionary<Guid, HashSet<Guid>>?)null);

            var command = new CreateLikeCommand(blogId);

            await _createHandler.Handle(
                command,
                CancellationToken.None);

            Assert.That(
                pendingDeletions[blogId].Contains(userId),
                Is.False);

            _memoService.Verify(
                x => x.SetObject(
                    "pending-like-deletions",
                    pendingDeletions),
                Times.Once);
        }

        // =========================================
        // Delete Like
        // =========================================

        [Test]
        [Category("Like")]
        public async Task EnsureThatDeleteLikeHandlerReturnsSuccessWhenLikeExists()
        {
            var userId = Guid.NewGuid();
            var blogId = Guid.NewGuid();

            var likes = new Dictionary<Guid, HashSet<Guid>>
            {
                [blogId] = [userId]
            };

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(userId);

            _memoService
                .Setup(x => x.GetObject("blog-likes"))
                .Returns(likes);

            var command = new DeleteLikeCommand(blogId);

            await _deleteHandler.Handle(
                command,
                CancellationToken.None);

            Assert.That(
                likes[blogId].Contains(userId),
                Is.False);

            _memoService.Verify(
                x => x.SetObject(
                    "blog-likes",
                    likes),
                Times.Once);
        }

        [Test]
        [Category("Like")]
        public async Task EnsureThatDeleteLikeHandlerThrowsNotFoundExceptionWhenUserIdIsNull()
        {
            var command = new DeleteLikeCommand(Guid.NewGuid());

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
        [Category("Like")]
        public async Task EnsureThatDeleteLikeHandlerThrowsNotFoundExceptionWhenUserIdIsEmpty()
        {
            var command = new DeleteLikeCommand(Guid.NewGuid());

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
        [Category("Like")]
        public async Task EnsureThatDeleteLikeHandlerAddsUserToPendingDeletionsWhenLikeDoesNotExist()
        {
            var userId = Guid.NewGuid();
            var blogId = Guid.NewGuid();

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(userId);

            _memoService
                .Setup(x => x.GetObject("blog-likes"))
                .Returns((Dictionary<Guid, HashSet<Guid>>?)null);

            _memoService
                .Setup(x => x.GetObject("pending-like-deletions"))
                .Returns((Dictionary<Guid, HashSet<Guid>>?)null);

            var command = new DeleteLikeCommand(blogId);

            await _deleteHandler.Handle(
                command,
                CancellationToken.None);

            _memoService.Verify(
                x => x.SetObject(
                    "pending-like-deletions",
                    It.Is<Dictionary<Guid, HashSet<Guid>>>(d =>
                        d.ContainsKey(blogId) &&
                        d[blogId].Contains(userId))),
                Times.Once);
        }

        [Test]
        [Category("Like")]
        public async Task EnsureThatDeleteLikeHandlerAddsUserToExistingPendingDeletions()
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
                .Setup(x => x.GetObject("blog-likes"))
                .Returns((Dictionary<Guid, HashSet<Guid>>?)null);

            _memoService
                .Setup(x => x.GetObject("pending-like-deletions"))
                .Returns(pendingDeletions);

            var command = new DeleteLikeCommand(blogId);

            await _deleteHandler.Handle(
                command,
                CancellationToken.None);

            Assert.That(
                pendingDeletions[blogId].Contains(userId),
                Is.True);

            _memoService.Verify(
                x => x.SetObject(
                    "pending-like-deletions",
                    pendingDeletions),
                Times.Once);
        }

        [Test]
        [Category("Like")]
        public async Task EnsureThatDeleteLikeHandlerDoesNotRemoveLikeWhenUserIsNotLiker()
        {
            var userId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();
            var blogId = Guid.NewGuid();

            var likes = new Dictionary<Guid, HashSet<Guid>>
            {
                [blogId] = [otherUserId]
            };

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(userId);

            _memoService
                .Setup(x => x.GetObject("blog-likes"))
                .Returns(likes);

            _memoService
                .Setup(x => x.GetObject("pending-like-deletions"))
                .Returns((Dictionary<Guid, HashSet<Guid>>?)null);

            var command = new DeleteLikeCommand(blogId);

            await _deleteHandler.Handle(
                command,
                CancellationToken.None);

            Assert.That(
                likes[blogId].Contains(otherUserId),
                Is.True);

            _memoService.Verify(
                x => x.SetObject(
                    "pending-like-deletions",
                    It.IsAny<Dictionary<Guid, HashSet<Guid>>>()),
                Times.Once);
        }

        // =========================================
        // Get All Blog Likes
        // =========================================

        [Test]
        [Category("Like")]
        public async Task EnsureThatGetAllBlogLikesHandlerReturnsLikesWhenLikesExist()
        {
            var blogId = Guid.NewGuid();

            var likes = new List<Like>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    BlogId = blogId,
                    UserId = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    BlogId = blogId,
                    UserId = Guid.NewGuid(),
                    CreatedAt = DateTime.UtcNow
                }
            };

            _unitOfWork
                .Setup(x => x.Likes.FindAllAsync(
                    It.IsAny<System.Linq.Expressions.Expression<Func<Like, bool>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(likes);

            var query = new GetAllBlogLikesQuery(blogId);

            var result = await _getAllHandler.Handle(
                query,
                CancellationToken.None);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        [Category("Like")]
        public async Task EnsureThatGetAllBlogLikesHandlerReturnsEmptyListWhenNoLikesExist()
        {
            var blogId = Guid.NewGuid();

            _unitOfWork
                .Setup(x => x.Likes.FindAllAsync(
                    It.IsAny<System.Linq.Expressions.Expression<Func<Like, bool>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            var query = new GetAllBlogLikesQuery(blogId);

            var result = await _getAllHandler.Handle(
                query,
                CancellationToken.None);

            Assert.That(result, Is.Not.Null.And.Empty);
        }

        [Test]
        [Category("Like")]
        public async Task EnsureThatGetAllBlogLikesHandlerMapsLikeDataCorrectly()
        {
            var blogId = Guid.NewGuid();
            var likeId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var createdAt = DateTime.UtcNow;

            var likes = new List<Like>
            {
                new()
                {
                    Id = likeId,
                    BlogId = blogId,
                    UserId = userId,
                    CreatedAt = createdAt
                }
            };

            _unitOfWork
                .Setup(x => x.Likes.FindAllAsync(
                    It.IsAny<System.Linq.Expressions.Expression<Func<Like, bool>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(likes);

            var query = new GetAllBlogLikesQuery(blogId);

            var result = await _getAllHandler.Handle(
                query,
                CancellationToken.None);

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].LikeId, Is.EqualTo(likeId));
            Assert.That(result[0].BlogId, Is.EqualTo(blogId));
            Assert.That(result[0].UserId, Is.EqualTo(userId));
            Assert.That(result[0].CreatedAt, Is.EqualTo(createdAt));
        }
    }

}
