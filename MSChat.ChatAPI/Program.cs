using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using MSChat.ChatAPI;
using MSChat.ChatAPI.Data;
using MSChat.ChatAPI.Hubs;
using MSChat.ChatAPI.Middlewares;
using MSChat.ChatAPI.Services;
using MSChat.ChatAPI.Services.Grpc;
using MSChat.Shared.Auth.Handlers;
using MSChat.Shared.Auth.Requirements;
using MSChat.Shared.Configuration.Extensions;
using NuGet.Protocol;
using StackExchange.Redis;
using System;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// get validated app settings
var authSettings = builder.Configuration.GetAuthSettings();
var corsSettings = builder.Configuration.GetCorsSettings();
var redisSettings = builder.Configuration.GetRedisSettings();
var rmqSettings = builder.Configuration.GetRabbitMQSettings();

builder.WebHost.ConfigureKestrelWithGrpc();

var connectionString = builder.Configuration.GetConnectionString("ChatDBConnection")
    ?? throw new InvalidOperationException("Connection string 'ApplicationDbContext' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(redisSettings.ConnectionString));

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rmqSettings.Host, "/", h =>
        {
            h.Username(rmqSettings.Username);
            h.Password(rmqSettings.Password);
        });
    });
});

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

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddGrpc();
builder.Services.AddMediatR(o => o.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddHostedService<DatabaseMigrationService>();
builder.Services.AddSingleton<IAuthorizationHandler, ScopeHandler>();
builder.Services.AddScoped<IChatsService, ChatsService>();
builder.Services.AddScoped<IMembersService, MembersService>();
builder.Services.AddScoped<IChatParticipantsService, ChatParticipantsService>();
builder.Services.AddScoped<IMessagesService, MessagesService>();
builder.Services.AddScoped<IChatMessageIdService, ChatMessageIdService>();
builder.Services.AddScoped<IMemberRegistrationService, MemberRegistrationService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapOpenApi();
    app.UseSwaggerUI(o => o.SwaggerEndpoint("/openapi/v1.json", "default"));
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<MemberRegistrationMiddleware>();

app.MapHub<ChatHub>("/_hubs/chat");
app.MapGrpcService<ChatAPIService>();
app.MapControllers();

app.Run();
