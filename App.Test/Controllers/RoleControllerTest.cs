using Active_Blog_Service_API.Controllers;
using App.Application.Roles.Command.AssignRole;
using App.Application.Roles.Command.CreateRole;
using App.Application.Roles.Command.DeleteAssignRole;
using App.Application.Roles.Command.DeleteRole;
using App.Application.Roles.Dto;
using App.Application.Roles.Queries.GetAllRoles;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace App.Test.Controllers
{
    [TestFixture]
        public class RoleControllerTest
        {
            private Mock<IMediator> _mediator = null!;
            private RoleController _controller = null!;

            [SetUp]
            public void Setup()
            {
                _mediator = new Mock<IMediator>();
                _controller = new RoleController(_mediator.Object);
            }

            [Test]
            [Category("Role")]
            public async Task EnsureThatIndexReturnsOkResultWhenRolesExist()
            {
                var roles = new List<RoleDto>
            {
                new()
                {
                    RoleName = "admin",
                    RoleDescription = "Administrator role"
                },
                new()
                {
                    RoleName = "user",
                    RoleDescription = "User role"
                }
            };

                _mediator
                    .Setup(x => x.Send(
                        It.IsAny<GetAllRolesQuery>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(roles);

                var result = await _controller.Index();

                Assert.That(result, Is.TypeOf<OkObjectResult>());
            }

            [Test]
            [Category("Role")]
            public async Task EnsureThatIndexReturnsNoContentWhenNoRolesExist()
            {
                _mediator
                    .Setup(x => x.Send(
                        It.IsAny<GetAllRolesQuery>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync([]);

                var result = await _controller.Index();

                Assert.That(result, Is.TypeOf<NoContentResult>());
            }

            [Test]
            [Category("Role")]
            public async Task EnsureThatIndexReturnsNoContentWhenRolesAreNull()
            {
                _mediator
                    .Setup(x => x.Send(
                        It.IsAny<GetAllRolesQuery>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync((List<RoleDto>?)null!);

                var result = await _controller.Index();

                Assert.That(result, Is.TypeOf<NoContentResult>());
            }

            [Test]
            [Category("Role")]
            public async Task EnsureThatAddRoleReturnsOkWhenRoleIsAdded()
            {
                var command = new CreateRoleCommand
                {
                    RoleName = "admin",
                    RoleDescription = "Administrator role"
                };

                _mediator
                    .Setup(x => x.Send(
                        It.IsAny<CreateRoleCommand>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(IdentityResult.Success);

                var result = await _controller.AddRole(
                    command,
                    CancellationToken.None);

                Assert.That(result, Is.TypeOf<OkObjectResult>());
            }

            [Test]
            [Category("Role")]
            public async Task EnsureThatAddRoleReturnsBadRequestWhenRoleIsNotAdded()
            {
                var command = new CreateRoleCommand
                {
                    RoleName = "admin",
                    RoleDescription = "Administrator role"
                };

                var identityResult = IdentityResult.Failed(
                    new IdentityError
                    {
                        Description = "Role already exists."
                    });

                _mediator
                    .Setup(x => x.Send(
                        It.IsAny<CreateRoleCommand>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(identityResult);

                var result = await _controller.AddRole(
                    command,
                    CancellationToken.None);

                Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
            }

            [Test]
            [Category("Role")]
            public async Task EnsureThatAssignRoleReturnsOkWhenRoleIsAssigned()
            {
                var command = new AssignRoleCommand
                {
                    UserEmail = "user@gmail.com",
                    RoleName = "admin"
                };

                _mediator
                    .Setup(x => x.Send(
                        It.IsAny<AssignRoleCommand>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(IdentityResult.Success);

                var result = await _controller.AssignRole(
                    command,
                    CancellationToken.None);

                Assert.That(result, Is.TypeOf<OkObjectResult>());
            }

            [Test]
            [Category("Role")]
            public async Task EnsureThatAssignRoleReturnsBadRequestWhenRoleIsNotAssigned()
            {
                var command = new AssignRoleCommand
                {
                    UserEmail = "user@gmail.com",
                    RoleName = "admin"
                };

                var identityResult = IdentityResult.Failed(
                    new IdentityError
                    {
                        Description = "Failed to assign role."
                    });

                _mediator
                    .Setup(x => x.Send(
                        It.IsAny<AssignRoleCommand>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(identityResult);

                var result = await _controller.AssignRole(
                    command,
                    CancellationToken.None);

                Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
            }

            [Test]
            [Category("Role")]
            public async Task EnsureThatDeleteRoleReturnsOkWhenRoleIsDeleted()
            {
                var command = new DeleteRoleCommand
                {
                    RoleName = "admin"
                };

                _mediator
                    .Setup(x => x.Send(
                        It.IsAny<DeleteRoleCommand>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(true);

                var result = await _controller.DeleteRole(
                    command,
                    CancellationToken.None);

                Assert.That(result, Is.TypeOf<OkObjectResult>());
            }

            [Test]
            [Category("Role")]
            public async Task EnsureThatDeleteRoleReturnsBadRequestWhenRoleIsNotDeleted()
            {
                var command = new DeleteRoleCommand
                {
                    RoleName = "admin"
                };

                _mediator
                    .Setup(x => x.Send(
                        It.IsAny<DeleteRoleCommand>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(false);

                var result = await _controller.DeleteRole(
                    command,
                    CancellationToken.None);

                Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
            }

            [Test]
            [Category("Role")]
            public async Task EnsureThatDeleteAssignRoleReturnsOkWhenRoleIsDeleted()
            {
                var command = new DeleteAssignRoleCommand
                {
                    Email = "user@gmail.com",
                    RoleName = "admin"
                };

                _mediator
                    .Setup(x => x.Send(
                        It.IsAny<DeleteAssignRoleCommand>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(true);

                var result = await _controller.DeleteAssignRole(
                    command,
                    CancellationToken.None);

                Assert.That(result, Is.TypeOf<OkObjectResult>());
            }

            [Test]
            [Category("Role")]
            public async Task EnsureThatDeleteAssignRoleReturnsBadRequestWhenRoleIsNotDeleted()
            {
                var command = new DeleteAssignRoleCommand
                {
                    Email = "user@gmail.com",
                    RoleName = "admin"
                };

                _mediator
                    .Setup(x => x.Send(
                        It.IsAny<DeleteAssignRoleCommand>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(false);

                var result = await _controller.DeleteAssignRole(
                    command,
                    CancellationToken.None);

                Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
            }
        }
    
}
