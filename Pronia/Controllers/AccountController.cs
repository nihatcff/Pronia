using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pronia.Abstraction;
using Pronia.ViewModels.UserViewModels;
using System.Threading.Tasks;

namespace Pronia.Controllers
{
    public class AccountController(UserManager<AppUser> _userManager, SignInManager<AppUser> _signInManager/*, RoleManager<IdentityRole> _roleManager*/, IEmailService _emailService) : Controller
    {
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var existUser = await _userManager.FindByEmailAsync(vm.UserName);
            if (existUser is { })
            {
                ModelState.AddModelError("Username", "This username is already exists");
                return View(vm);
            }

            existUser = await _userManager.FindByEmailAsync(vm.EmailAddress);
            if (existUser is { })
            {
                ModelState.AddModelError(nameof(vm.EmailAddress), "This email already exists");
                return View();
            }

            AppUser newUser = new()
            {
                Fullname = vm.Firstname + " " + vm.Lastname,
                Email = vm.EmailAddress,
                UserName = vm.UserName,
            };

            var result = await _userManager.CreateAsync(newUser, vm.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View();
            }


            await SendConfirmationMailAsync(newUser);

            TempData["SuccessMessage"] = "Registrationdan ugurla kecdiniz zehmet olmasa emailinizi tesdiqleyin";

            return RedirectToAction("Login");
        }


        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var user = await _userManager.FindByEmailAsync(vm.EmailAddress);
            if (user is null)
            {
                ModelState.AddModelError("", "Email or password is incorrect!");
                return View(vm);
            }

            var loginResult = await _userManager.CheckPasswordAsync(user, vm.Password);
            if (!loginResult)
            {
                ModelState.AddModelError("", "Email or password is incorrect!");
                return View(vm);
            }

            if (!user.EmailConfirmed)
            {
                ModelState.AddModelError("", "Please confirm your email");
                await SendConfirmationMailAsync(user);
                return View(vm);
            }

            await _signInManager.SignInAsync(user, vm.IsRemember);

            if (!string.IsNullOrWhiteSpace(vm.ReturnUrl))
            {
                return Redirect(vm.ReturnUrl);
            }

            return RedirectToAction(nameof(Index), "Home");
        }

        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }


        private async Task SendConfirmationMailAsync(AppUser user)
        {
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            //string url = $"https://localhost:7123/Account/ConfirmEmail?token={token}&userId={user.Id}";

            var url = Url.Action("ConfirmEmail", "Account", new { token = token, userId = user.Id }, Request.Scheme);




            string emailBody = @$"<!DOCTYPE html>
<html lang=""en"">
  <head>
    <meta charset=""UTF-8"" />
    <title>Confirm Your Email</title>
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
  </head>
  <body style=""margin:0; padding:0; background-color:#f4f6f8; font-family:Arial, Helvetica, sans-serif;"">
    <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color:#f4f6f8; padding:20px 0;"">
      <tr>
        <td align=""center"">
          <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""max-width:600px; background-color:#ffffff; border-radius:6px; overflow:hidden;"">
            
            <!-- Header -->
            <tr>
              <td style=""padding:24px; text-align:center; background-color:#0d6efd; color:#ffffff;"">
                <h1 style=""margin:0; font-size:24px;"">Confirm Your Email</h1>
              </td>
            </tr>

            <!-- Body -->
            <tr>
              <td style=""padding:32px; color:#333333; font-size:16px; line-height:1.5;"">
                <p style=""margin-top:0;"">Hi {user.Fullname},</p>

                <p>
                  Thanks for signing up! Please confirm your email address by clicking the button below.
                </p>

                <p style=""text-align:center; margin:32px 0;"">
                  <a href=""{url}""
                     style=""background-color:#0d6efd; color:#ffffff; text-decoration:none; padding:14px 24px; border-radius:4px; display:inline-block; font-weight:bold;"">
                    Confirm Email
                  </a>
                </p>

                <p>
                  If you didn’t create an account, you can safely ignore this email.
                </p>

                <p style=""margin-bottom:0;"">
                  Thanks,<br />
                  <strong>Pronia</strong>
                </p>
              </td>
            </tr>

            <!-- Footer -->
            <tr>
              <td style=""padding:20px; text-align:center; font-size:12px; color:#888888; background-color:#f9fafb;"">
                <p style=""margin:0;"">
                  © 2026 Pronia. All rights reserved.
                </p>
              </td>
            </tr>

          </table>
        </td>
      </tr>
    </table>
  </body>
</html>
";

            await _emailService.SendEMailAsync(user.Email!, "Confirm your email", emailBody);
        }

        public async Task<IActionResult> ConfirmEmail(string token, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) { return NotFound(); }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return BadRequest();
            }

            await _signInManager.SignInAsync(user, false);

            return RedirectToAction("Index", "Home");
        }

        /*public async Task<IActionResult> CreateRoles()
        {
            await _roleManager.CreateAsync(new IdentityRole()
            {
                Name = "User"
            });

            await _roleManager.CreateAsync(new IdentityRole()
            {
                Name = "Admin"
            });
            await _roleManager.CreateAsync(new IdentityRole()
            {
                Name = "Moderator"
            });

            return Ok("Roles Created");
        }*/
    }
}
