using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.Section.Queries.GetSectionnFileById;

public record GetSectionnFileByIdQuery(Guid Id) : ICommand<List<string>>;

public class GetSectionnFileByIdQueryValidator : AbstractValidator<GetSectionnFileByIdQuery>
{
    public GetSectionnFileByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(MessageCode.SectionIdIsRequired)
            .NotNull()
            .WithMessage(MessageCode.SectionIdIsRequired);
    }
}

public class GetSectionnFileByIdQueryHandler(IDocumentSession session) : IRequestHandler<GetSectionnFileByIdQuery, List<string>>
{
    public async Task<List<string>> Handle(GetSectionnFileByIdQuery request, CancellationToken cancellationToken)
    {
        var section = await session.LoadAsync<SectionEntity>(request.Id, cancellationToken)
           ?? throw new NotFoundException(MessageCode.SectionIsNotExists, request.Id.ToString());

        return section.Files ?? new List<string>();
    }
}