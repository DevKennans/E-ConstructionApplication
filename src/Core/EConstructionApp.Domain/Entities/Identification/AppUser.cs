using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace EConstructionApp.Domain.Entities.Identification
{
    public class AppUser : IdentityUser<string>
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        [NotMapped]
        public new bool TwoFactorEnabled { get; set; }
    }
}
