using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using GradeInsight.Data;
using Microsoft.AspNetCore.Builder;
using GradeInsight.SpecificRepositories.Marks;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<GradeInsightContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("GradeInsightContext") ?? throw new InvalidOperationException("Connection string 'GradeInsightContext' not found.")));

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<IMarksRepositories, MarksRepositories>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});




var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.UseCors(options=>options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());


app.UseAuthorization();

app.MapControllers();

app.Run();
