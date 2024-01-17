using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebAPI.DTO;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> UserManager;
        private readonly IConfiguration config;

        public AccountController(UserManager<ApplicationUser> userManager, IConfiguration config)
        {
            this.UserManager = userManager;
            this.config = config;

        }
        //Create Account Post New User "Registration" "Post"

        [HttpPost("register")] //api/account/register
        public async Task<IActionResult> Registration(RegisterUserDto userDto) 
        {
            if(ModelState.IsValid)
            {
                //save
                ApplicationUser user = new ApplicationUser();
                user.UserName = userDto.UserName;
                user.Email = userDto.Email;
                IdentityResult result = await UserManager.CreateAsync(user,userDto.Password);
                if (result.Succeeded)
                {
                    return Ok("Account Added Sucessfully");
                }
                foreach(var item in result.Errors)
                {
                    return BadRequest(item);
                }
                //return BadRequest(result.Errors.FirstOrDefault().ToString());

            }
            return BadRequest(ModelState);
               
        }


        //Check Account Valid "Login" "Post"
        [HttpPost("login")] //api/account/login
        public async Task<IActionResult> Login(LoginUserDto userDto)
        {
            if (ModelState.IsValid)
            {
               //check - create token
              ApplicationUser user = await UserManager.FindByNameAsync(userDto.UserName); 
                if (user != null) //username found 
                {
                  bool found = await UserManager.CheckPasswordAsync(user,userDto.Password);
                  if(found == true) //check the same password
                  {
                        //claim token
                        var claims = new List<Claim>();
                        claims.Add(new Claim(ClaimTypes.Name, user.UserName));
                        claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
                        claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));  
                        
                        //get roles
                        var role= await UserManager.GetRolesAsync(user);
                        foreach(var itemrole in role) {
                            claims.Add(new Claim(ClaimTypes.Role, itemrole));
                        
                        }

                        //security key
                        SecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Secret"]));

                        // signincreidts
                        SigningCredentials signincred = new SigningCredentials(
                            securityKey, SecurityAlgorithms.HmacSha256);
                        //
                        //create token
                        JwtSecurityToken mytoken = new JwtSecurityToken(
                            issuer: config["JWT:ValidIssuer"],
                            audience: config["JWT:ValidAudience"],
                            claims:claims,
                            expires: DateTime.Now.AddHours(1),
                            signingCredentials: signincred

                            );

                        return Ok(new
                        {
                            token= new JwtSecurityTokenHandler().WriteToken(mytoken),
                            expiration=mytoken.ValidTo
                        });


                  }
                  return Unauthorized();
                }
                return Unauthorized();
            }
            return Unauthorized();

        }
    }
}
