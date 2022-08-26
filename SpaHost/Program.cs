using Duende.Bff.Yarp;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddBff()
    .AddRemoteApis();

// For recommendations see:
// https://docs.duendesoftware.com/identityserver/v6/bff/session/handlers/#the-openid-connect-authentication-handler
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = "cookie";
        options.DefaultChallengeScheme = "oidc";
        options.DefaultSignOutScheme = "oidc";
    })
    .AddCookie("cookie", options =>
    {
        // Session lifetime
        options.ExpireTimeSpan = TimeSpan.FromHours(8);

        // Sliding or absolute
        options.SlidingExpiration = false;

        // Host prefixed cookie name
        options.Cookie.Name = "__Unify-spa";

        // Strict SameSite handling
        options.Cookie.SameSite = SameSiteMode.Strict; // At least lax, strict preferred
    })
    .AddOpenIdConnect("oidc", options =>
    {
        options.Authority = "https://localhost:5001";

        options.ClientId = "bff";
        options.ClientSecret = "secret";
        options.ResponseType = "code";
        // Query response type is compatible with strict SameSite mode
        options.ResponseMode = "query";

        // Save tokens into authentication session
        // to enable automatic token management
        options.SaveTokens = true;

        // Get claims without mappings
        options.MapInboundClaims = false;
        options.GetClaimsFromUserInfoEndpoint = true;

        // Request scopes
        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("api");

        // Refresh token:
        options.Scope.Add("offline_access");
    });

var app = builder.Build();

if (app.Environment.IsProduction())
{
    app.UseDefaultFiles();
    app.UseStaticFiles();
}

app.UseRouting();
app.UseAuthentication();

// The BFF middleware must be placed before the authorization middleware, but after routing
app.UseBff();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapBffManagementEndpoints();

    endpoints.MapRemoteBffApiEndpoint("/api", "https://localhost:6001")
        .RequireAccessToken(Duende.Bff.TokenType.User);
});

if (app.Environment.IsDevelopment())
    app.UseSpa(spa =>spa.UseProxyToSpaDevelopmentServer("https://127.0.0.1:5173"));
else app.MapFallbackToFile("index.html");

app.Run();
