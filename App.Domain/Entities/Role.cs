using Microsoft.AspNetCore.Identity;

namespace App.Domain.Entities
{
    public class Role : IdentityRole<Guid>
    {
        public string? RoleDescription { get; set; }
    }
}
