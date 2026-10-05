using App.Application.Roles.Command.AssignRole;
using App.Application.Roles.Command.CreateRole;
using App.Application.Roles.Command.DeleteAssignRole;
using App.Application.Roles.Command.DeleteRole;
using FluentValidation.TestHelper;
using NUnit.Framework;

namespace App.Test.Validators
{
    [TestFixture]
    public class RoleValidatorTest
    {
        private AssignRoleValidator _assignValidator = null!;
        private CreateRoleValidator _createValidator = null!;
        private DeleteAssingRoleValidator _deleteAssignValidator = null!;
        private DeleteRoleValidator _deleteValidator = null!;

        [SetUp]
        public void Setup()
        {
            _assignValidator = new AssignRoleValidator();
            _createValidator = new CreateRoleValidator();
            _deleteAssignValidator = new DeleteAssingRoleValidator();
            _deleteValidator = new DeleteRoleValidator();
        }

        [Test]
        [Category("Role")]
        public async Task EnsureThatAssignRoleCommandIsValid()
        {
            var command = new AssignRoleCommand
            {
                UserEmail = "user@gmail.com",
                RoleName = "admin"
            };

            var result = await _assignValidator.TestValidateAsync(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        [Category("Role")]
        public async Task EnsureThatAssignRoleCommandIsInValidWhenEmailIsInvalid()
        {
            var command = new AssignRoleCommand
            {
                UserEmail = "invalid-email",
                RoleName = "admin"
            };

            var result = await _assignValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.UserEmail);
        }

        [Test]
        [Category("Role")]
        public async Task EnsureThatAssignRoleCommandIsInValidWhenEmailDomainIsNotAllowed()
        {
            var command = new AssignRoleCommand
            {
                UserEmail = "user@outlook.com",
                RoleName = "admin"
            };

            var result = await _assignValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.UserEmail);
        }

        [Test]
        [Category("Role")]
        public async Task EnsureThatAssignRoleCommandRoleNameWithMinimumLength()
        {
            var command = new AssignRoleCommand
            {
                UserEmail = "user@gmail.com",
                RoleName = "A"
            };

            var result = await _assignValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.RoleName);
        }

        [Test]
        [Category("Role")]
        public async Task EnsureThatAssignRoleCommandRoleNameWithMaximumLength()
        {
            var command = new AssignRoleCommand
            {
                UserEmail = "user@gmail.com",
                RoleName = new string('A', 21)
            };

            var result = await _assignValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.RoleName);
        }

        [Test]
        [Category("Role")]
        public async Task EnsureThatCreateRoleCommandIsValid()
        {
            var command = new CreateRoleCommand
            {
                RoleName = "admin",
                RoleDescription = "Administrator role"
            };

            var result = await _createValidator.TestValidateAsync(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        [Category("Role")]
        public async Task EnsureThatCreateRoleCommandIsInValid()
        {
            var command = new CreateRoleCommand
            {
                RoleName = "A",
                RoleDescription = "Test"
            };

            var result = await _createValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrors();
        }

        [Test]
        [Category("Role")]
        public async Task EnsureCreateRoleCommandRoleNameWithMinimumLength()
        {
            var command = new CreateRoleCommand
            {
                RoleName = "A",
                RoleDescription = "Valid description"
            };

            var result = await _createValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.RoleName);
        }

        [Test]
        [Category("Role")]
        public async Task EnsureCreateRoleCommandRoleNameWithMaximumLength()
        {
            var command = new CreateRoleCommand
            {
                RoleName = new string('A', 21),
                RoleDescription = "Valid description"
            };

            var result = await _createValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.RoleName);
        }

        [Test]
        [Category("Role")]
        public async Task EnsureCreateRoleCommandRoleDescriptionWithMinimumLength()
        {
            var command = new CreateRoleCommand
            {
                RoleName = "admin",
                RoleDescription = "Test"
            };

            var result = await _createValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.RoleDescription);
        }

        [Test]
        [Category("Role")]
        public async Task EnsureCreateRoleCommandRoleDescriptionWithMaximumLength()
        {
            var command = new CreateRoleCommand
            {
                RoleName = "admin",
                RoleDescription = new string('A', 51)
            };

            var result = await _createValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.RoleDescription);
        }

        [Test]
        [Category("Role")]
        public async Task EnsureCreateRoleCommandRoleDescriptionCanBeNull()
        {
            var command = new CreateRoleCommand
            {
                RoleName = "admin",
                RoleDescription = null
            };

            var result = await _createValidator.TestValidateAsync(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        [Category("Role")]
        public async Task EnsureDeleteAssignRoleCommandIsValid()
        {
            var command = new DeleteAssignRoleCommand
            {
                Email = "user@gmail.com",
                RoleName = "admin"
            };

            var result = await _deleteAssignValidator.TestValidateAsync(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        [Category("Role")]
        public async Task EnsureDeleteAssignRoleCommandIsInValid()
        {
            var command = new DeleteAssignRoleCommand
            {
                Email = "",
                RoleName = ""
            };

            var result = await _deleteAssignValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrors();
        }

        [Test]
        [Category("Role")]
        public async Task EnsureDeleteAssignRoleCommandEmailIsInvalid()
        {
            var command = new DeleteAssignRoleCommand
            {
                Email = "invalid-email",
                RoleName = "admin"
            };

            var result = await _deleteAssignValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Test]
        [Category("Role")]
        public async Task EnsureDeleteAssignRoleCommandRoleNameIsRequired()
        {
            var command = new DeleteAssignRoleCommand
            {
                Email = "user@gmail.com",
                RoleName = ""
            };

            var result = await _deleteAssignValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.RoleName);
        }

        [Test]
        [Category("Role")]
        public async Task EnsureDeleteRoleCommandIsValid()
        {
            var command = new DeleteRoleCommand
            {
                RoleName = "admin"
            };

            var result = await _deleteValidator.TestValidateAsync(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Test]
        [Category("Role")]
        public async Task EnsureDeleteRoleCommandIsInValid()
        {
            var command = new DeleteRoleCommand
            {
                RoleName = ""
            };

            var result = await _deleteValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrors();
        }

        [Test]
        [Category("Role")]
        public async Task EnsureDeleteRoleCommandRoleNameIsRequired()
        {
            var command = new DeleteRoleCommand
            {
                RoleName = ""
            };

            var result = await _deleteValidator.TestValidateAsync(command);

            result.ShouldHaveValidationErrorFor(x => x.RoleName);
        }
    }

}
