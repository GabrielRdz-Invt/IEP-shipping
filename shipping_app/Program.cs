using shipping_app.Models;
using shipping_app.Repositories;

using shipping_app.Security;
// using Microsoft.AspNetCore.Authentication.Negotiate; (opcional si luego usas Windows Auth)
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 1) Services (ANTES de Build)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClients", policy =>
    {
        policy
            .WithOrigins("http://localhost:3000", "http://localhost:4200", "http://localhost:5173")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// Controllers + Endpoints + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "shipping_app API", Version = "v1" });
});

// Authorization (evita la excepción al usar UseAuthorization)
builder.Services.AddAuthorization();

// LDAP: configuración e inyección del servicio
var ldapSection = builder.Configuration.GetSection("Ldap");
builder.Services.AddSingleton(ldapSection.Get<LdapSettings>() ?? new LdapSettings
{
    // Fallback por si faltara en appsettings.json
    Domain = "iec.inventec",
    Container = "DC=iec,DC=inventec",
    ValidateContext = "Domain"
});
builder.Services.AddScoped<ILdapAuthService, LdapAuthService>();

// Repositorio IEP (tu código existente)
builder.Services.AddScoped<IIepCrossingDockShipmentRepository, IepCrossingDockShipment>();

// 2) Build
var app = builder.Build();

// 3) Middleware pipeline (DESPUÉS de Build)

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowClients");

// (Opcional) Si activas Authentication arriba, aquí va:
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
