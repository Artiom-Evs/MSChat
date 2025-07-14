using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using MSChat.PresenceAPI;
using MSChat.PresenceAPI.Hubs;
using MSChat.PresenceAPI.Services;
using MSChat.Shared.Auth.Handlers;
using MSChat.Shared.Auth.Requirements;
using MSChat.Shared.Configuration.Extensions;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// get validated app settings
var authSettings = builder.Configuration.GetAuthSettings();
var corsSettings = builder.Configuration.GetCorsSettings();
var redisSettings = builder.Configuration.GetRedisSettings();

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(redisSettings.ConnectionString));

builder.Services.
    AddAuthentication(options =>
    {
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.Authority = authSettings.Authority;
        options.Audience = AppConstants.Audience;
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();

        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidIssuer = authSettings.ValidIssuer,
            ValidAudience = AppConstants.Audience,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser()
        .AddRequirements(new ScopeRequirement(AppConstants.DefaultScope))
        .Build();
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins(corsSettings.AllowedOrigins.Split(","))
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    }); ;
});

builder.Services
    .AddSignalR()
    .AddStackExchangeRedis(redisSettings.ConnectionString);

builder.Services.AddSingleton<IAuthorizationHandler, ScopeHandler>();
builder.Services.AddScoped<IUserPresenceService, UserPresenceService>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World!");
app.MapHub<PresenceHub>("/_hubs/presence");

app.Run();
