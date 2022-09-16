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

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddIdentityCore<User>(o =>
{
    o.User.RequireUniqueEmail = true;
    o.Password.RequireDigit = false;
    o.Password.RequireNonAlphanumeric = false;
    o.Password.RequireUppercase = false;
    o.Password.RequiredLength = 0;
}).AddEntityFrameworkStores<AuthenticationDbContext>();

builder.Services.AddDbContext<AuthenticationDbContext>(options =>
    {
        options.UseMySql(builder.Configuration.GetConnectionString("DatabaseConnectionString"), ServerVersion.Parse("8.0.29"));
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

JwtConfiguration authenticationConfiguration = new JwtConfiguration();
builder.Configuration.Bind("JwtConfiguration", authenticationConfiguration);
RandomNicknameConfiguration randomNicknameConfiguration = new RandomNicknameConfiguration();
builder.Configuration.Bind("RandomNicknameconfiguration", randomNicknameConfiguration);

//SecretClient keyVaultClient = new SecretClient(new Uri(builder.Configuration.GetValue<string>("KeyVaultUri")), new DefaultAzureCredential());
//authenticationConfiguration.AccessTokenSecret = keyVaultClient.GetSecret("access-token-secret").Value.Value;


builder.Services.AddSingleton(authenticationConfiguration);
builder.Services.AddSingleton(randomNicknameConfiguration);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters()
    {
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationConfiguration.AccessTokenSecret)),
        ValidIssuer = authenticationConfiguration.Issuer,
        ValidAudience = authenticationConfiguration.Audience,
        ValidateIssuerSigningKey = true,
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.FromMinutes(1)
    };
});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.KnownProxies.Add(Dns.GetHostEntry("nginx").AddressList[0]);
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
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


var app = builder.Build();


using (IServiceScope scope = app.Services.CreateScope())
{
    using (AuthenticationDbContext context = scope.ServiceProvider.GetRequiredService<AuthenticationDbContext>())
    {
        if (app.Environment.IsDevelopment())
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            DbInitializer dbInitializer=scope.ServiceProvider.GetRequiredService<DbInitializer>();
            await dbInitializer.init();
        }
    }
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}

app.UseSwagger(c => {
    c.RouteTemplate = "auth/swagger/{documentname}/swagger.json";
});

app.UseSwaggerUI(c => {
    c.SwaggerEndpoint("/auth/swagger/v1/swagger.json", "My Cool API V1");
    c.RoutePrefix = "auth/swagger";
});
app.UseForwardedHeaders(new ForwardedHeadersOptions() { ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto});

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
