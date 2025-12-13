using ToDo.API.Features.Tasks.Create;

namespace ToDo.API.Endpoints
{
    public static class ValidatorRegistration
    {
        public static void MapValidators(this IEndpointRouteBuilder app)
        {
          
            app.MapCreateTask(); 
          
        }
    }
}
