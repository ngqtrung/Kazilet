using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PRN231_Kazilet_API.Models.Entities;
using PRN231_Kazilet_API.Services;
using PRN231_Kazilet_API.Utils;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PRN231_Kazilet_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;
        private readonly EmailService _emailService;
        private readonly IAuthService _authService;
        private readonly Common utils = new Common();
        private readonly IWebHostEnvironment _env;

        public AuthenticationController(IConfiguration configuration, IUserService userService, IWebHostEnvironment env, IAuthService authService)
        {
            _configuration = configuration;
            _userService = userService;
            _emailService = new EmailService(configuration);
            _env = env;
            _authService = authService;
        }

        [HttpGet("check")]
        public async Task<IActionResult> CheckToken()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { isValid = false, message = "No token provided." });
            }

            try
            {
                User? u = _authService.GetUserFromJwtToken(token);
                if (u == null)
                {
                    return Unauthorized(new { isValid = false, message = "Not found user." });
                }
                else
                {
                    return Ok(new { UserId = u.Id, Username = u.Username, Email = u.Email });
                }
            }
            catch (Exception)
            {
                return Unauthorized(new { isValid = false, message = "Invalid token." });
            }
        }

        [HttpPost("change")]
        public async Task<IActionResult> ChangePassword([FromBody] string newpassword)
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { isValid = false, message = "No token provided." });
            }

            try
            {
                User? u = _authService.GetUserFromJwtToken(token);
                if (u == null)
                {
                    return Unauthorized(new { isValid = false, message = "Not found user." });
                }
                else
                {
                    bool rs = _userService.ChangePassword(u.Id, utils.HashPassword(newpassword));

                    if (!rs) return BadRequest();

                    var filePath = Path.Combine(_env.WebRootPath, "email_template", "changePwdSuccess.html");
                    var htmlContent = await System.IO.File.ReadAllTextAsync(filePath);
                    htmlContent = htmlContent.Replace("{Ent3r@Usernam3!Her3}", u.Username);
                    htmlContent = htmlContent.Replace("{Tim3C4an9ePa$$word}", DateTime.Now.ToString("HH:mm - dd/MM/yyyy"));

                    _emailService.SendEmailAsync(u.Email, "Kazilet", "Change password successfully", htmlContent);
                    return Ok();
                }
            }
            catch (Exception)
            {
                return Unauthorized(new { isValid = false, message = "Invalid token." });
            }
        }

        [HttpGet("forgot")]
        public async Task<IActionResult> CheckEmail(string email)
        {
            bool rs = _userService.UserExists(email).Result;
            if (rs)
            {
                User u = _userService.GetUserByEmail(email);

                if(u == null) return NotFound();

                string pwd = utils.GeneratePassword();
                _userService.ChangePassword(u.Id, utils.HashPassword(pwd));

                var filePath = Path.Combine(_env.WebRootPath, "email_template", "forgotPassword.html");
                var htmlContent = await System.IO.File.ReadAllTextAsync(filePath);
                htmlContent = htmlContent.Replace("{Ent3r@Usernam3!Her3}", u.Username);
                htmlContent = htmlContent.Replace("{new@Password!Here}", pwd);

                _emailService.SendEmailAsync(email, "Kazilet", "Forgot password", htmlContent);

                return Ok();

            }

            return NotFound();

        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            if (await _userService.UserExists(registerRequest.email))
                return BadRequest("Email already exists.");

            User u = new User
            {
                Username = registerRequest.username,
                Email = registerRequest.email,
                Password = registerRequest.password,
                Role = 1,
                Type = "email",
            };

            var result = _userService.Register(u);
            return result > 0 ? Ok(result) : BadRequest("Register failed.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var authenticatedUser = await _userService.Authenticate(loginRequest.email, loginRequest.password);
            if (authenticatedUser == null)
                return Unauthorized();
            var token = _authService.GenerateJwtToken(authenticatedUser);

            // Lưu token vào cookie
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddSeconds(int.Parse(_configuration["Jwt:ExpireSeconds"]))
            };

            Response.Cookies.Append("accessToken", token, cookieOptions);
            return Ok(new { Token = token });
        }

        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            var redirectUrl = Url.Action("GoogleCallback", "Authentication");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, "Google");
        }

        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback()
        {
            var result = await HttpContext.AuthenticateAsync("Google");
            if (result.Succeeded)
            {
                var googleId = result.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
                var email = result.Principal.FindFirstValue(ClaimTypes.Email);
                var username = result.Principal.FindFirstValue(ClaimTypes.Name);
                User? u = _userService.GetUserByEmail(email);
                int uid;
                if (u == null)
                {
                    string pwd = utils.GeneratePassword();
                    u = new User
                    {
                        Username = username,
                        Email = email,
                        Password = utils.HashPassword(pwd),
                        Gid = googleId,
                        Role = 1,
                        Type = "google"
                    };
                    uid = _userService.RegisterGoogle(u);
                    u = _userService.GetUser(uid);

                    var filePath = Path.Combine(_env.WebRootPath, "email_template", "createPassword.html");
                    var htmlContent = await System.IO.File.ReadAllTextAsync(filePath);
                    htmlContent = htmlContent.Replace("{Ent3r@Usernam3!Her3}", u.Username);
                    htmlContent = htmlContent.Replace("{new@Password!Here}", pwd);
                    _emailService.SendEmailAsync(email, "Kazilet", "Your account has been created successfully", htmlContent);
                }
                else
                {
                    uid = u.Id;
                    u.Type = "google";
                    if (String.IsNullOrEmpty(u.Gid))
                    {
                        u.Gid = googleId;
                    }
                    bool rs = _userService.UpdateUser(u);
                }
                var token = _authService.GenerateJwtToken(u);

                // Lưu token vào cookie
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = false,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTimeOffset.UtcNow.AddSeconds(int.Parse(_configuration["Jwt:ExpireSeconds"]))
                };

                Response.Cookies.Append("accessToken", token, cookieOptions);
                return Redirect($"https://localhost:7081/");
            }
            return Unauthorized();
        }
    }
}

public class LoginRequest
{
    public string email { get; set; }
    public string password { get; set; }
}

public class RegisterRequest
{
    public string username { get; set; }
    public string email { get; set; }
    public string password { get; set; }
}