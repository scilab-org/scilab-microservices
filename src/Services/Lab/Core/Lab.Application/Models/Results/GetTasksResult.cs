using Lab.Application.Dtos.Tasks;

namespace Lab.Application.Models.Results;

public class GetTasksResult
{
        public List<TaskDto> Items { get; init; }
    
        public GetTasksResult(List<TaskDto> items)
        {
            Items = items;
        }
}