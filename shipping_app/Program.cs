using shipping_app.Models;
using shipping_app.Repositories;

// Add services to the container.
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClients",policy =>
    {
        policy.WithOrigins("http://localhost:3000","http://localhost:4200")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Registro de interfaz -> implementación
builder.Services.AddScoped<IIepCrossingDockShipmentRepository, IepCrossingDockShipment>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowClients");

app.UseAuthorization();

app.MapControllers();

app.Run();
