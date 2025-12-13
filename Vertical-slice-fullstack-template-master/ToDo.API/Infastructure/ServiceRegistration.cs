using FluentValidation;
using ToDo.API.Features.Tasks.Create;
using ToDo.API.Features.ToDo.CreateToDo;

namespace ToDo.API.Extensions
{
    public static class ServiceRegistration
    {
        public static void AddApiServices(this IServiceCollection services)
        {
        
            services.AddScoped<CreateTaskHandler>();

            services.AddValidatorsFromAssemblyContaining<CreateTaskValidator>();
        }
    }
}
