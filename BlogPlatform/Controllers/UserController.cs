using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using BlogPlatform.Models;
using BlogPlatform.Models.EF_Model_classes;
using AutoMapper;

namespace BlogPlatform.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerCommon<UserController>
    {
        public UserController(
            BlogsDataBaseContext context,
            IMapper mapper,
            UserManager<User> userManager,
            ILogger<UserController> logger) : base(context, mapper, logger)
        {
            this.userManager = userManager;
        }

        [HttpPost]
        public async Task<ActionResult<UserExtendedInfo>> Register([FromBody] UserRegistrationInfo info)
        {
            logger.LogInformation("Registering new user {user}", info.UserName);
            var user = new User
            {
                UserName = info.UserName,
                Blog = new Blog()
            };
            var createResult = await userManager.CreateAsync(user, info.Password);
            if (!createResult.Succeeded)
            {
                var descriptions = createResult.Errors.Select(error => error.Description).ToList();
                logger.LogWarning("Registering user {user} failed: {errors}", info.UserName, descriptions);
                return Conflict(descriptions);
            }

            logger.LogInformation("Registraton succeeded, user {user}", info.UserName);
            logger.LogInformation("User {user} obtains blog {blog}", info.UserName, user.Blog.BlogId);

            var extUser = mapper.Map<UserExtendedInfo>(user);
            return extUser;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<UserExtendedInfo>> Get()
        {
            logger.LogInformation("Getting info for user user {user}", User?.Identity?.Name);
            var user = await UserHelper.GetUserAsync(User, userManager);
            if (user == null)
            {
                logger.LogWarning("Couldn't find user {user} in the database, why", User?.Identity?.Name);
                return NotFound();
            }

            var extUser = mapper.Map<UserExtendedInfo>(user);
            // blog id must be extracted because GetUserAsync doesn't load properties
            extUser.BlogId = GetBlogId();
            return extUser;
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> Unregister()
        {
            logger.LogInformation("Unregistering current user {user}", User?.Identity?.Name);
            var user = await UserHelper.GetUserAsync(User, userManager);
            if (user == null)
            {
                logger.LogWarning("Couldn't find user {user} in the database, unregistration failed", User?.Identity?.Name);
                return NoContent();
            }

            logger.LogInformation("Deleting user {user} from database", user.UserName);
            // TODO: delete the blog as well!
            var deleteResult = await userManager.DeleteAsync(user);
            if (!deleteResult.Succeeded)
            {
                var descriptions = deleteResult.Errors.Select(error => error.Description).ToList();
                logger.LogError("Couldn't delete user {user} from the database, errors: {errors}", user?.UserName, descriptions);
                throw new InvalidOperationException(descriptions.FirstOrDefault() ?? "unknown error");
            }
            else
            {
                logger.LogInformation("Deleting user {user} succeeded", user.UserName);
                return NoContent();
            }
        }

        readonly UserManager<User> userManager;
    }
}



