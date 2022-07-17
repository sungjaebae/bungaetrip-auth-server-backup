using AuthenticationServer.API.Data;
using AuthenticationServer.API.Models;
using AuthenticationServer.API.Services.Authenticators;
using AuthenticationServer.API.Services.PasswordHashers;
using AuthenticationServer.API.Services.RefreshTokenRepositories;
using AuthenticationServer.API.Services.RefreshValidators;
using AuthenticationServer.API.Services.TokenGenerators;
using AuthenticationServer.API.Services.UserRepositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Azure.Security.KeyVault.Secrets;
using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(
//    builder.Configuration.GetConnectionString("LocalDockerMySQL"),
//    new MySqlServerVersion(new Version(8, 0, 29))
//    ));
builder.Services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();
builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
builder.Services.AddSingleton<IRefreshTokenRepository, InMemoryRefreshTokenRepository>();
builder.Services.AddSingleton<AccessTokenGenerator>();
builder.Services.AddSingleton<RefreshTokenGenerator>();
builder.Services.AddSingleton<Authenticator>();
builder.Services.AddSingleton<RefreshTokenValidator>();

AuthenticationConfiguration authenticationConfiguration = new AuthenticationConfiguration();
builder.Configuration.Bind("Authentication", authenticationConfiguration);

SecretClient keyVaultClient = new SecretClient(new Uri(builder.Configuration.GetValue<string>("KeyVaultUri")), new DefaultAzureCredential());
authenticationConfiguration.AccessTokenSecret = keyVaultClient.GetSecret("access-token-secret").Value.Value;

builder.Services.AddSingleton(authenticationConfiguration);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters()
    {
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationConfiguration.AccessTokenSecret)),
        ValidIssuer = authenticationConfiguration.Audience,
        ValidAudience = authenticationConfiguration.Audience,
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ClockSkew = TimeSpan.FromMinutes(1)
    };
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme()
    {
        Description = "standard Authorization header using the Bearer scheme (\"bearer {token}\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type=SecuritySchemeType.ApiKey
    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
