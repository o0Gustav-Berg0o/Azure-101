
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using ToDo.API.Endpoints;
using ToDo.API.Extensions;
using ToDo.API.Features.ToDo.CreateToDo;
using ToDo.Infrastructure.Persistence;

namespace ToDo.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddApiServices();
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddDbContext<ToDoAppContext>(options =>
            options.UseSqlServer(
            builder.Configuration.GetConnectionString("Default")
            ));

            var app = builder.Build();

            app.MapApiEndpoints();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference(options =>
                {
                    options.Title = "ToDo API";
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
