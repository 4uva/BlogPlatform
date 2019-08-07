using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BlogPlatform.Models.EF_Model_classes
{
    public class User : IdentityUser
    {
        public Blog Blog { get; set; }
    }
}
