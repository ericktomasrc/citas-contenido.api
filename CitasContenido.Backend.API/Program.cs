using CitasContenido.Backend.Application.Behaviors;
using CitasContenido.Backend.Domain.Common;
using CitasContenido.Backend.Domain.Repositories;
using CitasContenido.Backend.Domain.Services;
using CitasContenido.Backend.Infraestructure.Common;
using CitasContenido.Backend.Infraestructure.Config;
using CitasContenido.Backend.Infraestructure.Persistence;
using CitasContenido.Backend.Infraestructure.Services;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS para el frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Database Configuration (SIN password - más simple)
var databaseConfig = new DatabaseConfig
{
    SqlServerConnection = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.")
};
builder.Services.AddSingleton(databaseConfig);

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey no configurada");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

// MediatR
builder.Services.AddMediatR(Assembly.Load("CitasContenido.Backend.Application"));

// AutoMapper
builder.Services.AddAutoMapper(Assembly.Load("CitasContenido.Backend.Application"));

// FluentValidation
builder.Services.AddValidatorsFromAssembly(Assembly.Load("CitasContenido.Backend.Application"));

// Pipeline Behaviors
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

// Servicios de Infraestructura
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

// Repositories 
builder.Services.AddScoped<ITipoDocumentoRepository, TipoDocumentoRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IVerificacionEmailRepository, VerificacionEmailRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IVerificacionIdentidadRepository, VerificacionIdentidadRepository>();
builder.Services.AddScoped<ITipoContenidoRepository, TipoContenidoRepository>();
builder.Services.AddScoped<IContenidoArchivoRepository, ContenidoArchivoRepository>();

// Services 
builder.Services.AddScoped<IVerificacionEmailDomainService, VerificacionEmailDomainService>();

// Domain Services
builder.Services.AddScoped<IVerificacionEmailDomainService, VerificacionEmailDomainService>();
builder.Services.AddScoped<ICompletarRegistroDomainService, CompletarRegistroDomainService>();
builder.Services.AddScoped<ILoginDomainService, LoginDomainService>();
builder.Services.AddScoped<IRefreshTokenDomainService, RefreshTokenDomainService>();
builder.Services.AddScoped<IVerificarIdentidadDomainService, VerificarIdentidadDomainService>();
// Domain Services
builder.Services.AddScoped<IRegistrarEmailDomainService, RegistrarEmailDomainService>();
builder.Services.AddScoped<IVerificacionEmailDomainService, VerificacionEmailDomainService>();
// ... otros servicios
builder.Services.AddScoped<IAzureStorageService, AzureStorageService>();
builder.Services.AddScoped<IFaceVerificationService, AzureFaceVerificationService>();
builder.Services.AddScoped<IGeocodingService, NominatimGeocodingService>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IVerificacionEmailRepository, VerificacionEmailRepository>();
builder.Services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
builder.Services.AddScoped<IPasswordHistoryRepository, PasswordHistoryRepository>();

// ==================== DOMAIN SERVICES ====================
builder.Services.AddScoped<IRegistrarEmailDomainService, RegistrarEmailDomainService>();
builder.Services.AddScoped<IPasswordResetDomainService, PasswordResetDomainService>();  

// ==================== INFRASTRUCTURE SERVICES ====================
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// ==================== MEDIATR ====================
builder.Services.AddMediatR(typeof(Program));


// Common
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>(); 

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend"); // IMPORTANTE: Antes de Authentication

app.UseAuthentication(); // IMPORTANTE: Antes de Authorization
app.UseAuthorization();

app.MapControllers();

app.Run();