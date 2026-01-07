using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pronia.ViewModels.UserViewModels;
using System.Threading.Tasks;

namespace Pronia.Controllers
{
    public class AccountController(UserManager<AppUser> _userManager, SignInManager<AppUser> _signInManager,RoleManager<IdentityRole> _roleManager ) : Controller
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
                ModelState.AddModelError("Username","This username is already exists");
                return View(vm);
            }

            existUser = await _userManager.FindByEmailAsync(vm.EmailAddress);
            if (existUser is { })
            {
                ModelState.AddModelError(nameof(vm.EmailAddress),"This email already exists");
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

            await _signInManager.SignInAsync(newUser,false);

            return RedirectToAction(nameof(Index),"Home");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM vm)
        {
            if(!ModelState.IsValid)
            {
                return View(vm);
            }

            var user = await _userManager.FindByEmailAsync(vm.EmailAddress);
            if(user is null)
            {
                ModelState.AddModelError("","Email or password is incorrect!");
                return View(vm);
            }

            var loginResult = await _userManager.CheckPasswordAsync(user, vm.Password);
            if (!loginResult)
            {
                ModelState.AddModelError("", "Email or password is incorrect!");
                return View(vm);
            }

            await _signInManager.SignInAsync(user, vm.IsRemember);

            return RedirectToAction(nameof(Index), "Home");
        }

        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
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
