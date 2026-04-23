using ProjectBackend.api.Models.Domain;

namespace ProjectBackend.api.Services
{
    public interface ITokenService
    {
        string CreateToken(UserDomain user);
    }
}
