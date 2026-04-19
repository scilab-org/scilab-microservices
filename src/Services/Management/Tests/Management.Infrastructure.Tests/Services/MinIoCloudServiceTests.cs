using System.Net;
using Management.Infrastructure.Services;
using Management.Infrastructure.Exceptions;
using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;
using Minio.DataModel.Response;
using Minio.Exceptions;
using Common.Models;

namespace Management.Infrastructure.Tests.Services;

public sealed class MinIoCloudServiceTests
{
    private readonly Mock<IMinioClient> _minioClientMock;
    private readonly MinIoCloudService _sut;

    public MinIoCloudServiceTests()
    {
        _minioClientMock = new Mock<IMinioClient>();
        var config = new MinioConfig();
        // Use reflection to set read-only Endpoint
        typeof(MinioConfig).GetProperty(nameof(MinioConfig.Endpoint))!
            .SetValue(config, "https://minio.example.com:9000");
        _minioClientMock.Setup(m => m.Config).Returns(config);
        _sut = new MinIoCloudService(_minioClientMock.Object);
    }

    // ==========================================
    // UploadFilesAsync
    // ==========================================

    [Fact]
    public async Task UploadFilesAsync_Should_ReturnEmptyList_WhenNoFiles()
    {
        var result = await _sut.UploadFilesAsync("id", new List<UploadFileBytes>(), "bucket");
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task UploadFilesAsync_Should_ReturnEmptyList_WhenNullFiles()
    {
        var result = await _sut.UploadFilesAsync("id", null!, "bucket");
        result.Should().BeEmpty();
    }

    // ==========================================
    // GetShareLinkAsync
    // ==========================================

    [Fact]
    public async Task GetShareLinkAsync_Should_ReturnPresignedUrl()
    {
        _minioClientMock.Setup(m => m.PresignedGetObjectAsync(It.IsAny<PresignedGetObjectArgs>()))
            .ReturnsAsync("https://minio.example.com/bucket/file.pdf?token=abc");

        var result = await _sut.GetShareLinkAsync("bucket", "file.pdf", 30);

        result.Should().Contain("minio.example.com");
    }

    [Fact]
    public async Task GetShareLinkAsync_Should_ThrowInfrastructureException_WhenMinioFails()
    {
        _minioClientMock.Setup(m => m.PresignedGetObjectAsync(It.IsAny<PresignedGetObjectArgs>()))
            .ThrowsAsync(new Exception("Minio error"));

        var act = () => _sut.GetShareLinkAsync("bucket", "file.pdf", 30);

        await act.Should().ThrowAsync<InfrastructureException>();
    }

    // ==========================================
    // UploadFilesAsync — success path
    // ==========================================

    [Fact]
    public async Task UploadFilesAsync_Should_ReturnResults_WhenSuccess()
    {
        _minioClientMock.Setup(m => m.BucketExistsAsync(It.IsAny<BucketExistsArgs>(), default))
            .ReturnsAsync(true);
        _minioClientMock.Setup(m => m.PutObjectAsync(It.IsAny<PutObjectArgs>(), default))
            .ReturnsAsync(new PutObjectResponse(System.Net.HttpStatusCode.OK, "bucket", new Dictionary<string, string>(), 100, "obj"));

        var files = new List<UploadFileBytes>
        {
            new() { FileName = "test.pdf", Bytes = new byte[] { 1, 2, 3 }, ContentType = "application/pdf" }
        };

        var result = await _sut.UploadFilesAsync("file-id", files, "test-bucket");

        result.Should().ContainSingle();
        result[0].FolderName.Should().Be("test-bucket");
        result[0].OriginalFileName.Should().Be("test.pdf");
    }

    [Fact]
    public async Task UploadFilesAsync_Should_SetPublicUrl_WhenPublicBucket()
    {
        _minioClientMock.Setup(m => m.BucketExistsAsync(It.IsAny<BucketExistsArgs>(), default))
            .ReturnsAsync(true);
        _minioClientMock.Setup(m => m.PutObjectAsync(It.IsAny<PutObjectArgs>(), default))
            .ReturnsAsync(new PutObjectResponse(System.Net.HttpStatusCode.OK, "bucket", new Dictionary<string, string>(), 100, "obj"));

        var files = new List<UploadFileBytes>
        {
            new() { FileName = "test.pdf", Bytes = new byte[] { 1, 2, 3 }, ContentType = "application/pdf" }
        };

        var result = await _sut.UploadFilesAsync("file-id", files, "test-bucket", isPublicBucket: true);

        result.Should().ContainSingle();
        result[0].PublicURL.Should().Contain("minio.example.com");
    }

    [Fact]
    public async Task UploadFilesAsync_Should_ThrowInfrastructureException_WhenMinioFails()
    {
        _minioClientMock.Setup(m => m.BucketExistsAsync(It.IsAny<BucketExistsArgs>(), default))
            .ThrowsAsync(new MinioException("Minio error"));

        var files = new List<UploadFileBytes>
        {
            new() { FileName = "test.pdf", Bytes = new byte[] { 1, 2, 3 }, ContentType = "application/pdf" }
        };

        var act = () => _sut.UploadFilesAsync("file-id", files, "test-bucket");

        await act.Should().ThrowAsync<InfrastructureException>();
    }

    // ==========================================
    // EnsureBucketAsync — create new bucket path
    // ==========================================

    [Fact]
    public async Task UploadFilesAsync_Should_CreateBucket_WhenNotExists()
    {
        _minioClientMock.Setup(m => m.BucketExistsAsync(It.IsAny<BucketExistsArgs>(), default))
            .ReturnsAsync(false);
        _minioClientMock.Setup(m => m.MakeBucketAsync(It.IsAny<MakeBucketArgs>(), default))
            .Returns(Task.CompletedTask);
        _minioClientMock.Setup(m => m.PutObjectAsync(It.IsAny<PutObjectArgs>(), default))
            .ReturnsAsync(new PutObjectResponse(System.Net.HttpStatusCode.OK, "bucket", new Dictionary<string, string>(), 100, "obj"));

        var files = new List<UploadFileBytes>
        {
            new() { FileName = "test.pdf", Bytes = new byte[] { 1, 2 }, ContentType = "application/pdf" }
        };

        var result = await _sut.UploadFilesAsync("file-id", files, "new-bucket");

        _minioClientMock.Verify(m => m.MakeBucketAsync(It.IsAny<MakeBucketArgs>(), default), Times.Once);
        result.Should().ContainSingle();
    }

    [Fact]
    public async Task UploadFilesAsync_Should_SetPublicPolicy_WhenNewPublicBucket()
    {
        _minioClientMock.Setup(m => m.BucketExistsAsync(It.IsAny<BucketExistsArgs>(), default))
            .ReturnsAsync(false);
        _minioClientMock.Setup(m => m.MakeBucketAsync(It.IsAny<MakeBucketArgs>(), default))
            .Returns(Task.CompletedTask);
        _minioClientMock.Setup(m => m.SetPolicyAsync(It.IsAny<SetPolicyArgs>(), default))
            .Returns(Task.CompletedTask);
        _minioClientMock.Setup(m => m.PutObjectAsync(It.IsAny<PutObjectArgs>(), default))
            .ReturnsAsync(new PutObjectResponse(System.Net.HttpStatusCode.OK, "bucket", new Dictionary<string, string>(), 100, "obj"));

        var files = new List<UploadFileBytes>
        {
            new() { FileName = "test.pdf", Bytes = new byte[] { 1, 2 }, ContentType = "application/pdf" }
        };

        await _sut.UploadFilesAsync("file-id", files, "public-bucket", isPublicBucket: true);

        _minioClientMock.Verify(m => m.SetPolicyAsync(It.IsAny<SetPolicyArgs>(), default), Times.Once);
    }

    // ==========================================
    // Constructor — endpoint without scheme
    // ==========================================

    [Fact]
    public void Constructor_Should_HandleEndpointWithoutScheme()
    {
        var mockClient = new Mock<IMinioClient>();
        var config = new MinioConfig();
        typeof(MinioConfig).GetProperty(nameof(MinioConfig.Endpoint))!
            .SetValue(config, "minio.local:9000");
        mockClient.Setup(m => m.Config).Returns(config);

        var sut = new MinIoCloudService(mockClient.Object);

        sut.Should().NotBeNull();
    }
}
