using App.Application.Common.Exceptions;
using App.Application.Common.Interfaces.Services;
using App.Application.Roles.Command.AssignRole;
using App.Application.Roles.Command.CreateRole;
using App.Application.Roles.Command.DeleteAssignRole;
using App.Application.Roles.Command.DeleteRole;
using App.Application.Roles.Queries.GetAllRoles;
using App.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace App.Test.Handlers
{
    [TestFixture]
    public class RoleHandlerTest
    {
        private Mock<IIdentityService> _identityService = null!;
        private Mock<IRoleService> _roleService = null!;

        private Mock<ILogger<AssignRoleHandler>> _assignLogger = null!;
        private Mock<ILogger<CreateRoleHandler>> _createLogger = null!;
        private Mock<ILogger<DeleteAssignRoleHandler>> _deleteAssignLogger = null!;
        private Mock<ILogger<DeleteRoleHandler>> _deleteLogger = null!;
        private Mock<ILogger<GetAllRolesHandler>> _getAllRolesLogger = null!;

        private AssignRoleHandler _assignHandler = null!;
        private CreateRoleHandler _createHandler = null!;
        private DeleteAssignRoleHandler _deleteAssignHandler = null!;
        private DeleteRoleHandler _deleteHandler = null!;
        private GetAllRolesHandler _getAllRolesHandler = null!;

        [SetUp]
        public void Setup()
        {
            _identityService = new Mock<IIdentityService>();
            _roleService = new Mock<IRoleService>();

            _assignLogger = new Mock<ILogger<AssignRoleHandler>>();
            _createLogger = new Mock<ILogger<CreateRoleHandler>>();
            _deleteAssignLogger = new Mock<ILogger<DeleteAssignRoleHandler>>();
            _deleteLogger = new Mock<ILogger<DeleteRoleHandler>>();
            _getAllRolesLogger = new Mock<ILogger<GetAllRolesHandler>>();

            _assignHandler = new AssignRoleHandler(
                _identityService.Object,
                _roleService.Object,
                _assignLogger.Object);

            _createHandler = new CreateRoleHandler(
                _roleService.Object,
                _createLogger.Object);

            _deleteAssignHandler = new DeleteAssignRoleHandler(
                _identityService.Object,
                _roleService.Object,
                _deleteAssignLogger.Object);

            _deleteHandler = new DeleteRoleHandler(
                _roleService.Object,
                _deleteLogger.Object);

            _getAllRolesHandler = new GetAllRolesHandler(
                _roleService.Object,
                _getAllRolesLogger.Object);
        }

        [Test]
        [Category("Role")]
        public async Task EnsureThatAssignRoleHandlerReturnsSuccessWhenValidRequest()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "user@gmail.com"
            };

            var command = new AssignRoleCommand
            {
                UserEmail = "user@gmail.com",
                RoleName = "ADMIN"
            };

            _identityService
                .Setup(x => x.GetUserByEmailAsync(command.UserEmail))
                .ReturnsAsync(user);

            _roleService
                .Setup(x => x.IsRoleExistsAsync("admin"))
                .ReturnsAsync(true);

            _roleService
                .Setup(x => x.AssignRoleToUserAsync(user, "admin"))
                .ReturnsAsync(IdentityResult.Success);

            var result = await _assignHandler.Handle(
                command,
                CancellationToken.None);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(command.RoleName, Is.EqualTo("admin"));
        }

        [Test]
        [Category("Role")]
        public async Task EnsureThatAssignRoleHandlerThrowsNotFoundExceptionWhenRoleDoesNotExist()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "user@gmail.com"
            };

            var command = new AssignRoleCommand
            {
                UserEmail = "user@gmail.com",
                RoleName = "admin"
            };

            _identityService
                .Setup(x => x.GetUserByEmailAsync(command.UserEmail))
                .ReturnsAsync(user);

            _roleService
                .Setup(x => x.IsRoleExistsAsync("admin"))
                .ReturnsAsync(false);

            await Assert.ThatAsync(
                async () => await _assignHandler.Handle(
                    command,
                    CancellationToken.None),
                Throws.TypeOf<NotFoundException>());
        }

        [Test]
        [Category("Role")]
        public async Task EnsureThatAssignRoleHandlerThrowsNotFoundExceptionWhenUserDoesNotExist()
        {
            var command = new AssignRoleCommand
            {
                UserEmail = "user@gmail.com",
                RoleName = "admin"
            };

            _identityService
                .Setup(x => x.GetUserByEmailAsync(command.UserEmail))
                .ReturnsAsync((User?)null);

            _roleService
                .Setup(x => x.IsRoleExistsAsync("admin"))
                .ReturnsAsync(true);

            await Assert.ThatAsync(
                async () => await _assignHandler.Handle(
                    command,
                    CancellationToken.None),
                Throws.TypeOf<NotFoundException>());
        }

        [Test]
        [Category("Role")]
        public async Task EnsureThatCreateRoleHandlerReturnsSuccessWhenValidRequest()
        {
            var command = new CreateRoleCommand
            {
                RoleName = "ADMIN",
                RoleDescription = "Administrator role"
            };

            _roleService
                .Setup(x => x.CreateRoleAsync(
                    "admin",
                    "Administrator role"))
                .ReturnsAsync(IdentityResult.Success);

            var result = await _createHandler.Handle(
                command,
                CancellationToken.None);

            Assert.That(result.Succeeded, Is.True);
            Assert.That(command.RoleName, Is.EqualTo("admin"));
        }

        [Test]
        [Category("Role")]
        public async Task EnsureThatCreateRoleHandlerReturnsFailedIdentityResultWhenRoleCreationFails()
        {
            var command = new CreateRoleCommand
            {
                RoleName = "admin",
                RoleDescription = "Administrator role"
            };

            var failedResult = IdentityResult.Failed(
                new IdentityError
                {
                    Description = "Role already exists."
                });

            _roleService
                .Setup(x => x.CreateRoleAsync(
                    "admin",
                    "Administrator role"))
                .ReturnsAsync(failedResult);

            var result = await _createHandler.Handle(
                command,
                CancellationToken.None);

            Assert.That(result.Succeeded, Is.False);
        }

        [Test]
        [Category("Role")]
        public async Task EnsureThatDeleteAssignRoleHandlerReturnsSuccessWhenValidRequest()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "user@gmail.com"
            };

            var command = new DeleteAssignRoleCommand
            {
                Email = "user@gmail.com",
                RoleName = "ADMIN"
            };

            _identityService
                .Setup(x => x.GetUserByEmailAsync(command.Email))
                .ReturnsAsync(user);

            _roleService
                .Setup(x => x.IsRoleExistsAsync("admin"))
                .ReturnsAsync(true);

            _roleService
                .Setup(x => x.DeleteAssignRoleAsync(user, "admin"))
                .ReturnsAsync(true);

            var result = await _deleteAssignHandler.Handle(
                command,
                CancellationToken.None);

            Assert.That(result, Is.True);
            Assert.That(command.RoleName, Is.EqualTo("admin"));
        }

        [Test]
        [Category("Role")]
        public async Task EnsureThatDeleteAssignRoleHandlerReturnsFalseWhenDeleteFails()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "user@gmail.com"
            };

            var command = new DeleteAssignRoleCommand
            {
                Email = "user@gmail.com",
                RoleName = "admin"
            };

            _identityService
                .Setup(x => x.GetUserByEmailAsync(command.Email))
                .ReturnsAsync(user);

            _roleService
                .Setup(x => x.IsRoleExistsAsync("admin"))
                .ReturnsAsync(true);

            _roleService
                .Setup(x => x.DeleteAssignRoleAsync(user, "admin"))
                .ReturnsAsync(false);

            var result = await _deleteAssignHandler.Handle(
                command,
                CancellationToken.None);

            Assert.That(result, Is.False);
        }

        [Test]
        [Category("Role")]
        public async Task EnsureThatDeleteAssignRoleHandlerThrowsNotFoundExceptionWhenUserDoesNotExist()
        {
            var command = new DeleteAssignRoleCommand
            {
                Email = "user@gmail.com",
                RoleName = "admin"
            };

            _identityService
                .Setup(x => x.GetUserByEmailAsync(command.Email))
                .ReturnsAsync((User?)null);

            await Assert.ThatAsync(
                async () => await _deleteAssignHandler.Handle(
                    command,
                    CancellationToken.None),
                Throws.TypeOf<NotFoundException>());
        }

        [Test]
        [Category("Role")]
        public async Task EnsureThatDeleteAssignRoleHandlerThrowsNotFoundExceptionWhenRoleDoesNotExist()
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "user@gmail.com"
            };

            var command = new DeleteAssignRoleCommand
            {
                Email = "user@gmail.com",
                RoleName = "admin"
            };

            _identityService
                .Setup(x => x.GetUserByEmailAsync(command.Email))
                .ReturnsAsync(user);

            _roleService
                .Setup(x => x.IsRoleExistsAsync("admin"))
                .ReturnsAsync(false);

            await Assert.ThatAsync(
                async () => await _deleteAssignHandler.Handle(
                    command,
                    CancellationToken.None),
                Throws.TypeOf<NotFoundException>());
        }

        [Test]
        [Category("Role")]
        public async Task EnsureThatDeleteRoleHandlerReturnsSuccessWhenValidRequest()
        {
            var role = new Role
            {
                Id = Guid.NewGuid(),
                Name = "admin"
            };

            var command = new DeleteRoleCommand
            {
                RoleName = "ADMIN"
            };

            _roleService
                .Setup(x => x.GetRoleAsync("admin"))
                .ReturnsAsync(role);

            _roleService
                .Setup(x => x.DeleteRoleAsync(role))
                .ReturnsAsync(true);

            var handler = (IRequestHandler<DeleteRoleCommand, bool>)_deleteHandler;

            var result = await handler.Handle(
                command,
                CancellationToken.None);

            Assert.That(result, Is.True);
            Assert.That(command.RoleName, Is.EqualTo("admin"));
        }

        [Test]
        [Category("Role")]
        public async Task EnsureThatDeleteRoleHandlerReturnsFalseWhenDeleteFails()
        {
            var role = new Role
            {
                Id = Guid.NewGuid(),
                Name = "admin"
            };

            var command = new DeleteRoleCommand
            {
                RoleName = "admin"
            };

            _roleService
                .Setup(x => x.GetRoleAsync("admin"))
                .ReturnsAsync(role);

            _roleService
                .Setup(x => x.DeleteRoleAsync(role))
                .ReturnsAsync(false);

            var handler = (IRequestHandler<DeleteRoleCommand, bool>)_deleteHandler;

            var result = await handler.Handle(
                command,
                CancellationToken.None);

            Assert.That(result, Is.False);
        }

        [Test]
        [Category("Role")]
        public async Task EnsureThatDeleteRoleHandlerThrowsNotFoundExceptionWhenRoleDoesNotExist()
        {
            var command = new DeleteRoleCommand
            {
                RoleName = "admin"
            };

            _roleService
                .Setup(x => x.GetRoleAsync("admin"))
                .ReturnsAsync((Role?)null);

            var handler = (IRequestHandler<DeleteRoleCommand, bool>)_deleteHandler;

            await Assert.ThatAsync(
                async () => await handler.Handle(
                    command,
                    CancellationToken.None),
                Throws.TypeOf<NotFoundException>());
        }

        [Test]
        [Category("Role")]
        public async Task EnsureThatGetAllRolesHandlerReturnsRolesWhenRolesExist()
        {
            var roles = new List<Role>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "admin",
                    NormalizedName = "ADMIN",
                    RoleDescription = "Administrator role"
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Name = "user",
                    NormalizedName = "USER",
                    RoleDescription = "User role"
                }
            };

            _roleService
                .Setup(x => x.GetAllRolesAsync())
                .ReturnsAsync(roles);

            var query = new GetAllRolesQuery();

            var result = await _getAllRolesHandler.Handle(
                query,
                CancellationToken.None);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].RoleName, Is.EqualTo("admin"));
            Assert.That(result[0].RoleDescription, Is.EqualTo("ADMIN"));
        }

        [Test]
        [Category("Role")]
        public async Task EnsureThatGetAllRolesHandlerReturnsEmptyListWhenNoRolesExist()
        {
            _roleService
                .Setup(x => x.GetAllRolesAsync())
                .ReturnsAsync([]);

            var query = new GetAllRolesQuery();

            var result = await _getAllRolesHandler.Handle(
                query,
                CancellationToken.None);

            Assert.That(result, Is.Not.Null.And.Empty);
        }
    }

}
