using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

// ----------------------------------------------------------------------------
// Add services to the container
// ----------------------------------------------------------------------------
builder.Services.AddControllers();

// ✅ CORS sin restricciones (acepta cualquier origen, header y método)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()     // Permite cualquier dominio / frontend
            .AllowAnyMethod()     // Permite cualquier método HTTP: GET, POST, PUT, DELETE
            .AllowAnyHeader();    // Permite cualquier header, incluido Authorization
    });
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Conexion BD
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? Environment.GetEnvironmentVariable("MYSQL_CONNECTION_STRING")
    ?? "server=localhost;port=3306;database=usersdb;user=app;password=app";

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

// DI de servicios y repositorios
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IStudentService, StudentService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IStudentsRepository, StudentsRepository>();

// JWT - Authentication / Authorization
var jwtKey =
    builder.Configuration["Jwt:Key"] ??
    builder.Configuration["SecretKey"] ??
    Environment.GetEnvironmentVariable("SECRET_KEY");

var jwtIssuer =
    builder.Configuration["Jwt:Issuer"] ??
    builder.Configuration["Issuer"] ??
    Environment.GetEnvironmentVariable("ISSUER");

var jwtAudience =
    builder.Configuration["Jwt:Audience"] ??
    builder.Configuration["Audience"] ??
    Environment.GetEnvironmentVariable("AUDIENCE");

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException(
        "JWT Key is not configured. Set Jwt:Key or environment variable SECRET_KEY."
    );
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer           = !string.IsNullOrWhiteSpace(jwtIssuer),
            ValidIssuer              = jwtIssuer,
            ValidateAudience         = !string.IsNullOrWhiteSpace(jwtAudience),
            ValidAudience            = jwtAudience,
            ValidateLifetime         = true,
            ClockSkew                = TimeSpan.FromSeconds(30)
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// ----------------------------------------------------------------------------
// Configure the HTTP request pipeline
// ----------------------------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ✅ Activar CORS permitido para todo el pipeline (IMPORTANT!)
app.UseCors("AllowAll");


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
