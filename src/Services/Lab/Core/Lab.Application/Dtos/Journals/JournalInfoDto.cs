using Lab.Application.Dtos.Abstractions;
using Lab.Domain.Models;

namespace Lab.Application.Dtos.Journals;

public class JournalInfoDto : DtoId<Guid>
{
    #region Fields, Properties and Indexers

    public string Name { get; set; } = null!;
    public List<Style> Styles { get; set; } = [];

    #endregion
}