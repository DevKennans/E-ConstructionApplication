using EConstructionApp.Domain.Entities.Identification;

namespace EConstructionApp.Application.Interfaces.Services.Identification
{
    public interface ITokenHandler
    {
        DTOs.Identification.Token CreateAccessToken(int seconds, AppUser user, IList<string> roles);
    }
}
