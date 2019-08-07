using BlogPlatform.Models.EF_Model_classes;
using Microsoft.AspNetCore.Identity;

namespace BlogPlatform.Models
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}

  
