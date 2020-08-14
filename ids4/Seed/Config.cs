using IdentityServer4.Models;
using ids4.Model.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ids4.Seed
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
                   new IdentityResource[]
                   {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Phone(),
                   };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("amsapi"),
            };
        public static IEnumerable<ApiResource> GetApis()
        {
            return new List<ApiResource>
            {
                 new ApiResource("amsapi")
                {
                    Scopes = { "amsapi" },
                }
            };
        }
        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                // m2m client credentials flow client
                new Client
                {
                    ClientId = "m2m.client",
                    ClientName = "Client Credentials Client",

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

                    AllowedScopes = { "scope1" }
                },

                // interactive client using code flow + pkce
                new Client
                {
                    ClientId = "interactive",
                    ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Code,

                    RedirectUris = { "https://localhost:44300/signin-oidc" },
                    FrontChannelLogoutUri = "https://localhost:44300/signout-oidc",
                    PostLogoutRedirectUris = { "https://localhost:44300/signout-callback-oidc" },

                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "scope2" }
                },

                new Client
                {
                    ClientId = "ids4_swagger",
                    ClientName = "ams api",
                    RedirectUris = { "http://localhost:6001/swagger/oauth2-redirect.html" },
                    //PostLogoutRedirectUris = { "https://notused" },

                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowedScopes = { "openid", "profile", "email","amsapi" },
                    AllowAccessTokensViaBrowser = true
                },
                new Client
                {
                    ClientId = "js_oidc",
                    ClientName = "javascript client",
                    RedirectUris = 
                    {
                        "http://localhost:44300/callback.html",
                        "http://localhost:44300/silent.html"
                    },
                    PostLogoutRedirectUris = 
                    {
                        "http://localhost:44300/index.html"
                    },
                    RequireConsent = true,
                    AccessTokenLifetime = 60 * 5,
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowedScopes = { "openid", "profile", "email","amsapi" },
                    AllowAccessTokensViaBrowser = true
                },
            };
    }
}
