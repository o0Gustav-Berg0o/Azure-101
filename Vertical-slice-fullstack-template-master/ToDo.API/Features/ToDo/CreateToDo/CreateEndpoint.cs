using Microsoft.AspNetCore.Mvc;
using ToDo.API.Features.ToDo.CreateToDo;

namespace ToDo.API.Features.Tasks.Create
{
    public static class CreateTaskEndpoint
    {
        public static void MapCreateTask(this IEndpointRouteBuilder app)
        {
            app.MapPost("/tasks", async (
                CreateTaskRequestDTO request,
                CreateTaskHandler handler,
                CreateTaskValidator validator,
                CancellationToken ct) =>
            {
                var validationResult = validator.Validate(request);

                if (!validationResult.IsValid)
                    return Results.BadRequest(validationResult.Errors);

                var result = await handler.HandleAsync(request, ct);

                return Results.Ok(result);
            });
        }
    }
}
