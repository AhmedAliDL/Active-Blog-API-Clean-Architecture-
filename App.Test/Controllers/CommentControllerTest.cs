using Active_Blog_Service_API.Controllers;
using App.Application.Comments.Command.CreateComment;
using App.Application.Comments.Command.DeleteComment;
using App.Application.Comments.Command.UpdateComment;
using App.Application.Comments.Dto;
using App.Application.Comments.HttpRequests;
using App.Application.Comments.Queries.GetAllCommentOfBlog;
using App.Application.Comments.Queries.GetCommentById;
using App.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace App.Test.Controllers
{
    [TestFixture]
        public class CommentControllerTest
        {
            private Mock<IMediator> _mediator = null!;
            private CommentController _controller = null!;

            [SetUp]
            public void Setup()
            {
                _mediator = new Mock<IMediator>();
                _controller = new CommentController(_mediator.Object);
            }

            // =========================================
            // Get Comments Of Blog
            // =========================================

            [Test]
            [Category("Comment")]
            public async Task EnsureIndexReturnsOkResultWhenCommentsExist()
            {
                var blogId = Guid.NewGuid();

                var comments = new List<CommentDto>
            {
                new()
                {
                    UserName = "Ahmed Ali",
                    CommentContent = "Great article"
                }
            };

                _mediator
                    .Setup(x => x.Send(
                        It.Is<GetCommentsOfBlogQuery>(
                            q => q.BlogId == blogId),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(comments);

                var result = await _controller.Index(
                    blogId,
                    CancellationToken.None);

                Assert.That(
                    result,
                    Is.TypeOf<OkObjectResult>());

                var okResult = (OkObjectResult)result;

                Assert.That(
                    okResult.Value,
                    Is.EqualTo(comments));
            }

            [Test]
            [Category("Comment")]
            public async Task EnsureIndexReturnsNoContentWhenCommentsAreEmpty()
            {
                var blogId = Guid.NewGuid();

                _mediator
                    .Setup(x => x.Send(
                        It.Is<GetCommentsOfBlogQuery>(
                            q => q.BlogId == blogId),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync([]);

                var result = await _controller.Index(
                    blogId,
                    CancellationToken.None);

                Assert.That(
                    result,
                    Is.TypeOf<NoContentResult>());
            }

            [Test]
            [Category("Comment")]
            public async Task EnsureIndexReturnsNoContentWhenCommentsAreNull()
            {
                var blogId = Guid.NewGuid();

                _mediator
                    .Setup(x => x.Send(
                        It.Is<GetCommentsOfBlogQuery>(
                            q => q.BlogId == blogId),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync((List<CommentDto>?)null!);

                var result = await _controller.Index(
                    blogId,
                    CancellationToken.None);

                Assert.That(
                    result,
                    Is.TypeOf<NoContentResult>());
            }

            // =========================================
            // Get Comment By Id
            // =========================================

            [Test]
            [Category("Comment")]
            public async Task EnsureGetCommentByIdReturnsOkResultWhenCommentExists()
            {
                var commentId = Guid.NewGuid();

                var comment = new GetCommentDto
                {
                    BlogId = Guid.NewGuid(),
                    UserId = Guid.NewGuid(),
                    CommentContent = "Great article"
                };

                _mediator
                    .Setup(x => x.Send(
                        It.Is<GetCommentByIdQuery>(
                            q => q.CommentId == commentId),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(comment);

                var result = await _controller.GetCommentById(
                    commentId,
                    CancellationToken.None);

                Assert.That(
                    result,
                    Is.TypeOf<OkObjectResult>());

                var okResult = (OkObjectResult)result;

                Assert.That(
                    okResult.Value,
                    Is.EqualTo(comment));
            }

            [Test]
            [Category("Comment")]
            public async Task EnsureGetCommentByIdReturnsNoContentWhenCommentIsNull()
            {
                var commentId = Guid.NewGuid();

                _mediator
                    .Setup(x => x.Send(
                        It.Is<GetCommentByIdQuery>(
                            q => q.CommentId == commentId),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync((GetCommentDto?)null!);

                var result = await _controller.GetCommentById(
                    commentId,
                    CancellationToken.None);

                Assert.That(
                    result,
                    Is.TypeOf<NoContentResult>());
            }

            // =========================================
            // Add Comment
            // =========================================

            [Test]
            [Category("Comment")]
            public async Task EnsureAddCommentReturnsOkResultWhenCommentIsAdded()
            {
                var blogId = Guid.NewGuid();

                var request = new CreateCommentRequest(
                    "Great article!",
                    null);

                _mediator
                    .Setup(x => x.Send(
                        It.Is<CreateCommentCommand>(c =>
                            c.BlogId == blogId &&
                            c.CommentContent == request.CommentContent &&
                            c.ParentCommentId == request.ParentCommentId),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(1);

                var result = await _controller.AddComment(
                    blogId,
                    request,
                    CancellationToken.None);

                Assert.That(
                    result,
                    Is.TypeOf<OkObjectResult>());

                var okResult = (OkObjectResult)result;

                Assert.That(
                    okResult.Value,
                    Is.EqualTo("Comment successfuly added."));
            }

            [Test]
            [Category("Comment")]
            public async Task EnsureAddCommentReturnsBadRequestWhenCommentCannotBeAdded()
            {
                var blogId = Guid.NewGuid();

                var request = new CreateCommentRequest(
                    "Great article!",
                    null);

                _mediator
                    .Setup(x => x.Send(
                        It.IsAny<CreateCommentCommand>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(0);

                var result = await _controller.AddComment(
                    blogId,
                    request,
                    CancellationToken.None);

                Assert.That(
                    result,
                    Is.TypeOf<BadRequestObjectResult>());

                var badRequest = (BadRequestObjectResult)result;

                Assert.That(
                    badRequest.Value,
                    Is.EqualTo(
                        "Can`t add comment right now try again later."));
            }

            // =========================================
            // Edit Comment
            // =========================================

            [Test]
            [Category("Comment")]
            public async Task EnsureEditCommentReturnsOkResultWhenCommentIsEdited()
            {
                var commentId = Guid.NewGuid();
                var blogId = Guid.NewGuid();

                var request = new UpdateCommentRequest(
                    "Updated comment",
                    blogId);

                _mediator
                    .Setup(x => x.Send(
                        It.Is<UpdateCommentCommand>(c =>
                            c.CommentId == commentId &&
                            c.CommentContent == request.CommentContent &&
                            c.BlogId == request.BlogId),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(1);

                var result = await _controller.EditComment(
                    commentId,
                    request,
                    CancellationToken.None);

                Assert.That(
                    result,
                    Is.TypeOf<OkObjectResult>());

                var okResult = (OkObjectResult)result;

                Assert.That(
                    okResult.Value,
                    Is.EqualTo("Comment successfuly edited."));
            }

            [Test]
            [Category("Comment")]
            public async Task EnsureEditCommentReturnsBadRequestWhenCommentCannotBeEdited()
            {
                var commentId = Guid.NewGuid();

                var request = new UpdateCommentRequest(
                    "Updated comment",
                    Guid.NewGuid());

                _mediator
                    .Setup(x => x.Send(
                        It.IsAny<UpdateCommentCommand>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(0);

                var result = await _controller.EditComment(
                    commentId,
                    request,
                    CancellationToken.None);

                Assert.That(
                    result,
                    Is.TypeOf<BadRequestObjectResult>());

                var badRequest = (BadRequestObjectResult)result;

                Assert.That(
                    badRequest.Value,
                    Is.EqualTo(
                        "Can`t edit comment right now try again later."));
            }

            // =========================================
            // Delete Comment
            // =========================================

            [Test]
            [Category("Comment")]
            public async Task EnsureDeleteCommentReturnsOkResultWhenCommentIsDeleted()
            {
                var commentId = Guid.NewGuid();
                var blogId = Guid.NewGuid();

                var request = new DeleteCommentRequest(blogId);

                _mediator
                    .Setup(x => x.Send(
                        It.Is<DeleteCommentCommand>(c =>
                            c.CommentId == commentId &&
                            c.BlogId == blogId),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(1);

                var result = await _controller.DeleteComment(
                    commentId,
                    request,
                    CancellationToken.None);

                Assert.That(
                    result,
                    Is.TypeOf<OkObjectResult>());

                var okResult = (OkObjectResult)result;

                Assert.That(
                    okResult.Value,
                    Is.EqualTo("Comment successfuly deleted."));
            }

            [Test]
            [Category("Comment")]
            public async Task EnsureDeleteCommentReturnsBadRequestWhenCommentCannotBeDeleted()
            {
                var commentId = Guid.NewGuid();

                var request = new DeleteCommentRequest(
                    Guid.NewGuid());

                _mediator
                    .Setup(x => x.Send(
                        It.IsAny<DeleteCommentCommand>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(0);

                var result = await _controller.DeleteComment(
                    commentId,
                    request,
                    CancellationToken.None);

                Assert.That(
                    result,
                    Is.TypeOf<BadRequestObjectResult>());

                var badRequest = (BadRequestObjectResult)result;

                Assert.That(
                    badRequest.Value,
                    Is.EqualTo(
                        "Can`t delete comment right now try again later."));
            }
        }
    
}

