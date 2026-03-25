using Lab.Domain.Enums;

namespace Lab.Application.Models.Filters;

public class GetTaskFilter
{
    #region Properties

    public string? AssignedToUserName { get; set; }
    public TaskDefineStatus? Status { get; set; }
    public Guid? PaperId { get; set; }
    public DateTaskFilterField? DateField { get; set; }
    public DateTimeOffset? FromDate { get; set; }
    public DateTimeOffset? ToDate { get; set; }

    #endregion
}
