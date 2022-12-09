using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace FileStorage.Tests.ApiTests
{
    public class FakePolicyEvaluator : IPolicyEvaluator
    {
        public virtual async Task<AuthenticateResult> AuthenticateAsync(AuthorizationPolicy policy, HttpContext context)
        {
            var principal = new ClaimsPrincipal();
            principal.AddIdentity(new ClaimsIdentity(new[] {
            new Claim("Permission", "CanViewPage"),
            new Claim("Manager", "yes"),
            new Claim(ClaimTypes.Role, "Administrator"),
            new Claim(ClaimTypes.NameIdentifier, "John")
        }, "FakeScheme"));
            return await Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(principal,
                new AuthenticationProperties(), "FakeScheme")));
        }

        public virtual async Task<PolicyAuthorizationResult> AuthorizeAsync(AuthorizationPolicy policy,
            AuthenticateResult authenticationResult, HttpContext context, object resource)
        {
            return await Task.FromResult(PolicyAuthorizationResult.Success());
        }
    }
}
