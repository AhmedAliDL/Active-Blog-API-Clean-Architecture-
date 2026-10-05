using App.Application.Comments.Command.CreateComment;
using App.Application.Comments.Command.DeleteComment;
using App.Application.Comments.Command.UpdateComment;
using App.Application.Comments.Queries.GetAllCommentOfBlog;
using App.Application.Comments.Queries.GetCommentById;
using App.Application.Common.Enums;
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
    public class CommentHandlerTest
    {
        private Mock<IUnitOfWork> _unitOfWork = null!;
        private Mock<ICurrentUserService> _currentUserService = null!;
        private Mock<INotificationService> _notificationService = null!;

        private Mock<ILogger<CreateCommentHandler>> _createLogger = null!;
        private Mock<ILogger<DeleteCommentHandler>> _deleteLogger = null!;
        private Mock<ILogger<UpdateCommentHandler>> _updateLogger = null!;
        private Mock<ILogger<GetCommentsOfBlogHandler>> _getAllLogger = null!;

        private CreateCommentHandler _createHandler = null!;
        private DeleteCommentHandler _deleteHandler = null!;
        private UpdateCommentHandler _updateHandler = null!;
        private GetCommentsOfBlogHandler _getAllHandler = null!;
        private GetCommentByIdHandler _getByIdHandler = null!;

        [SetUp]
        public void Setup()
        {
            _unitOfWork = new Mock<IUnitOfWork>();
            _currentUserService = new Mock<ICurrentUserService>();
            _notificationService = new Mock<INotificationService>();

            _createLogger = new Mock<ILogger<CreateCommentHandler>>();
            _deleteLogger = new Mock<ILogger<DeleteCommentHandler>>();
            _updateLogger = new Mock<ILogger<UpdateCommentHandler>>();
            _getAllLogger = new Mock<ILogger<GetCommentsOfBlogHandler>>();

            _createHandler = new CreateCommentHandler(
                _unitOfWork.Object,
                _currentUserService.Object,
                _createLogger.Object,
                _notificationService.Object);

            _deleteHandler = new DeleteCommentHandler(
                _unitOfWork.Object,
                _currentUserService.Object,
                _deleteLogger.Object);

            _updateHandler = new UpdateCommentHandler(
                _unitOfWork.Object,
                _currentUserService.Object,
                _updateLogger.Object);

            _getAllHandler = new GetCommentsOfBlogHandler(
                _unitOfWork.Object,
                _getAllLogger.Object);

            _getByIdHandler = new GetCommentByIdHandler(
                _unitOfWork.Object);
        }

        // =========================================
        // Create Comment Handler
        // =========================================

        [Test]
        [Category("Comment")]
        public async Task EnsureCreateCommentReturnsSuccess()
        {
            var userId = Guid.NewGuid();
            var blogId = Guid.NewGuid();

            var command = new CreateCommentCommand
            {
                BlogId = blogId,
                CommentContent = "Great article!",
                ParentCommentId = null
            };

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(userId);

            _unitOfWork
                .Setup(x => x.Comments.AddAsync(
                    It.IsAny<Comment>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _unitOfWork
                .Setup(x => x.CompleteAsync(
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await _createHandler.Handle(
                command,
                CancellationToken.None);

            Assert.That(result, Is.EqualTo(1));

            _unitOfWork.Verify(
                x => x.Comments.AddAsync(
                    It.Is<Comment>(c =>
                        c.BlogId == blogId &&
                        c.UserId == userId &&
                        c.CommentContent == "Great article!" &&
                        c.ParentCommentId == null),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _unitOfWork.Verify(
                x => x.CompleteAsync(
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        [Category("Comment")]
        public async Task EnsureCreateCommentThrowsNotFoundWhenUserIsNull()
        {
            var command = new CreateCommentCommand
            {
                BlogId = Guid.NewGuid(),
                CommentContent = "Great article!"
            };

            _currentUserService
                .Setup(x => x.UserId)
                .Returns((Guid?)null);

            await Assert.ThatAsync(
                async () => await _createHandler.Handle(
                    command,
                    CancellationToken.None),
                Throws.TypeOf<NotFoundException>());
        }

        [Test]
        [Category("Comment")]
        public async Task EnsureCreateCommentThrowsNotFoundWhenUserIdIsEmpty()
        {
            var command = new CreateCommentCommand
            {
                BlogId = Guid.NewGuid(),
                CommentContent = "Great article!"
            };

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(Guid.Empty);

            await Assert.ThatAsync(
                async () => await _createHandler.Handle(
                    command,
                    CancellationToken.None),
                Throws.TypeOf<NotFoundException>());
        }

        [Test]
        [Category("Comment")]
        public async Task EnsureCreateCommentThrowsNotFoundWhenParentCommentDoesNotExist()
        {
            var userId = Guid.NewGuid();
            var parentCommentId = Guid.NewGuid();

            var command = new CreateCommentCommand
            {
                BlogId = Guid.NewGuid(),
                CommentContent = "Reply",
                ParentCommentId = parentCommentId
            };

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(userId);

            _unitOfWork
                .Setup(x => x.Comments.GetByIdAsync(parentCommentId))
                .ReturnsAsync((Comment?)null);

            await Assert.ThatAsync(
                async () => await _createHandler.Handle(
                    command,
                    CancellationToken.None),
                Throws.TypeOf<NotFoundException>());
        }

        [Test]
        [Category("Comment")]
        public async Task EnsureCreateCommentCreatesReplyWhenParentCommentExists()
        {
            var userId = Guid.NewGuid();
            var blogId = Guid.NewGuid();
            var parentCommentId = Guid.NewGuid();

            var parentComment = new Comment
            {
                Id = parentCommentId,
                BlogId = blogId,
                UserId = Guid.NewGuid(),
                CommentContent = "Parent comment"
            };

            var command = new CreateCommentCommand
            {
                BlogId = blogId,
                CommentContent = "Reply comment",
                ParentCommentId = parentCommentId
            };

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(userId);

            _unitOfWork
                .Setup(x => x.Comments.GetByIdAsync(parentCommentId))
                .ReturnsAsync(parentComment);

            _unitOfWork
                .Setup(x => x.Comments.AddAsync(
                    It.IsAny<Comment>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _unitOfWork
                .Setup(x => x.CompleteAsync(
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await _createHandler.Handle(
                command,
                CancellationToken.None);

            Assert.That(result, Is.EqualTo(1));

            _unitOfWork.Verify(
                x => x.Comments.AddAsync(
                    It.Is<Comment>(c =>
                        c.BlogId == blogId &&
                        c.UserId == userId &&
                        c.CommentContent == "Reply comment" &&
                        c.ParentCommentId == parentCommentId),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        // =========================================
        // Delete Comment Handler
        // =========================================

        [Test]
        [Category("Comment")]
        public async Task EnsureDeleteCommentReturnsSuccess()
        {
            var userId = Guid.NewGuid();
            var commentOwnerId = Guid.NewGuid();

            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                BlogId = Guid.NewGuid(),
                UserId = commentOwnerId,
                CommentContent = "Comment"
            };

            var command = new DeleteCommentCommand(
                comment.Id,
                comment.BlogId);

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(userId);

            _unitOfWork
                .Setup(x => x.Comments.GetByIdAsync(comment.Id))
                .ReturnsAsync(comment);

            _unitOfWork
                .Setup(x => x.CompleteAsync(
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await _deleteHandler.Handle(
                command,
                CancellationToken.None);

            Assert.That(result, Is.EqualTo(1));

            _unitOfWork.Verify(
                x => x.Comments.GetByIdAsync(comment.Id),
                Times.Once);

            _unitOfWork.Verify(
                x => x.Comments.Delete(comment),
                Times.Once);

            _unitOfWork.Verify(
                x => x.CompleteAsync(
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        [Category("Comment")]
        public async Task EnsureDeleteCommentThrowsNotFoundWhenUserIsNull()
        {
            var command = new DeleteCommentCommand(
                Guid.NewGuid(),
                Guid.NewGuid());

            _currentUserService
                .Setup(x => x.UserId)
                .Returns((Guid?)null);

            await Assert.ThatAsync(
                async () => await _deleteHandler.Handle(
                    command,
                    CancellationToken.None),
                Throws.TypeOf<NotFoundException>());
        }

        [Test]
        [Category("Comment")]
        public async Task EnsureDeleteCommentThrowsNotFoundWhenUserIdIsEmpty()
        {
            var command = new DeleteCommentCommand(
                Guid.NewGuid(),
                Guid.NewGuid());

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(Guid.Empty);

            await Assert.ThatAsync(
                async () => await _deleteHandler.Handle(
                    command,
                    CancellationToken.None),
                Throws.TypeOf<NotFoundException>());
        }

        [Test]
        [Category("Comment")]
        public async Task EnsureDeleteCommentThrowsNotFoundWhenCommentDoesNotExist()
        {
            var userId = Guid.NewGuid();
            var commentId = Guid.NewGuid();

            var command = new DeleteCommentCommand(
                commentId,
                Guid.NewGuid());

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(userId);

            _unitOfWork
                .Setup(x => x.Comments.GetByIdAsync(commentId))
                .ReturnsAsync((Comment?)null);

            await Assert.ThatAsync(
                async () => await _deleteHandler.Handle(
                    command,
                    CancellationToken.None),
                Throws.TypeOf<NotFoundException>());
        }

        [Test]
        [Category("Comment")]
        public async Task EnsureDeleteCommentThrowsForbiddenWhenUserOwnsComment()
        {
            var userId = Guid.NewGuid();

            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                BlogId = Guid.NewGuid(),
                UserId = userId,
                CommentContent = "My comment"
            };

            var command = new DeleteCommentCommand(
                comment.Id,
                comment.BlogId);

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(userId);

            _unitOfWork
                .Setup(x => x.Comments.GetByIdAsync(comment.Id))
                .ReturnsAsync(comment);

            await Assert.ThatAsync(
                async () => await _deleteHandler.Handle(
                    command,
                    CancellationToken.None),
                Throws.TypeOf<ForbiddenException>());
        }

        // =========================================
        // Update Comment Handler
        // =========================================

        [Test]
        [Category("Comment")]
        public async Task EnsureUpdateCommentReturnsSuccess()
        {
            var userId = Guid.NewGuid();
            var commentOwnerId = Guid.NewGuid();

            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                BlogId = Guid.NewGuid(),
                UserId = commentOwnerId,
                CommentContent = "Old comment"
            };

            var command = new UpdateCommentCommand(
                comment.Id,
                "Updated comment",
                comment.BlogId);

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(userId);

            _unitOfWork
                .Setup(x => x.Comments.GetByIdAsync(comment.Id))
                .ReturnsAsync(comment);

            _unitOfWork
                .Setup(x => x.CompleteAsync(
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            var result = await _updateHandler.Handle(
                command,
                CancellationToken.None);

            Assert.That(result, Is.EqualTo(1));
            Assert.That(
                comment.CommentContent,
                Is.EqualTo("Updated comment"));

            Assert.That(
                comment.UpdatedAt,
                Is.Not.Null);

            _unitOfWork.Verify(
                x => x.Comments.Update(comment),
                Times.Once);

            _unitOfWork.Verify(
                x => x.CompleteAsync(
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Test]
        [Category("Comment")]
        public async Task EnsureUpdateCommentThrowsNotFoundWhenUserIsNull()
        {
            var command = new UpdateCommentCommand(
                Guid.NewGuid(),
                "Updated",
                Guid.NewGuid());

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
        [Category("Comment")]
        public async Task EnsureUpdateCommentThrowsNotFoundWhenUserIdIsEmpty()
        {
            var command = new UpdateCommentCommand(
                Guid.NewGuid(),
                "Updated",
                Guid.NewGuid());

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(Guid.Empty);

            await Assert.ThatAsync(
                async () => await _updateHandler.Handle(
                    command,
                    CancellationToken.None),
                Throws.TypeOf<NotFoundException>());
        }

        [Test]
        [Category("Comment")]
        public async Task EnsureUpdateCommentThrowsNotFoundWhenCommentDoesNotExist()
        {
            var userId = Guid.NewGuid();
            var commentId = Guid.NewGuid();

            var command = new UpdateCommentCommand(
                commentId,
                "Updated",
                Guid.NewGuid());

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(userId);

            _unitOfWork
                .Setup(x => x.Comments.GetByIdAsync(commentId))
                .ReturnsAsync((Comment?)null);

            await Assert.ThatAsync(
                async () => await _updateHandler.Handle(
                    command,
                    CancellationToken.None),
                Throws.TypeOf<NotFoundException>());
        }

        [Test]
        [Category("Comment")]
        public async Task EnsureUpdateCommentThrowsForbiddenWhenUserOwnsComment()
        {
            var userId = Guid.NewGuid();

            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                BlogId = Guid.NewGuid(),
                UserId = userId,
                CommentContent = "My comment"
            };

            var command = new UpdateCommentCommand(
                comment.Id,
                "Updated",
                comment.BlogId);

            _currentUserService
                .Setup(x => x.UserId)
                .Returns(userId);

            _unitOfWork
                .Setup(x => x.Comments.GetByIdAsync(comment.Id))
                .ReturnsAsync(comment);

            await Assert.ThatAsync(
                async () => await _updateHandler.Handle(
                    command,
                    CancellationToken.None),
                Throws.TypeOf<ForbiddenException>());
        }

        // =========================================
        // Get Comments Of Blog Handler
        // =========================================

        [Test]
        [Category("Comment")]
        public async Task EnsureGetCommentsOfBlogReturnsComments()
        {
            var blogId = Guid.NewGuid();

            var user1 = new User
            {
                Id = Guid.NewGuid(),
                FName = "Ahmed",
                LName = "Ali",
                Image = "ahmed.jpg"
            };

            var user2 = new User
            {
                Id = Guid.NewGuid(),
                FName = "Mohamed",
                LName = "Ali",
                Image = "mohamed.jpg"
            };

            var comments = new List<Comment>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    BlogId = blogId,
                    UserId = user1.Id,
                    User = user1,
                    CommentContent = "First comment",
                    ParentCommentId = null
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    BlogId = blogId,
                    UserId = user2.Id,
                    User = user2,
                    CommentContent = "Reply comment",
                    ParentCommentId = null
                }
            };

            _unitOfWork
                .Setup(x => x.Comments.FindAllAsync(
                    It.IsAny<System.Linq.Expressions.Expression<Func<Comment, bool>>>(),
                    It.IsAny<System.Linq.Expressions.Expression<Func<Comment, object>>[]>(),
                    It.IsAny<SortDirection>(),
                    It.IsAny<System.Linq.Expressions.Expression<Func<Comment, object>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(comments);

            var result = await _getAllHandler.Handle(
                new GetCommentsOfBlogQuery(blogId),
                CancellationToken.None);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));

            Assert.That(
                result[0].UserName,
                Is.EqualTo("Ahmed Ali"));

            Assert.That(
                result[0].CommentContent,
                Is.EqualTo("First comment"));

            Assert.That(
                result[0].UserImage,
                Is.EqualTo("ahmed.jpg"));
        }

        [Test]
        [Category("Comment")]
        public async Task EnsureGetCommentsOfBlogReturnsRepliesInsideParent()
        {
            var blogId = Guid.NewGuid();

            var user1 = new User
            {
                Id = Guid.NewGuid(),
                FName = "Ahmed",
                LName = "Ali"
            };

            var user2 = new User
            {
                Id = Guid.NewGuid(),
                FName = "Mohamed",
                LName = "Ali"
            };

            var parentId = Guid.NewGuid();

            var comments = new List<Comment>
            {
                new()
                {
                    Id = parentId,
                    BlogId = blogId,
                    UserId = user1.Id,
                    User = user1,
                    CommentContent = "Parent comment",
                    ParentCommentId = null
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    BlogId = blogId,
                    UserId = user2.Id,
                    User = user2,
                    CommentContent = "Reply comment",
                    ParentCommentId = parentId
                }
            };

            _unitOfWork
                .Setup(x => x.Comments.FindAllAsync(
                    It.IsAny<System.Linq.Expressions.Expression<Func<Comment, bool>>>(),
                    It.IsAny<System.Linq.Expressions.Expression<Func<Comment, object>>[]>(),
                    It.IsAny<SortDirection>(),
                    It.IsAny<System.Linq.Expressions.Expression<Func<Comment, object>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(comments);

            var result = await _getAllHandler.Handle(
                new GetCommentsOfBlogQuery(blogId),
                CancellationToken.None);

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].CommentContent, Is.EqualTo("Parent comment"));
            Assert.That(result[0].Replies.Count, Is.EqualTo(1));
            Assert.That(
                result[0].Replies[0].CommentContent,
                Is.EqualTo("Reply comment"));
        }

        [Test]
        [Category("Comment")]
        public async Task EnsureGetCommentsOfBlogThrowsNotFoundWhenCommentsAreNull()
        {
            var blogId = Guid.NewGuid();

            _unitOfWork
                .Setup(x => x.Comments.FindAllAsync(
                    It.IsAny<System.Linq.Expressions.Expression<Func<Comment, bool>>>(),
                    It.IsAny<System.Linq.Expressions.Expression<Func<Comment, object>>[]>(),
                    It.IsAny<SortDirection>(),
                    It.IsAny<System.Linq.Expressions.Expression<Func<Comment, object>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((IEnumerable<Comment>?)null);

            await Assert.ThatAsync(
                async () => await _getAllHandler.Handle(
                    new GetCommentsOfBlogQuery(blogId),
                    CancellationToken.None),
                Throws.TypeOf<NotFoundException>());
        }

        // =========================================
        // Get Comment By Id Handler
        // =========================================

        [Test]
        [Category("Comment")]
        public async Task EnsureGetCommentByIdReturnsComment()
        {
            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                BlogId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                CommentContent = "Test comment",
                ParentCommentId = Guid.NewGuid()
            };

            var query = new GetCommentByIdQuery(comment.Id);

            _unitOfWork
                .Setup(x => x.Comments.GetByIdAsync(comment.Id))
                .ReturnsAsync(comment);

            var result = await _getByIdHandler.Handle(
                query,
                CancellationToken.None);

            Assert.That(result, Is.Not.Null);

            Assert.That(
                result.CommentContent,
                Is.EqualTo(comment.CommentContent));

            Assert.That(
                result.BlogId,
                Is.EqualTo(comment.BlogId));

            Assert.That(
                result.UserId,
                Is.EqualTo(comment.UserId));

            Assert.That(
                result.ParentCommentId,
                Is.EqualTo(comment.ParentCommentId));
        }

        [Test]
        [Category("Comment")]
        public async Task EnsureGetCommentByIdThrowsNotFoundWhenCommentDoesNotExist()
        {
            var commentId = Guid.NewGuid();

            var query = new GetCommentByIdQuery(commentId);

            _unitOfWork
                .Setup(x => x.Comments.GetByIdAsync(commentId))
                .ReturnsAsync((Comment?)null);

            await Assert.ThatAsync(
                async () => await _getByIdHandler.Handle(
                    query,
                    CancellationToken.None),
                Throws.TypeOf<NotFoundException>());
        }
    }

}
