using Management.Application.Features.Dataset.Commands;
using Management.Application.Tests.Common;
using Management.Application.Tests.Common.TestData;

namespace Management.Application.Tests.Features.Dataset.Handlers;

public class UpdateDatasetCommandHandlerTests : BaseTest
{
    private readonly Mock<IDocumentSession> _sessionMock;
    private readonly Mock<IMinIoCloudService> _minIoMock;
    private readonly UpdateDatasetCommandHandler _handler;

    public UpdateDatasetCommandHandlerTests()
    {
        _sessionMock = new Mock<IDocumentSession>();
        _minIoMock = new Mock<IMinIoCloudService>();
        _handler = new UpdateDatasetCommandHandler(_sessionMock.Object, _minIoMock.Object);
    }

    [Fact]
    public async Task Handle_DatasetNotFound_ThrowsClientValidationException()
    {
        // Arrange
        var datasetId = Guid.NewGuid();
        var dto = DatasetTestData.CreateUpdateDatasetDto();
        var command = new UpdateDatasetCommand(datasetId, dto);

        _sessionMock.Setup(s => s.LoadAsync<DatasetEntity>(datasetId, CancellationToken))
            .ReturnsAsync((DatasetEntity?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ClientValidationException>(
            () => _handler.Handle(command, CancellationToken));
    }

    [Fact]
    public async Task Handle_ValidDatasetNoFile_UpdatesWithoutUpload()
    {
        // Arrange
        var datasetId = Guid.NewGuid();
        var dto = DatasetTestData.CreateUpdateDatasetDto(
            name: "Updated Name",
            description: "Updated Desc",
            status: DatasetStatus.Public,
            uploadFile: null);
        var command = new UpdateDatasetCommand(datasetId, dto);

        var dataset = DatasetTestData.CreateDatasetEntity(id: datasetId);
        _sessionMock.Setup(s => s.LoadAsync<DatasetEntity>(datasetId, CancellationToken))
            .ReturnsAsync(dataset);

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.Should().Be(datasetId);
        dataset.Name.Should().Be("Updated Name");
        dataset.Description.Should().Be("Updated Desc");
        _sessionMock.Verify(s => s.BeginTransactionAsync(CancellationToken), Times.Once);
        _sessionMock.Verify(s => s.Store(dataset), Times.Once);
        _sessionMock.Verify(s => s.SaveChangesAsync(CancellationToken), Times.Once);
        _minIoMock.Verify(s => s.UploadFilesAsync(
            It.IsAny<string>(), It.IsAny<List<UploadFileBytes>>(),
            It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ValidDatasetWithFile_UploadsAndSetsFilePath()
    {
        // Arrange
        var datasetId = Guid.NewGuid();
        var uploadFile = DatasetTestData.CreateUploadFileBytes();
        var dto = DatasetTestData.CreateUpdateDatasetDto(uploadFile: uploadFile);
        var command = new UpdateDatasetCommand(datasetId, dto);

        var dataset = DatasetTestData.CreateDatasetEntity(id: datasetId);
        _sessionMock.Setup(s => s.LoadAsync<DatasetEntity>(datasetId, CancellationToken))
            .ReturnsAsync(dataset);

        var uploadResult = DatasetTestData.CreateUploadFileResult(publicUrl: "https://minio/updated.csv");
        _minIoMock.Setup(s => s.UploadFilesAsync(
                datasetId.ToString(), It.Is<List<UploadFileBytes>>(l => l.Contains(uploadFile)),
                AppConstants.Bucket.Datasets, true, CancellationToken))
            .ReturnsAsync(new List<UploadFileResult> { uploadResult });

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.Should().Be(datasetId);
        dataset.FilePath.Should().Be("https://minio/updated.csv");
        _sessionMock.Verify(s => s.Store(dataset), Times.Once);
        _sessionMock.Verify(s => s.SaveChangesAsync(CancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_UploadReturnsNoResult_DoesNotSetFilePath()
    {
        // Arrange
        var datasetId = Guid.NewGuid();
        var uploadFile = DatasetTestData.CreateUploadFileBytes();
        var dto = DatasetTestData.CreateUpdateDatasetDto(uploadFile: uploadFile);
        var command = new UpdateDatasetCommand(datasetId, dto);

        var dataset = DatasetTestData.CreateDatasetEntity(id: datasetId);
        var originalFilePath = dataset.FilePath;
        _sessionMock.Setup(s => s.LoadAsync<DatasetEntity>(datasetId, CancellationToken))
            .ReturnsAsync(dataset);

        _minIoMock.Setup(s => s.UploadFilesAsync(
                It.IsAny<string>(), It.IsAny<List<UploadFileBytes>>(),
                AppConstants.Bucket.Datasets, true, CancellationToken))
            .ReturnsAsync(new List<UploadFileResult>());

        // Act
        var result = await _handler.Handle(command, CancellationToken);

        // Assert
        result.Should().Be(datasetId);
        dataset.FilePath.Should().Be(originalFilePath);
        _sessionMock.Verify(s => s.SaveChangesAsync(CancellationToken), Times.Once);
    }
}
