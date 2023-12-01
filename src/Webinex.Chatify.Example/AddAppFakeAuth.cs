using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Webinex.Chatify.Example;

public static class AddAppFakeAuth
{
    public static IServiceCollection AddFakeAuth(this IServiceCollection services)
    {
        services
            .AddAuthentication("FAKE")
            .AddScheme<FakeAuthenticationSchemeOptions, FakeAuthenticationSchemeHandler>(
                "FAKE",
                opts =>
                {
                }
            );

        services.AddAuthorization(x =>
        {
            x.DefaultPolicy = new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes("FAKE")
                .RequireAuthenticatedUser()
                .Build();
        });

        return services;
    }

    public class FakeAuthenticationSchemeOptions : AuthenticationSchemeOptions
    {
    }

    public class FakeAuthenticationSchemeHandler : AuthenticationHandler<FakeAuthenticationSchemeOptions>
    {
        public FakeAuthenticationSchemeHandler(
            IOptionsMonitor<FakeAuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var id = Request.Headers["X-USER-ID"].ToString();

            if (string.IsNullOrWhiteSpace(id))
            {
                id = Request.Query["access_token"].ToString();
            }

            if (string.IsNullOrWhiteSpace(id))
                return Task.FromResult(AuthenticateResult.Fail("No X-USER-ID provided"));

            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, id) };
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "FAKE"));
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}