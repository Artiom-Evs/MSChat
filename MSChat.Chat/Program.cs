using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using MSChat.Chat;
using MSChat.Chat.Configurations;
using MSChat.Chat.Data;
using MSChat.Chat.Handlers;
using MSChat.Chat.Requirements;
using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("ChatDBConnection")
    ?? throw new InvalidOperationException("Connection string 'ApplicationDbContext' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// get validated API authentication settings
var authSettings = builder.Configuration
    .GetRequiredSection(ApiAuthSettings.Position)
    .Get<ApiAuthSettings>()!;
var validationContext = new ValidationContext(authSettings);
Validator.ValidateObject(authSettings!, validationContext, validateAllProperties: true);

builder.Services.
    AddAuthentication(options =>
    {
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.Authority = authSettings.Issuer;
        options.Audience = AppConstants.Audience;
        options.RequireHttpsMetadata = true;

        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = authSettings.Issuer,
            ValidAudience = AppConstants.Audience,
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
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    }); ;
});

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddSingleton<IAuthorizationHandler, ScopeHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
