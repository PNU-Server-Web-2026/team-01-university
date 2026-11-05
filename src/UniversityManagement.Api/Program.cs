using UniversityManagement.Api.Data;
using Microsoft.EntityFrameworkCore;
<<<<<<< HEAD
using UniversityManagement.Api.Features.Cors;
=======
using UniversityManagement.Api.Features.Students;

>>>>>>> feat/15/student-crud
var builder = WebApplication.CreateBuilder(args);

// Сервіси. Реєстрацію сервісів конкретних фіч додавайте через extension-методи
// у папці фічі (наприклад, builder.Services.AddHotelsFeature();), див. CONTRIBUTING.md.
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
builder.Services.AddStudentsFeature();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));




builder.Services.AddCorsFeature(builder.Configuration);




var app = builder.Build();


// HTTP-конвеєр.
if (app.Environment.IsDevelopment())
{
    // OpenAPI-документ: /openapi/v1.json
    app.MapOpenApi();
}

app.UseCorsFeature();
app.MapControllers();

app.Run();

// Потрібно для інтеграційних тестів (WebApplicationFactory<Program>).
public partial class Program;
