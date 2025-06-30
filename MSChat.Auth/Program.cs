using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MSChat.Auth.Data;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("AuthDBConnection") ?? throw new InvalidOperationException("Connection string 'AuthDBConnection' not found.");;

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString);
    options.UseOpenIddict();
});

builder.Services.AddOpenIddict()
    .AddCore(options =>
    {
        options
            .UseEntityFrameworkCore()
            .UseDbContext<ApplicationDbContext>();
    })
    .AddServer(options =>
    {
        options.SetTokenEndpointUris("connect/token");
        options.AllowClientCredentialsFlow();
        options
            .AddDevelopmentEncryptionCertificate()
            .AddDevelopmentSigningCertificate();
        options
            .UseAspNetCore()
            .EnableTokenEndpointPassthrough();
    });

var app = builder.Build();

app.UseDeveloperExceptionPage();

app.UseForwardedHeaders();

app.UseRouting();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapDefaultControllerRoute();

app.Run();
