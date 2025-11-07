using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using userManagement.Application.Interfaces.Auth;
using userManagement.Application.Interfaces.Security;
using userManagement.Application.Interfaces.Students;
using userManagement.Application.Interfaces.Users;
using userManagement.Application.Services.Auth;
using userManagement.Application.Services.Security;
using userManagement.Application.Services.Students;
using userManagement.Application.Services.Users;
using userManagement.Domain.Interfaces;
using userManagement.Infrastructure.Persistence;
using userManagement.Infrastructure.Repository;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 2. Configuración de Base de Datos
// Asume que el connection string 'Default' está en appsettings.json
var certPath = Environment.GetEnvironmentVariable("MYSQL_CA_PATH");
var connectionString = Environment.GetEnvironmentVariable("MYSQL_CONNECTION_STRING");
if (!string.IsNullOrEmpty(certPath))
{
    connectionString = connectionString.Replace("${HOME}/Certs/ca.pem", certPath);
}
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 36)));
});


// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//Inyeccion de dependencias --------------------------------------------------------------
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddScoped<IStudentsRepository, StudentsRepository>();
builder.Services.AddScoped<IStudentService, StudentService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();


// builder.Services.AddScoped<IAuthService, AuthService>();

// Configuración de Autenticación y Autorización--------------------------------------------

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
