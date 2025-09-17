using FSP.Application.command;
using FSP.Domain.Models.Wrapper;
using FSP.Infrastructure.Helpers;
using FSP.Infrastructure.Middleware;
using FSP.Infrastructure.Repository;
using FSP.Infrastructure.Repository.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NLog.Extensions.Logging;
using System.Net;
using System.Security.Cryptography;

internal class Program
{
    private static void Main(string[] args)
    {
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
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
            .Build();

        var connectionString = builder.Configuration.GetConnectionString("WebConnection");
        var rsa = RSA.Create();
        var dir = Path.Combine(AppContext.BaseDirectory, config["Jwt:PublicKeyPath"]);
        rsa.ImportFromPem(File.ReadAllText(dir));

        var rsaSecurityKey = new RsaSecurityKey(rsa);

        builder.Services.AddSingleton(new SqlConnection(connectionString));

        builder.Services.AddSingleton(new DbConnectionConfig
        {
            ConnectionString = connectionString
        });
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<AddUserCommandHandler>());
        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(AddRecordAnimalCommandHandler).Assembly);
        });

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
                IssuerSigningKey = rsaSecurityKey,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true
            };
        });
        builder.Services.AddScoped<IUserRepository, UsersRepository>();
        builder.Services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
        builder.Services.AddScoped<IAnimalRepository, AnimalsRepository>();
        builder.Services.AddScoped<IAdminRepository, AdminRepository>(); 
        builder.Services.AddScoped<IPdfGeneratorHelper, PdfGeneratorHelper>();

        builder.Logging.AddNLog();

        builder.Services.AddAutoMapper(typeof(Program));
        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseCors(x => x
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowAnyOrigin());

        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}