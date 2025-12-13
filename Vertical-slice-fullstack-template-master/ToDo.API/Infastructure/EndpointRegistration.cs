using ToDo.API.Features.Tasks.Create;

namespace ToDo.API.Endpoints
{
    public static class EndpointRegistration
    {
        public static void MapApiEndpoints(this IEndpointRouteBuilder app)
        {
          
            app.MapCreateTask(); 
          
        }
    }
}
