using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Vehicles_API.ViewModels.Authorization;

namespace Vehicles_API.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AuthController(IConfiguration config, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
            _signInManager = signInManager;
            _userManager = userManager;
            _config = config;
        }

        [HttpPost("create-role")]
        public async Task<IActionResult> CreateRole([FromQuery] string roleName)
        {
            if (await _roleManager.RoleExistsAsync(roleName))
                return BadRequest($"The role \"{roleName}\" already exists in the database");

            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("Role creation", error.Description);
                }

                return BadRequest(ModelState);
            }

            return StatusCode(201);
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserViewModel>> RegisterUser(RegisterUserViewModel model)
        {
            if (await _userManager.FindByNameAsync(model.Email) is not null)
                return BadRequest("There is already a user with name in the database");

            var user = new IdentityUser
            {
                Email = model.Email!.ToLower(),
                UserName = model.Email.ToLower()
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await AddClaims(model, user);

                var userData = new UserViewModel
                {
                    UserName = user.UserName,
                    Expires = DateTime.Now.AddDays(1),
                    Token = await CreateJwtToken(user)
                };

                return StatusCode(201, userData);
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("User registration", error.Description);
                }

                return StatusCode(500, ModelState);
            }
        }

        private async Task AddClaims(RegisterUserViewModel model, IdentityUser user)
        {
            if (model.IsAdmin)
            {
                await _userManager.AddClaimAsync(user, new Claim("Admin", "true"));
                await _userManager.AddToRoleAsync(user, "Administrator");
            }

            await _userManager.AddClaimAsync(user, new Claim("User", "true"));
            await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Name, user.UserName));
            await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Email, user.Email));
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserViewModel>> Login(LoginViewModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);

            if (user is null)
                return Unauthorized("Invalid username");

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

            if (!result.Succeeded)
                return Unauthorized("Incorrect password");

            var userData = new UserViewModel
            {
                UserName = user.UserName,
                Expires = DateTime.Now.AddDays(1), // Should not be hard coded, can be fetched from somewhere where length of token is controlled
                Token = await CreateJwtToken(user)
            };

            return Ok(userData);
        }

        private async Task<string> CreateJwtToken(IdentityUser user)
        {
            var key = Encoding.ASCII.GetBytes(_config.GetValue<string>("apiKey"));

            var userClaims = (await _userManager.GetClaimsAsync(user)).ToList();
            var userRoles = await _userManager.GetRolesAsync(user);

            userClaims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));

            var jwt = new JwtSecurityToken(
                claims: userClaims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha512Signature
                )
            );

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}