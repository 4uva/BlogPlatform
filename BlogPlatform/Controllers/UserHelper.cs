using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using BlogPlatform.Models.EF_Model_classes;

namespace BlogPlatform.Controllers
{
    static class UserHelper
    {
        public static async Task<User> GetUserAsync(
            ClaimsPrincipal principal, UserManager<User> userManager)
        {
            // userManager.GetUserAsync(User) doesn't work:
            // https://stackoverflow.com/q/51119926/10243782
            var userName = principal.Identity.Name;
            return await userManager.FindByNameAsync(userName);
        }
    }
}
