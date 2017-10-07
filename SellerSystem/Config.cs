using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using SellerSystem.Helpers;

namespace SellerSystem
{
    public static class Config
    {
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("allowedscopeapi", "Security.AuthServer.Api", new[] { JwtClaimTypes.Name, JwtClaimTypes.Role, "office" })
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),

            };
        }

        public static IEnumerable<Client> GetClients()
        {
            Collection<String> allowedGoogleGrantTypes = new Collection<string>();
            allowedGoogleGrantTypes.Add("googleAuth");

			Collection<String> allowedFacebookGrantTypes = new Collection<string>();
			allowedFacebookGrantTypes.Add("facebookAuth");

            return new List<Client>
            {
                // Hybrid Flow = OpenId Connect + OAuth
                // To use both Identity and Access Tokens
                //new Client
                //{
                //    ClientId = "neptrox_auth_client",
                //    ClientName = "Neptrox.Security.AuthServer.Client",
                //    //ClientSecrets = { new Secret("secret".Sha256()) },
                //    //ClientSecrets = { new Secret("adfabdsfsdf".Sha256()) },
                //    RequireClientSecret = false,

                //    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                //    AllowOfflineAccess = false,
                //    RequireConsent = false,

                //    RedirectUris = { "http://localhost:44391/signin-oidc" },
                //    PostLogoutRedirectUris = { "http://localhost:44391/signout-callback-oidc" },

                //    AllowedScopes =
                //    {
                //        IdentityServerConstants.StandardScopes.OpenId,
                //        IdentityServerConstants.StandardScopes.Profile,
                //        //"fiver_auth_api",
                //        "allowedscopeapi",
                //        IdentityServerConstants.StandardScopes.Email
                //    },
                //},
                // Resource Owner Password Flow
                new Client
                {
                    ClientId = "API_CLIENT",
                    ClientName = "API_CLIENT.Security.AuthServer.Client.Name",
                    ClientSecrets = { new Secret("secret#123".Sha256()) },
                    AccessTokenLifetime = 60 * 60 * 24,
                    RequireClientSecret = true,
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    AllowedScopes =
                    {
						IdentityServerConstants.StandardScopes.OpenId,
						IdentityServerConstants.StandardScopes.Profile,
						IdentityServerConstants.StandardScopes.Email,
                        "allowedscopeapi",
                    },
                },
				new Client
    			{
    				ClientId = "GOOGLE_CLIENT",
                    ClientName = "GOOGLE_CLIENT.Security.AuthServer.Client.Name",
    				ClientSecrets =
    				{
    					new Secret("secret#123".Sha256())
    				},
                    RequireClientSecret = true,
					AllowedGrantTypes = allowedGoogleGrantTypes,
    				AllowedScopes =
    				{
    				   "allowedscopeapi"
    				}
    			},
				new Client
				{
					ClientId = "FACEBOOK_CLIENT",
					ClientName = "FACEBOOK_CLIENT.Security.AuthServer.Client.Name",
					ClientSecrets =
					{
						new Secret("secret#123".Sha256())
					},
					RequireClientSecret = true,
					//AllowedGrantTypes = allowedGoogleGrantTypes,
                    AllowedGrantTypes = allowedFacebookGrantTypes,

                    AllowedScopes =
					{
					   "allowedscopeapi"
					}
				},
                // JavaScript Client
                new Client
                {
                	ClientId = "js.tokenmanager",
                	ClientName = "JavaScript Client",
                	AllowedGrantTypes = GrantTypes.Implicit,
                	AllowAccessTokensViaBrowser = true,

                	RedirectUris =           { "http://localhost:3000/home" },
                	PostLogoutRedirectUris = { "http://localhost:3000/home" },
                	//AllowedCorsOrigins =     { "http://localhost:5003" },
                    
                	AllowedScopes =
                	{
                		IdentityServerConstants.StandardScopes.OpenId,
                		IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                		"api1"
                	}
                }
            };
        }

        public static List<TestUser> GetUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "james",
                    Password = "password",
                    Claims = new List<Claim>
                    {
                        new Claim("name", "James Bond"),
                        new Claim("website", "https://james.com")
                    }
                },
                new TestUser
                {
                    SubjectId = "2",
                    Username = "spectre",
                    Password = "password",
                    Claims = new List<Claim>
                    {
                        new Claim("name", "Spectre"),
                        new Claim("website", "https://spectre.com")
                    }
                }
            };
        }
    }
}
