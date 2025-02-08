using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using GradeInsight.Data;
using Microsoft.AspNetCore.Builder;
using GradeInsight.SpecificRepositories.Marks;
using GradeInsight.SpecificRepositories.Faculties;
using GradeInsight.SpecificRepositories.Teachers;
using GradeInsight.SpecificRepositories.Courses;
using GradeInsight.SpecificRepositories.Students;
using GradeInsight.SpecificRepositories.Prediction;
using GradeInsight.SpecificRepositories.UserRepositories;
using GradeInsight.SpecificRepositories.UserRepositories.cs;
using GradeInsight;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<GradeInsightContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("GradeInsightContext") ?? throw new InvalidOperationException("Connection string 'GradeInsightContext' not found.")));

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<IMarksRepositories, MarksRepositories>();
builder.Services.AddScoped<IFacultiesRepositories, FacultiesRepositories>();
builder.Services.AddScoped<ITeachersRepositories,TeachersRepositories>();
builder.Services.AddScoped<ICoursesRepositories,CoursesRepositories>();
builder.Services.AddScoped<IStudentsRepositories,StudentsRepositories>();
builder.Services.AddScoped<IPredictionRepositories,PredictionRepositories>();
builder.Services.AddScoped<IUserRepositories,UserRepositories>();


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
BasePageModel.APIKey = builder.Configuration.GetSection("EnvironmentSettings:APIKey").Value;


app.UseAuthorization();

app.MapControllers();

app.Run();
