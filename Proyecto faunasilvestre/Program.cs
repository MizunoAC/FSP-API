using FluentAssertions.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Proyecto_faunasilvestre.Context;
using Proyecto_faunasilvestre.Servicios;
using Proyecto_faunasilvestre.Utilidades;
using System.Configuration;
using System.Net.Mail;
using System.Text;
using NLog.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

////////////////////////////////////////////////////////////////////////
///

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


///////////////////////////////////////////////////////////////////////
///






////////////////////////////////////////////////////////////////////////

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
        IssuerSigningKey = new SymmetricSecurityKey
        (Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true
    };
});



////////////////////////////////////////////////////////////////////////


builder.Services.AddDbContext<ContexDb>();
builder.Services.AddScoped<IUsuariosServicio, UsuariosServicio>();
builder.Services.AddScoped<IAnimalServicio, AnimalServicio>();
builder.Services.AddScoped<ILoginServicio, LoginServicio>();
builder.Services.AddScoped<IRecuperarcontra, Recuperarcontra>();


builder.Host.ConfigureLogging((hostingContext, logging) =>
{
    logging.AddNLog();
});


builder.Services.AddAutoMapper(typeof(Program));

////////////////////////////////////////////////////////////////////////


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
