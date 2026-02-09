using AutoMapper;
using Management.Application.Dtos.Datasets;
using Management.Application.Services;
using Management.Domain.Entities;
using Marten;
using MediatR;

namespace Management.Application.Features.Dataset.Commands;

public record UpdateDatasetCommand(Guid DatasetId, UpdateDatasetDto Dto) : ICommand<Guid>;

public class UpdateDatasetCommandValidator : AbstractValidator<UpdateDatasetCommand>
{
    #region Ctors

    public UpdateDatasetCommandValidator()
    {
        RuleFor(x => x.DatasetId)
            .NotEmpty()
            .WithMessage(MessageCode.DatasetIdIsRequired);

        RuleFor(x => x.Dto)
            .NotNull()
            .WithMessage(MessageCode.BadRequest)
            .DependentRules(() =>
            {
                RuleFor(x => x.Dto.Name)
                    .NotEmpty()
                    .WithMessage(MessageCode.DatasetNameIsRequired);
            });
    }

    #endregion
}

public class UpdateDatasetCommandHandler(
    IMapper mapper,
    IDocumentSession session,
    IMinIoCloudService minIo) : ICommandHandler<UpdateDatasetCommand, Guid>
{
    #region Implementations

    public async Task<Guid> Handle(UpdateDatasetCommand command, CancellationToken cancellationToken)
    {
        
        await session.BeginTransactionAsync(cancellationToken);

        var entity = await session.LoadAsync<DatasetEntity>(command.DatasetId, cancellationToken)
            ?? throw new ClientValidationException(MessageCode.DatasetIsNotExists, command.DatasetId);

        var dto = command.Dto;

        entity.Update(
            name: dto.Name!,
            description: dto.Description!,
            status: dto.Status);
        
        await UploadFileAsync(entity.Id.ToString(), dto.UploadFile, entity, cancellationToken);

        session.Store(entity);
        await session.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    #endregion
    
    #region Methods

    private async Task UploadFileAsync(string? fileId, UploadFileBytes? file, DatasetEntity entity, CancellationToken cancellationToken)
    {
        if (file is null) return;
        
        var result = await minIo.UploadFilesAsync(fileId, [file],
            AppConstants.Bucket.Datasets,
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