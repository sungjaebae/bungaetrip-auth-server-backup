using AuthenticationServer.API.Data;
using AuthenticationServer.API.Models;
using AuthenticationServer.API.Services.Authenticators;
using AuthenticationServer.API.Services.RefreshTokenRepositories;
using AuthenticationServer.API.Services.RefreshValidators;
using AuthenticationServer.API.Services.TokenGenerators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Swashbuckle.AspNetCore.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Azure.Security.KeyVault.Secrets;
using Azure.Identity;
using AuthenticationServer.API.Entities;
using Microsoft.Extensions.DependencyInjection;
using AuthenticationServer.API.Services.PasswordHashers;
using AuthenticationServer.API.Services.MemberRepositories;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.HttpOverrides;
using System.Net;
using AuthenticationServer.API.Services;
using AuthenticationServer.API.Services.NicknameGenerators;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.OAuth;
using static AuthenticationServer.API.Models.AppSecrets;
using AuthenticationServer.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
IConfigurationSection appSettingsSection = Startup.SetupConfiguration(builder.Services);
AppSettings appSettings = new AppSettings();
appSettingsSection.Bind(appSettings);


IConfigurationSection randomNicknameConfiguration =
builder.Configuration.GetSection(nameof(RandomNicknameConfiguration));
builder.Services.Configure<RandomNicknameConfiguration>(randomNicknameConfiguration);

IConfigurationSection appSecretsSection = builder.Configuration.GetSection(nameof(AppSecrets));
builder.Services.Configure<AppSecrets>(appSecretsSection);
AppSecrets appSecrets = new AppSecrets();
appSecretsSection.Bind(appSecrets);
JwtConfigurations jwtConfiguration = appSecrets.JwtConfiguration;

var Configuration = builder.Configuration;
builder.Services.AddSingleton(jwtConfiguration);
builder.Services.AddSingleton(randomNicknameConfiguration);
builder.Services.AddIdentity<User, IdentityRole<int>>(o =>
{
    o.User.RequireUniqueEmail = false;
    o.Password.RequireDigit = false;
    o.Password.RequireNonAlphanumeric = false;
    o.Password.RequireUppercase = false;
    o.Password.RequiredLength = 0;
}).AddEntityFrameworkStores<AuthenticationDbContext>();

builder.Services.AddDbContext<AuthenticationDbContext>(options =>
{
    var connectionString = appSecrets.ConnectionStrings.DbConnectionString;
    options.UseMySql(connectionString, ServerVersion.Parse("8.0.29"));
});
builder.Services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();
builder.Services.AddScoped<IRefreshTokenRepository, DatabaseRefreshTokenRepository>();
builder.Services.AddScoped<IMemberRepository, DatabaseMemberRepository>();
builder.Services.AddSingleton<StringGenerator>();
builder.Services.AddSingleton<NicknameGenerator>();
builder.Services.AddSingleton<AccessTokenGenerator>();
builder.Services.AddSingleton<RefreshTokenGenerator>();
builder.Services.AddScoped<Authenticator>();
builder.Services.AddSingleton<RefreshTokenValidator>();
builder.Services.AddScoped<DbInitializer>();
builder.Services.AddHttpClient("kakao", client => { client.BaseAddress = new Uri("https://kapi.kakao.com/v2/user/me"); });
builder.Services.AddHttpClient("google", client => { client.BaseAddress = new Uri("https://oauth2.googleapis.com/tokeninfo"); });
builder.Services.AddHttpClient("apple", client => { client.BaseAddress = new Uri("https://appleid.apple.com/auth/keys"); });
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<KakaoBackchannelAccessTokenAuthenticator>();
builder.Services.AddScoped<GoogleBackchannelAccessTokenAuthenticator>();
builder.Services.AddScoped<AppleBackchannelAccessTokenAuthenticator>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters()
    {
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfiguration.AccessTokenSecret)),
        ValidIssuer = jwtConfiguration.Issuer,
        ValidAudience = jwtConfiguration.Audience,
        ValidateIssuerSigningKey = true,
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.FromMinutes(1),
    };
}).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
.AddGoogle(options =>
{
    var googleSecrets = appSecrets.GoogleSecrets;

    options.ClientId = googleSecrets.AuthClientId;
    options.ClientSecret = googleSecrets.AuthClientSecret;
    options.CallbackPath = "/auth/signin-google";
    //this function is get user google profile image
    options.Scope.Add("profile");
    options.SignInScheme = IdentityConstants.ExternalScheme;
})
.AddKakaoTalk(options =>
{
    var kakaoSecrets = appSecrets.KakaoSecrets;
    options.ClientId = kakaoSecrets.AuthClientId;
    options.ClientSecret = kakaoSecrets.AuthClientSecret;
    options.CallbackPath = "/auth/signin-kakao";
    options.Scope.Add("account_email");
    options.SignInScheme = IdentityConstants.ExternalScheme;
}).AddApple(options =>
{
    var appleSecrets = appSecrets.AppleSecrets;
    options.ClientId = appleSecrets.AuthClientId;
    options.TeamId = appleSecrets.AuthTeamId;
    options.KeyId = appleSecrets.keyID;
    options.UsePrivateKey((keyId) =>
                builder.Environment.ContentRootFileProvider.GetFileInfo($"AuthKey_{keyId}.p8"));
    options.CallbackPath = "/auth/signin-apple";
    options.SignInScheme = IdentityConstants.ExternalScheme;
});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.KnownProxies.Add(Dns.GetHostEntry("nginx").AddressList[0]);
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme()
    {
        Description = "standard Authorization header using the Bearer scheme (\"bearer {token}\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
                      {
                          policy.WithOrigins("https://*.gogetter.kr", "http://*.gogetter.kr", "https://gogetter.kr", "http://gogetter.kr").AllowAnyHeader();
                      });
});
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});
var app = builder.Build();

app.UseHttpLogging();

using (IServiceScope scope = app.Services.CreateScope())
{
    using (AuthenticationDbContext context = scope.ServiceProvider.GetRequiredService<AuthenticationDbContext>())
    {
        context.Database.Migrate();
    }
}

app.UseForwardedHeaders();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(c => {
        c.RouteTemplate = "auth/swagger/{documentname}/swagger.json";
    });

    app.UseSwaggerUI(c => {
        c.SwaggerEndpoint("/auth/swagger/v1/swagger.json", "My Cool API V1");
        c.RoutePrefix = "auth/swagger";
    });
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
