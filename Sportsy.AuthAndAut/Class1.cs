using OAuth2.Client.Impl;
using OAuth2.Infrastructure;
using System;

namespace Sportsy.AuthAndAut
{
    public class Class1
    {
        public string GoogleLogin()
        {
            var redirectUri = new Uri(Url.Action("GoogleLoginCallBack", "Account", null, protocol: Request.Url.Scheme));
            var googleClient = new GoogleClient(new RequestFactory(), new OAuth2.Configuration.ClientConfiguration
            {
                ClientId = auth.ClientId?.Trim(),
                ClientSecret = auth.ClientSecret?.Trim(),
                RedirectUri = redirectUrl,
                Scope = "profile email"
            });
            return googleClient.GetLoginLinkUri("SomeStateValueYouWantToUse");
        }
    }
}
