using Lab.Application.Dtos.Abstractions;

namespace Lab.Application.Dtos.GapTypes;

public class GapTypeInfoDto : DtoId<Guid>
{
    public string Name { get; set; } = null!;
}
