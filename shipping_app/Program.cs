using shipping_app.Models;
using shipping_app.Repositories;
using Microsoft.OpenApi.Models;
// using shipping_app.Services;
// using shipping_app.Security;

var builder = WebApplication.CreateBuilder(args);


/* builder.Services.AddHttpClient("AuthApi", client => {
    client.BaseAddress = new Uri(builder.Configuration["AuthApi:BaseUrl"]!);
}); */

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
builder.Services.AddAuthorization();
// builder.Services.AddScoped<AuthBridge>();
builder.Services.AddScoped<IIepCrossingDockShipmentRepository, IepCrossingDockShipment>();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseCors("AllowClients");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();