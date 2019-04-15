using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingAPI.Data;
using DatingAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DatingAPI.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthRepository _authRepository;
        private readonly JwtKey _options;
        private readonly IDatingRepository _datingRepository;

        // DI: inject AuthRepository
        public AuthController(IAuthRepository authRepository, IOptions<JwtKey> options, IDatingRepository datingRepository)
        {
            _authRepository = authRepository;
            _options = options.Value;
            _datingRepository = datingRepository;
        }



        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel registerViewModel)
        {
            

            // validate request ==> check if the validation is passed/not (modelState)
            if (!ModelState.IsValid) return new BadRequestObjectResult((new Error() { ErrorMessage = "username and password is required"}));


            var username = registerViewModel.Username.ToLower();
            var password = registerViewModel.Password;

            // check if user already exist 
            var exists = await _authRepository.UserExists(username);

            if (exists) { return new BadRequestObjectResult(new Error() { ErrorMessage = "username already exists"}); }

            // user to create
            var userToRegister = new User() { Username = username };
            var createdUser = await _authRepository.Register(userToRegister, password);
        
            return new OkObjectResult(new { CreatedUser =  createdUser.Username });
        }



        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] userLogInViewModel userLogInViewModel)
        {
            var username = userLogInViewModel.username.ToLower();
            var password = userLogInViewModel.password;

            var loginUser = await _authRepository.Login(username, password);

            if (loginUser == null) return new UnauthorizedResult();

            // build  token
            var tokenString = await GenerateToken(loginUser);

            return Ok(new { tokenString });
        }


        private async Task<string> GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = _options.SignKey;
            var signKey = Encoding.ASCII.GetBytes(_options.SignKey); // encode key as Byte[]

            // get token payload:
            var tokenPayload = await GenerateTokenPayload(user, signKey);

            // create token:
            var token =  tokenHandler.CreateToken(tokenPayload);

            // write token out
            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }

        private async Task<SecurityTokenDescriptor> GenerateTokenPayload(User user, Byte[] signKey)
        {
            // get user's main photoURL
            var currentuser = await _datingRepository.GetUser(user.Id);

            var userMainPhotoURL = currentuser.Photos.Where(x => x.IsMain).FirstOrDefault().Url;
            // generate token payload (body)
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                                                 {
                                                   new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                                                   new Claim(ClaimTypes.Name, user.Username),
                                                   new Claim("MainPhotoURL", userMainPhotoURL)
                                                     // we can add issuer and audience 
                                                 }),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key: signKey), SecurityAlgorithms.HmacSha512Signature)


            };

            return tokenDescriptor;
        }
    }
}
