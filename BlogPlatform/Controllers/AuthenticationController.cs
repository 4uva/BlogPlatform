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
    [Route("api/auth/[action]")]
    [ApiController]
    public class AuthenticationController : ControllerCommon<AuthenticationController>
    {
        public AuthenticationController(
            BlogsDataBaseContext context,
            IMapper mapper,
            UserManager<User> userManager,
            ITokenService tokenService,
            ILogger<AuthenticationController> logger) : base(context, mapper, logger)
        {
            this.userManager = userManager;
            this.tokenService = tokenService;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserAuthenticationInfo info)
        {
            logger.LogInformation("User {user} logging in", info.UserName);
            var user = await userManager.FindByNameAsync(info.UserName);
            if (user == null)
            {
                logger.LogWarning("User {user} not found, login failed", info.UserName);
                return Unauthorized(); // no such user
            }

            var isPasswordValid = await userManager.CheckPasswordAsync(user, info.Password);
            if (!isPasswordValid)
            {
                logger.LogWarning("User {user} supplied wrong password, login failed", info.UserName);
                return Unauthorized();
            }

            // load blog too
            await context.Entry(user).Reference(u => u.Blog).LoadAsync();
            var token = tokenService.GenerateToken(user);
            
            logger.LogInformation("User {user} login succeeded", info.UserName);
            return Ok(token);
        }

        readonly UserManager<User> userManager;
        readonly ITokenService tokenService;
    }
}



