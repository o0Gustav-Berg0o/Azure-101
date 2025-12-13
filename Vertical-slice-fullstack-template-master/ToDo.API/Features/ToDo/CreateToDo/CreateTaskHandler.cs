using ToDo.API.Features.ToDo.CreateToDo;
using ToDo.Domain.Entities;
using ToDo.Domain.Enums;
using ToDo.Infrastructure.Persistence;

namespace ToDo.API.Features.Tasks.Create
{
    public class CreateTaskHandler
    {
        private readonly ToDoAppContext _context;

        public CreateTaskHandler(ToDoAppContext context)
        {
            _context = context;
        }

        public async Task<CreateTaskResponseDTO> HandleAsync(CreateTaskRequestDTO request, CancellationToken ct)
        {
            var entity = new TodoTask
            {
                Uid = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description,
                Status = Status.NotStarted
            };

            _context.Tasks.Add(entity);
            await _context.SaveChangesAsync(ct);

            return new CreateTaskResponseDTO(entity.Uid);
        }
    }
}
