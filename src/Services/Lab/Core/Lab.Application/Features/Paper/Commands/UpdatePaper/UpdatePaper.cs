using Lab.Application.Dtos.Papers;
using Lab.Application.Services;
using Lab.Domain.Entities;
using Marten;
using MediatR;

namespace Lab.Application.Features.Paper.Commands.UpdatePaper;

public record UpdatePaperCommand(Guid Id, UpdatePaperDto Dto) : ICommand<Guid>;

public class UpdatePaperCommandValidator : AbstractValidator<UpdatePaperCommand>
{
    public UpdatePaperCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(MessageCode.PaperIdIsRequired);

        RuleFor(x => x.Dto)
            .NotNull()
            .WithMessage(MessageCode.BadRequest)
            .DependentRules(() =>
            {
                RuleFor(x => x.Dto)
                    .NotNull()
                    .WithMessage(MessageCode.BadRequest)
                    .DependentRules(() =>
                    {
                        RuleFor(x => x.Dto.Title)
                            .NotEmpty()
                            .WithMessage(MessageCode.PaperTitleIsRequired)
                            .NotNull()
                            .WithMessage(MessageCode.PaperTitleIsRequired);

                        RuleFor(x => x.Dto.PublicationDate)
                            .LessThanOrEqualTo(DateTimeOffset.UtcNow)
                            .When(x => x.Dto.PublicationDate.HasValue)
                            .WithMessage(MessageCode.PaperPublicationDateInvalid);
                    });
                RuleFor(x => x.Dto.PublicationDate)
                    .LessThanOrEqualTo(DateTimeOffset.UtcNow)
                    .When(x => x.Dto.PublicationDate.HasValue)
                    .WithMessage(MessageCode.PaperPublicationDateInvalid);
            });
    }
}

public class UpdatePaperCommandHandler(IDocumentSession session, IMinIoCloudService minIo)
    : IRequestHandler<UpdatePaperCommand, Guid>
{
    public async Task<Guid> Handle(UpdatePaperCommand request, CancellationToken cancellationToken)
    {
        await session.BeginTransactionAsync(cancellationToken);

        var entity = await session.LoadAsync<PaperEntity>(request.Id, cancellationToken)
                     ?? throw new ClientValidationException(MessageCode.PaperIsNotExists, request.Id);

        var dto = request.Dto;

        entity.Update(
            title: dto.Title,
            abstractText: dto.Abstract,
            doi: dto.Doi,
            status: dto.Status,
            publicationDate: dto.PublicationDate,
            paperType: dto.PaperType,
            journalName: dto.JournalName,
            conferenceName: dto.ConferenceName);

        await UploadFileAsync(dto.UploadFile, entity, cancellationToken);

        session.Store(entity);
        await session.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    #region Methods

    private async Task UploadFileAsync(UploadFileBytes? file,
        PaperEntity entity,
        CancellationToken cancellationToken)
    {
        if (file == null) return;

        var result = await minIo.UploadFilesAsync(entity.Id.ToString(), [file],
            AppConstants.Bucket.Papers,
            true,
            cancellationToken);

        var uploaded = result.FirstOrDefault();

        if (uploaded != null)
        {
            entity.UpdateFilePath(uploaded.PublicURL);
        }
    }

    #endregion
}