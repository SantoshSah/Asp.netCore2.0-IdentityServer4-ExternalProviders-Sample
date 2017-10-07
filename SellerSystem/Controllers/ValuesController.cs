using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
//using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.Models;
using IdentityModel.Client;
using SellerSystem.BLL.Service.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SellerSystem.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        IAccountService _accountService;
        public ValuesController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        // GET: api/values
        [HttpGet]
        //[Authorize]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "company")]
        public IEnumerable<string> Get()
        {
            var secret = new Secret("secret".Sha256());
            //_accountService.Create();
            return new string[] { "value1", "value2" };
            //var role = (new System.Collections.Generic<System.Security.Claims.Claim>(((System.Security.Claims.ClaimsIdentity)User.Identity).Claims).Items[9]).Value;
            //var sch = (((System.Security.Claims.ClaimsIdentity)User.Identity).Claims).[9]
        }

        // GET api/values/5
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {

        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
