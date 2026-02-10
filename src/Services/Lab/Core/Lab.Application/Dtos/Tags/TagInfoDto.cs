using Lab.Application.Dtos.Abstractions;

namespace Lab.Application.Dtos.Tags;

public class TagInfoDto : DtoId<Guid>
{
    #region Fields, Properties and Indexers

    public string Name { get; set; } = null!;

    #endregion
}