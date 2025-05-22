using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NLog.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using FSP.Infrastructure.Repository.Contracts;
using FSP.Application.command;
using FSP.Infrastructure.Repository;
using System.Security.Cryptography;
using FSP.Domain.Models.Wrapper;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;

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
            .Build();

        var connectionString = builder.Configuration.GetConnectionString("WebConnection");
        var rsa = RSA.Create();
        var dir = config["Jwt:PublicKeyPath"];
        rsa.ImportFromPem(File.ReadAllText(dir));

        var rsaSecurityKey = new RsaSecurityKey(rsa);

        builder.Services.AddSingleton(new SqlConnection(connectionString));

        builder.Services.AddSingleton(new DbConnectionConfig
        {
            ConnectionString = connectionString
        });
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

        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
                context.Response.ContentType = "application/json";

                if (exception is HttpRequestException httpEx && httpEx.StatusCode == HttpStatusCode.Unauthorized)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        status = 401,
                        error = httpEx.Message
                    });
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        status = 500,
                        error = "Unexpected error"
                    });
                }
            });
        });
        app.MapControllers();

        app.Run();
    }
}