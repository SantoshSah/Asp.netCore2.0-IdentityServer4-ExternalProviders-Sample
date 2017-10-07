using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityServer4.Validation;
using static IdentityModel.OidcConstants;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using SellerSystem.Data;

namespace SellerSystem.Helpers
{
    public class GoogleGrant : IExtensionGrantValidator
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public GoogleGrant(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public string GrantType
        {
            get
            {
                return "googleAuth";
            }
        }

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var userToken = context.Request.Raw.Get("id_token");

            if (string.IsNullOrEmpty(userToken))
            {
                context.Result = new GrantValidationResult(TokenErrors.InvalidGrant, null);
                return;
            }
            HttpClient client = new HttpClient();

            var request = client.GetAsync("https://www.googleapis.com/oauth2/v3/tokeninfo?id_token=" + userToken).Result;

            if (request.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var googleResult = await request.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<GoogleValidationResult>(googleResult);

                var user = _userManager.Users.Where(u => u.Email == result.email).FirstOrDefault();
                if (user != null)
                {
                    context.Result = new GrantValidationResult(_userManager.Users.Where(u => u.Email == result.email).FirstOrDefault().Id.ToString(), "google");
                }
             
                try
                {
                    context.Result = new GrantValidationResult(user.Id.ToString(), "google");
                }
                catch (Exception x)
                {
                    var a = x.Message;
                }

                return;
            }
            else
            {
                return;
            }
        }
    }

    public class GoogleValidationResult
    {
        public bool email_verified { get; set; }
        public string email { get; set; }
        public string given_name { get; set; }
        public string family_name { get; set; }
    }
}
