using AuthenticationServer.API.Data;
using AuthenticationServer.API.Models;
using AuthenticationServer.API.Services.PasswordHashers;
using AuthenticationServer.API.Services.TokenGenerators;
using AuthenticationServer.API.Services.UserRepositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(
//    builder.Configuration.GetConnectionString("LocalDockerMySQL"),
//    new MySqlServerVersion(new Version(8, 0, 29))
//    ));
builder.Services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();
builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
builder.Services.AddSingleton<AccessTokenGenerator>();



AuthenticationConfiguration authenticationConfiguration = new AuthenticationConfiguration();
builder.Configuration.Bind("Authentication", authenticationConfiguration);
builder.Services.AddSingleton(authenticationConfiguration);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
