using ToDo.Domain.Enums;

namespace ToDo.Domain.Entities

{
    public class TodoTask
    {
        public int Id { get; set; }
        public Guid Uid { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Status Status { get; set; }
    }
}
