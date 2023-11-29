using System.Security.Claims;
using System.Security.Principal;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;


namespace BlazorApp1.Client.AuthProviders
{
    public class TestAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ISessionStorageService _sessionStorageService;
        private ClaimsPrincipal _principal;
        public TestAuthStateProvider(ISessionStorageService sessionStorageService)
        {
            _sessionStorageService = sessionStorageService;
        }
        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var Email = await _sessionStorageService.GetItemAsStringAsync("UserName");
            if (string.IsNullOrEmpty(Email))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, Email),
                new Claim(ClaimTypes.Email, Email),
            }, "apiauth_type");
            var user = new ClaimsPrincipal(identity);
            return new AuthenticationState(user);
        }
        public void MarkUserAsAuthenticated(string email)
        {
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, email),
            }, "Custom Authentication");
            var user = new ClaimsPrincipal(identity);
            _principal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Email, email) }, "Custom Authentication"));
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }
        public void MarkUserAsLoggedOut()
        {
            // Close session
            var identity = new ClaimsIdentity();
            var user = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }
    }
}
