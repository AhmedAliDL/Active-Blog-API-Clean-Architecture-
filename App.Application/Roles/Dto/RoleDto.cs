using System.ComponentModel.DataAnnotations;

namespace App.Application.Roles.Dto
{
    public class RoleDto
    {
        [MaxLength(15, ErrorMessage = "Role name between 15 and 3")]
        [MinLength(3, ErrorMessage = "Role name between 15 and 3")]
        public string RoleName { get; set; } = null!;
        [MinLength(10, ErrorMessage = "Role description is maximum 10 character")]
        public string? RoleDescription { get; set; }
    }
}
