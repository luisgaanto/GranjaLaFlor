using DB_GranjaLaFlor.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using DB_GranjaLaFlor.Services;
using DB_GranjaLaFlor.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace DB_GranjaLaFlor.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserService _userService;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserService userService,
            IPasswordHasher<User> passwordHasher,
            ILogger<AccountController> logger)
        {
            _userService = userService;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }


        private async Task SignInUserAsync(User user)
        {
            /*
             * Creates the user's authentication claims.
             * Claims store basic user information inside the authentication cookie.
             */
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.UserEmail),
                new Claim(ClaimTypes.Role, user.Role?.RoleName ?? string.Empty)
            };

            /*
             * Creates the user's identity using the Cookie Authentication scheme.
             * This identity will be stored inside the authentication cookie.
             */
            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            /*
             * Defines authentication cookie behavior.
             * The authentication cookie is valid only while the browser session is active.
             * Closing the browser automatically ends the user's session.
            */
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = false
            };

            /*
             * Signs in the user by creating the authentication cookie.
             * After this point, ASP.NET Core recognizes the user as authenticated.
             */
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }



        [HttpGet]
        public IActionResult Login()
        {
            _logger.LogInformation(
                "Entering AccountController.Login() GET.");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            _logger.LogInformation(
                "Entering AccountController.Login() POST. UserEmail: {UserEmail}",
                model.UserEmail);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning(
                    "AccountController.Login() POST validation failed. UserEmail: {UserEmail}",
                    model.UserEmail);

                return View(model);
            }

            var user = await _userService.GetActiveByEmailAsync(model.UserEmail);

            if (user == null)
            {
                _logger.LogWarning(
                    "Login failed. Active user/email not found. UserEmail: {UserEmail}",
                    model.UserEmail);

                ModelState.AddModelError(
                    string.Empty,
                    "Correo electrónico o contraseña incorrectos.");

                return View(model);
            }

            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(
                user,
                user.UserPassword,
                model.UserPassword);

            if (passwordVerificationResult == PasswordVerificationResult.Failed)
            {
                _logger.LogWarning(
                    "Login failed. Invalid password. UserEmail: {UserEmail}",
                    model.UserEmail);

                ModelState.AddModelError(
                    string.Empty,
                    "Correo electrónico o contraseña incorrectos.");

                return View(model);
            }

            /*
             * SignInUserAsync() creates the authentication cookie using the user's Claims.
             * Once authenticated, ASP.NET Core recognizes the user in future requests
             * until Logout or cookie expiration.
             */
            await SignInUserAsync(user);

            _logger.LogInformation(
                "User logged in successfully. UserId: {UserId}, UserEmail: {UserEmail}",
                user.UserId,
                user.UserEmail);

            TempData["SuccessMessage"] = "Inicio de sesión exitoso.";
            return RedirectToAction(
                "Index",
                "Dashboard");
        }

        
        /*
         * Signs out the current user by removing the authentication cookie.
         * After Logout, ASP.NET Core no longer recognizes the user as authenticated.
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            _logger.LogInformation(
                "Entering AccountController.Logout().");

            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            _logger.LogInformation(
                "User logged out successfully.");

            TempData["SuccessMessage"] = "La sesión fue cerrada correctamente.";

            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            _logger.LogWarning(
                "Access denied. User: {UserName}",
                User.Identity?.Name);

            return View();
        }


    }
}