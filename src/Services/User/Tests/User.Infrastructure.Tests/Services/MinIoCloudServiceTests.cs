using Minio;
using Minio.DataModel.Args;
using Minio.DataModel.Response;
using Minio.Exceptions;

namespace User.Infrastructure.Tests.Services;

public sealed class MinIoCloudServiceTests
{
    #region Setup

    private readonly Mock<IMinioClient> _minioClientMock = new();
    private readonly MinIoCloudService _sut;

    private const string Endpoint = "http://minio.test:9000";

    public MinIoCloudServiceTests()
    {
        var config = new MinioConfig();
        typeof(MinioConfig).GetProperty("Endpoint")!.SetValue(config, Endpoint);
        _minioClientMock
            .Setup(x => x.Config)
            .Returns(config);

        _sut = new MinIoCloudService(_minioClientMock.Object);
    }

    private static UploadFileBytes CreateTestFile(string fileName = "test.png", string contentType = "image/png")
    {
        return new UploadFileBytes
        {
            FileName = fileName,
            ContentType = contentType,
            Bytes = new byte[] { 0x89, 0x50, 0x4E, 0x47 }
        };
    }

    private void SetupBucketExists(bool exists)
    {
        _minioClientMock
            .Setup(x => x.BucketExistsAsync(It.IsAny<BucketExistsArgs>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(exists);
    }

    private void SetupMakeBucket()
    {
        _minioClientMock
            .Setup(x => x.MakeBucketAsync(It.IsAny<MakeBucketArgs>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    }

    private void SetupSetPolicy()
    {
        _minioClientMock
            .Setup(x => x.SetPolicyAsync(It.IsAny<SetPolicyArgs>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    }

    private void SetupPutObject()
    {
        _minioClientMock
            .Setup(x => x.PutObjectAsync(It.IsAny<PutObjectArgs>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PutObjectResponse(HttpStatusCode.OK, "test-bucket", new Dictionary<string, string>(), 4, "test-object"));
    }

    #endregion

    #region UploadFilesAsync

    [Fact]
    public async Task UploadFilesAsync_ShouldReturnEmptyList_WhenFilesIsNull()
    {
        // Arrange & Act
        var result = await _sut.UploadFilesAsync(null!, "test-bucket");

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task UploadFilesAsync_ShouldReturnEmptyList_WhenFilesIsEmpty()
    {
        // Arrange & Act
        var result = await _sut.UploadFilesAsync([], "test-bucket");

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task UploadFilesAsync_ShouldUploadAndReturnResults_WhenBucketExists()
    {
        // Arrange
        SetupBucketExists(true);
        SetupPutObject();
        var files = new List<UploadFileBytes> { CreateTestFile() };

        // Act
        var result = await _sut.UploadFilesAsync(files, "test-bucket");

        // Assert
        result.Should().ContainSingle();
        result[0].FolderName.Should().Be("test-bucket");
        result[0].OriginalFileName.Should().Be("test.png");
        result[0].ContentType.Should().Be("image/png");
        result[0].FileSize.Should().Be(4);
        result[0].PublicURL.Should().BeEmpty();
    }

    [Fact]
    public async Task UploadFilesAsync_ShouldSetPublicURL_WhenIsPublicBucket()
    {
        // Arrange
        SetupBucketExists(true);
        SetupPutObject();
        var files = new List<UploadFileBytes> { CreateTestFile() };

        // Act
        var result = await _sut.UploadFilesAsync(files, "public-bucket", isPublicBucket: true);

        // Assert
        result.Should().ContainSingle();
        result[0].PublicURL.Should().StartWith($"{Endpoint}/public-bucket/");
    }

    [Fact]
    public async Task UploadFilesAsync_ShouldCreateBucketAndSetPolicy_WhenBucketNotExistsAndPublic()
    {
        // Arrange
        SetupBucketExists(false);
        SetupMakeBucket();
        SetupSetPolicy();
        SetupPutObject();
        var files = new List<UploadFileBytes> { CreateTestFile() };

        // Act
        await _sut.UploadFilesAsync(files, "new-public-bucket", isPublicBucket: true);

        // Assert
        _minioClientMock.Verify(
            x => x.MakeBucketAsync(It.IsAny<MakeBucketArgs>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _minioClientMock.Verify(
            x => x.SetPolicyAsync(It.IsAny<SetPolicyArgs>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UploadFilesAsync_ShouldCreateBucketWithoutPolicy_WhenBucketNotExistsAndPrivate()
    {
        // Arrange
        SetupBucketExists(false);
        SetupMakeBucket();
        SetupPutObject();
        var files = new List<UploadFileBytes> { CreateTestFile() };

        // Act
        await _sut.UploadFilesAsync(files, "new-private-bucket", isPublicBucket: false);

        // Assert
        _minioClientMock.Verify(
            x => x.MakeBucketAsync(It.IsAny<MakeBucketArgs>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _minioClientMock.Verify(
            x => x.SetPolicyAsync(It.IsAny<SetPolicyArgs>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task UploadFilesAsync_ShouldThrowInfrastructureException_WhenMinioExceptionOccurs()
    {
        // Arrange
        SetupBucketExists(true);
        _minioClientMock
            .Setup(x => x.PutObjectAsync(It.IsAny<PutObjectArgs>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new BucketNotFoundException("test-bucket", "not found"));
        var files = new List<UploadFileBytes> { CreateTestFile() };

        // Act
        var act = () => _sut.UploadFilesAsync(files, "test-bucket");

        // Assert
        await act.Should().ThrowAsync<InfrastructureException>();
    }

    [Fact]
    public async Task UploadFilesAsync_ShouldUploadMultipleFiles()
    {
        // Arrange
        SetupBucketExists(true);
        SetupPutObject();
        var files = new List<UploadFileBytes>
        {
            CreateTestFile("file1.png"),
            CreateTestFile("file2.jpg", "image/jpeg")
        };

        // Act
        var result = await _sut.UploadFilesAsync(files, "test-bucket");

        // Assert
        result.Should().HaveCount(2);
        result[0].OriginalFileName.Should().Be("file1.png");
        result[1].OriginalFileName.Should().Be("file2.jpg");
    }

    #endregion

    #region UploadFileAsync

    [Fact]
    public async Task UploadFileAsync_ShouldReturnResult_WhenBucketExists()
    {
        // Arrange
        SetupBucketExists(true);
        SetupPutObject();
        var file = CreateTestFile();

        // Act
        var result = await _sut.UploadFileAsync(file, "test-bucket", "custom-object-name.png");

        // Assert
        result.FileId.Should().Be("custom-object-name.png");
        result.FolderName.Should().Be("test-bucket");
        result.OriginalFileName.Should().Be("test.png");
        result.FileName.Should().Be("custom-object-name.png");
        result.FileSize.Should().Be(4);
        result.PublicURL.Should().BeEmpty();
    }

    [Fact]
    public async Task UploadFileAsync_ShouldSetPublicURL_WhenIsPublicBucket()
    {
        // Arrange
        SetupBucketExists(true);
        SetupPutObject();
        var file = CreateTestFile();

        // Act
        var result = await _sut.UploadFileAsync(file, "public-bucket", "avatar.png", isPublicBucket: true);

        // Assert
        result.PublicURL.Should().Be($"{Endpoint}/public-bucket/avatar.png");
    }

    [Fact]
    public async Task UploadFileAsync_ShouldThrowInfrastructureException_WhenMinioExceptionOccurs()
    {
        // Arrange
        SetupBucketExists(true);
        _minioClientMock
            .Setup(x => x.PutObjectAsync(It.IsAny<PutObjectArgs>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new BucketNotFoundException("test-bucket", "not found"));
        var file = CreateTestFile();

        // Act
        var act = () => _sut.UploadFileAsync(file, "test-bucket", "object.png");

        // Assert
        await act.Should().ThrowAsync<InfrastructureException>();
    }

    #endregion

    #region GetShareLinkAsync

    [Fact]
    public async Task GetShareLinkAsync_ShouldReturnPresignedUrl()
    {
        // Arrange
        const string expectedUrl = "http://minio.test:9000/test-bucket/object.png?signature=abc";
        _minioClientMock
            .Setup(x => x.PresignedGetObjectAsync(It.IsAny<PresignedGetObjectArgs>()))
            .ReturnsAsync(expectedUrl);

        // Act
        var result = await _sut.GetShareLinkAsync("test-bucket", "object.png", 60);

        // Assert
        result.Should().Be(expectedUrl);
    }

    [Fact]
    public async Task GetShareLinkAsync_ShouldThrowInfrastructureException_WhenExceptionOccurs()
    {
        // Arrange
        _minioClientMock
            .Setup(x => x.PresignedGetObjectAsync(It.IsAny<PresignedGetObjectArgs>()))
            .ThrowsAsync(new Exception("presign failed"));

        // Act
        var act = () => _sut.GetShareLinkAsync("test-bucket", "object.png", 60);

        // Assert
        await act.Should().ThrowAsync<InfrastructureException>();
    }

    #endregion
}
