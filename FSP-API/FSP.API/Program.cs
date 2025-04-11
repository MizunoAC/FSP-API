using FluentAssertions.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using FSP_API.Context;
using FSP_API.Servicios;
using FSP_API.Utilidades;
using System.Configuration;
using System.Net.Mail;
using System.Text;
using NLog.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using FSP.Infrastructure.Repository.Contracts;
using FSP.Application.command;
using FSP.Infrastructure.Repository;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy("nuevapolitica", app =>
    {
        app.WithOrigins("https://*.ngrok.io")
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

var connectionString = builder.Configuration.GetConnectionString("WebConnection");
var rsa = RSA.Create();
var dir = config["Jwt:PublicKeyPath"];
rsa.ImportFromPem(File.ReadAllText(dir));

var rsaSecurityKey = new RsaSecurityKey(rsa);

builder.Services.AddSingleton(new SqlConnection(connectionString));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<AddUserCommandHandler>());

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey =  rsaSecurityKey,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true
    };
});

builder.Services.AddDbContext<ContexDb>();
builder.Services.AddScoped<IUsuariosServicio, UsuariosServicio>();
builder.Services.AddScoped<IAnimalServicio, AnimalServicio>();
builder.Services.AddScoped<ILoginServicio, LoginServicio>();
builder.Services.AddScoped<IRecuperarcontra, Recuperarcontra>();
builder.Services.AddScoped<IUserRepository, UsersRepository>();
builder.Services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
builder.Services.AddScoped<IAnimalRepository, AnimalsRepository>();



builder.Host.ConfigureLogging((hostingContext, logging) =>
{
    logging.AddNLog();
});


builder.Services.AddAutoMapper(typeof(Program));
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

app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowAnyOrigin());

app.UseAuthentication();
app.UseAuthorization(); 


app.MapControllers();

app.Run();
