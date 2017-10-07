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
	public class FacebookGrant : IExtensionGrantValidator
	{

		public string GrantType
		{
			get
			{
				return "facebookAuth";
			}
		}


        private readonly UserManager<ApplicationUser> _userManager;

        public FacebookGrant(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
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
            string fields = "id,email,first_name,last_name,name";
            var graphApiUrl = "https://graph.facebook.com/v2.5/me?fields=" + fields;
            var request = client.GetAsync(graphApiUrl + "&access_token=" + userToken).Result;

            if (request.StatusCode == System.Net.HttpStatusCode.OK)
			{
				var facebookResult = await request.Content.ReadAsStringAsync();
				var result = JsonConvert.DeserializeObject<FacebookValidationResult>(facebookResult);
                int userId;
                if (result.email == null)
                {
                    var userLogin = await _userManager.FindByLoginAsync("facebook", result.id);
                    userId = userLogin.Id;
                }
                else
                {
                    var user = _userManager.Users.Where(a => a.Email == result.email).FirstOrDefault();
                    userId = user.Id;
                }

                try
				{
					context.Result = new GrantValidationResult(userId.ToString(), "facebook");
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

	public class FacebookValidationResult
	{
        public string id { get; set; }
		public string name { get; set; }
		public string email { get; set; }
		public string first_name { get; set; }
		public string last_name { get; set; }
	}
}

