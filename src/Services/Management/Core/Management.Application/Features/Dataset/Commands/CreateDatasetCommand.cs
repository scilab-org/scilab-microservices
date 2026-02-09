using AutoMapper;
using Management.Application.Dtos.Datasets;
using Management.Domain.Entities;
using Management.Application.Services;
using Marten;
using MediatR;

namespace Management.Application.Features.Dataset.Commands;

public record CreateDatasetCommand(CreateDatasetDto Dto) : ICommand<Guid>;

public class CreateDatasetCommandValidator : AbstractValidator<CreateDatasetCommand>
{
    #region Ctors

    public CreateDatasetCommandValidator()
    {
        RuleFor(x => x.Dto)
            .NotNull()
            .WithMessage(MessageCode.BadRequest)
            .DependentRules(() =>
            {
                RuleFor(x => x.Dto.Name)
                    .NotEmpty()
                    .WithMessage(MessageCode.DatasetNameIsRequired);
                RuleFor(x => x.Dto.ProjectId)
                    .NotNull()
                    .WithMessage(MessageCode.ProjectIdIsRequired);
            });
        
    }

    #endregion
}

public class CreateDatasetCommandHandler(IMapper mapper,
    IDocumentSession session,
    IMinIoCloudService minIo) : ICommandHandler<CreateDatasetCommand, Guid>
{
    #region Implementations

    public async Task<Guid> Handle(CreateDatasetCommand command, CancellationToken cancellationToken)
    {
        
        
        var dto = command.Dto;
        
        var projectDatasetExists = await session.LoadAsync<ProjectEntity>(dto.ProjectId, cancellationToken);
        if (projectDatasetExists == null)
            throw new ClientValidationException(
                MessageCode.ProjectIsNotExists, dto.ProjectId.ToString());
        
        var datasetId = Guid.NewGuid();
        await session.BeginTransactionAsync(cancellationToken);
        var entity = DatasetEntity.Create(
            datasetId,
            dto.Name!,
            dto.Description);
        
        await UploadFileAsync(dto.UploadFile, entity, cancellationToken);
        
        session.Store(entity);
        
        var projectDataset = ProjectDatasetEntity.Create(
            dto.ProjectId,
            datasetId);
        session.Store(projectDataset);
        await session.SaveChangesAsync(cancellationToken);
        
        return entity.Id;
    }

    #endregion
    
    #region Methods

    private async Task UploadFileAsync(UploadFileBytes? file, 
        DatasetEntity entity,
        CancellationToken cancellationToken)
    {
        if (file == null) return;

        var result = await minIo.UploadFilesAsync(entity.Id.ToString(), [file],
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