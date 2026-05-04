using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.Section.Commands.DeleteSectionFile;

public record DeleteSectionFileCommand(Guid Id, string FileName) : ICommand<Unit>;

public class DeleteSectionFileCommandValidator : AbstractValidator<DeleteSectionFileCommand>
{
    public DeleteSectionFileCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(MessageCode.SectionIdIsRequired);

        RuleFor(x => x.FileName)
            .NotEmpty()
            .WithMessage(MessageCode.SectionFileNameIsRequired)
            .NotNull()
            .WithMessage(MessageCode.SectionFileNameIsRequired);
    }
}

public class DeleteSectionFileCommandHandler(IDocumentSession session) : ICommandHandler<DeleteSectionFileCommand, Unit>
{
    public async Task<Unit> Handle(DeleteSectionFileCommand request, CancellationToken cancellationToken)
    {
        await session.BeginTransactionAsync(cancellationToken);

        var section = await session.LoadAsync<SectionEntity>(request.Id, cancellationToken)
                      ?? throw new NotFoundException(MessageCode.SectionIsNotExists, request.Id.ToString());

        var files = section.Files ?? [];
        if (files.Count == 0)
            throw new NotFoundException(MessageCode.SectionFileNotFound, request.FileName);

        var target = files.FirstOrDefault(x =>
            string.Equals(ExtractFileName(x), request.FileName, StringComparison.OrdinalIgnoreCase));

        if (target == null)
            throw new NotFoundException(MessageCode.SectionFileNotFound, request.FileName);

        files.Remove(target);

        section.Update(files: files);
        session.Update(section);
        await session.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    private static string ExtractFileName(string pathOrUrl)
    {
        if (string.IsNullOrWhiteSpace(pathOrUrl)) return string.Empty;

        if (Uri.TryCreate(pathOrUrl, UriKind.Absolute, out var uri))
        {
            return Path.GetFileName(uri.LocalPath);
        }

        return Path.GetFileName(pathOrUrl);
    }
}