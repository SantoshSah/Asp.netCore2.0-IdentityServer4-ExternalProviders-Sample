using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using SellerSystem.Data;
using Microsoft.AspNetCore.Authorization;
using SellerSystem.Model.AccountViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SellerSystem.BLL.Service.Interface;
using SellerSystem.Entity;
using Newtonsoft.Json;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SellerSystem.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private IPasswordHasher<ApplicationUser> _passwordHasher;
        ApplicationDbContext _db;
        IAccountService _accountService;


        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager
            , IPasswordHasher<ApplicationUser> passwordHasher, IAccountService accountService, ApplicationDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _passwordHasher = passwordHasher;
            _accountService = accountService;
            _db = db;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var user = new ApplicationUser()
            {
                UserName = model.Email,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                Seller seller = new Seller()
                {
                    UserId = user.Id,
                    Name = model.Name,
                    Industry = model.Industry,
                    StoreWebAddress = model.StoreWebAddress
                };
                _accountService.Create(seller);
                return Ok(result);
            }

            return Ok(new { Error = result.Errors.First().Description });
        }

        [HttpPost]
        [Route("externalSignup")]
        public async Task<IActionResult> ExternalSignup([FromBody] ExternalSignupModel model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var user = new ApplicationUser()
            {
                UserName = model.Email,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };
            var result = await _userManager.CreateAsync(user);

            if (result.Succeeded)
            {
                try
                {
                    var userLogin = new IdentityUserLogin<int>()
                    {
                        LoginProvider = model.Provider,
                        ProviderKey = model.ProviderKey,
                        ProviderDisplayName = model.ProviderKey,
                        UserId = user.Id
                    };
                    _db.UserLogins.Add(userLogin);
                    _db.SaveChanges();
                }
                catch(Exception ex)
                {
                    throw ex;
                }
               
                Seller seller = new Seller()
                {
                    UserId = user.Id,
                    Name = model.Name,
                    Industry = model.Industry,
                    StoreWebAddress = model.StoreWebAddress
                };
                _accountService.Create(seller);

                return Ok(result);
            }
            return Ok(new { Error = result.Errors.First().Description });
        }

        [Route("getUserAccount")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetUserAccount()
        {
            var userName = User.Identity.Name;
            var roleClaim = User.Claims.Where(c => c.Type == "role").Select(c => c.Value).SingleOrDefault();
            return Ok(new { userName = userName, role = roleClaim });
        }
        
        [Route("hasEmailRegistered")]
        public async Task<IActionResult> HasEmailRegistered(string email, string provider, string providerKey)
        {
            bool hasRegistered;
            
            if(email == null)
            {
                var userLogin = await _userManager.FindByLoginAsync(provider, providerKey);
                hasRegistered = (userLogin != null);
            }
            else
            {
                var user = _userManager.Users.Where(a => a.Email == email).FirstOrDefault();
                hasRegistered = (user != null);
            }
            
            return Ok(new { hasRegistered = hasRegistered });
        }
    }
}
