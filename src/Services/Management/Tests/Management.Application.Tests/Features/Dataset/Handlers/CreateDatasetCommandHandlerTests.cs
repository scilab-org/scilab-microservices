using Management.Application.Features.Dataset.Commands;
using Management.Application.Tests.Common;
using Management.Application.Tests.Common.TestData;

namespace Management.Application.Tests.Features.Dataset.Handlers;

public class CreateDatasetCommandHandlerTests : BaseTest
{
    private readonly Mock<IDocumentSession> _sessionMock;
    private readonly Mock<IMinIoCloudService> _minIoMock;
    private readonly CreateDatasetCommandHandler _handler;

    public CreateDatasetCommandHandlerTests()
    {
        _sessionMock = new Mock<IDocumentSession>();
        _minIoMock = new Mock<IMinIoCloudService>();
        _handler = new CreateDatasetCommandHandler(_sessionMock.Object, _minIoMock.Object);
    }

    [Fact]
    public async Task Handle_ProjectNotFound_ThrowsClientValidationException()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var dto = DatasetTestData.CreateCreateDatasetDto(projectId: projectId);
        var command = new CreateDatasetCommand(dto);

        _sessionMock.Setup(s => s.LoadAsync<ProjectEntity>(projectId, CancellationToken))
            .ReturnsAsync((ProjectEntity?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ClientValidationException>(
            () => _handler.Handle(command, CancellationToken));
    }

    [Fact]
    public async Task Handle_ValidProjectNoFile_CreatesDatasetWithoutUpload()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var dto = DatasetTestData.CreateCreateDatasetDto(projectId: projectId, uploadFile: null);
        var command = new CreateDatasetCommand(dto);

        var project = ProjectTestData.CreateProjectEntity(id: projectId);
        _sessionMock.Setup(s => s.LoadAsync<ProjectEntity>(projectId, CancellationToken))
            .ReturnsAsync(project);

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.Should().NotBeEmpty();
        _sessionMock.Verify(s => s.BeginTransactionAsync(CancellationToken), Times.Once);
        _sessionMock.Verify(s => s.Store(It.IsAny<DatasetEntity>()), Times.Once);
        _sessionMock.Verify(s => s.Store(project), Times.Once);
        _sessionMock.Verify(s => s.SaveChangesAsync(CancellationToken), Times.Once);
        _minIoMock.Verify(s => s.UploadFilesAsync(
            It.IsAny<string>(), It.IsAny<List<UploadFileBytes>>(),
            It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Never);
        project.DatasetIds.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_ValidProjectWithFile_UploadsAndSetsFilePath()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var uploadFile = DatasetTestData.CreateUploadFileBytes();
        var dto = DatasetTestData.CreateCreateDatasetDto(projectId: projectId, uploadFile: uploadFile);
        var command = new CreateDatasetCommand(dto);

        var project = ProjectTestData.CreateProjectEntity(id: projectId);
        _sessionMock.Setup(s => s.LoadAsync<ProjectEntity>(projectId, CancellationToken))
            .ReturnsAsync(project);

        var uploadResult = DatasetTestData.CreateUploadFileResult(publicUrl: "https://minio/test.csv");
        _minIoMock.Setup(s => s.UploadFilesAsync(
                It.IsAny<string>(), It.Is<List<UploadFileBytes>>(l => l.Contains(uploadFile)),
                AppConstants.Bucket.Datasets, true, CancellationToken))
            .ReturnsAsync(new List<UploadFileResult> { uploadResult });

        DatasetEntity? storedDataset = null;
        _sessionMock.Setup(s => s.Store(It.IsAny<DatasetEntity>()))
            .Callback<DatasetEntity[]>(entities =>
            {
                storedDataset = entities.FirstOrDefault();
            });

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.Should().NotBeEmpty();
        _minIoMock.Verify(s => s.UploadFilesAsync(
            It.IsAny<string>(), It.IsAny<List<UploadFileBytes>>(),
            AppConstants.Bucket.Datasets, true, CancellationToken), Times.Once);
        _sessionMock.Verify(s => s.SaveChangesAsync(CancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_UploadReturnsNoResult_DoesNotSetFilePath()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var uploadFile = DatasetTestData.CreateUploadFileBytes();
        var dto = DatasetTestData.CreateCreateDatasetDto(projectId: projectId, uploadFile: uploadFile);
        var command = new CreateDatasetCommand(dto);

        var project = ProjectTestData.CreateProjectEntity(id: projectId);
        _sessionMock.Setup(s => s.LoadAsync<ProjectEntity>(projectId, CancellationToken))
            .ReturnsAsync(project);

        _minIoMock.Setup(s => s.UploadFilesAsync(
                It.IsAny<string>(), It.IsAny<List<UploadFileBytes>>(),
                AppConstants.Bucket.Datasets, true, CancellationToken))
            .ReturnsAsync(new List<UploadFileResult>());

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.Should().NotBeEmpty();
        _sessionMock.Verify(s => s.SaveChangesAsync(CancellationToken), Times.Once);
    }
}
