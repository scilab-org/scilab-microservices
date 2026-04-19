using System.Text;
using System.Text.Json;
using Management.Infrastructure.Services;
using Microsoft.Extensions.Caching.Distributed;

namespace Management.Infrastructure.Tests.Services;

public sealed class RedisServiceTests
{
    private readonly Mock<IDistributedCache> _cacheMock = new();
    private readonly Mock<ILogger<RedisService>> _loggerMock = new();
    private readonly RedisService _sut;

    public RedisServiceTests()
    {
        _sut = new RedisService(_cacheMock.Object, _loggerMock.Object);
    }

    // ==========================================
    // GetOrSetCacheAsync
    // ==========================================

    [Fact]
    public async Task GetOrSetCacheAsync_Should_ReturnCachedValue_WhenCacheHit()
    {
        var key = "test-key";
        var expected = new TestDto { Name = "cached" };
        var bytes = JsonSerializer.SerializeToUtf8Bytes(expected);

        _cacheMock.Setup(c => c.GetAsync(key, It.IsAny<CancellationToken>()))
            .ReturnsAsync(bytes);

        var result = await _sut.GetOrSetCacheAsync<TestDto>(key, _ => Task.FromResult(new TestDto { Name = "fresh" }));

        result.Should().NotBeNull();
        result!.Name.Should().Be("cached");
    }

    [Fact]
    public async Task GetOrSetCacheAsync_Should_InvokeFactory_WhenCacheMiss()
    {
        var key = "test-key";
        _cacheMock.Setup(c => c.GetAsync(key, It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);

        var result = await _sut.GetOrSetCacheAsync<TestDto>(key, _ => Task.FromResult(new TestDto { Name = "fresh" }));

        result.Should().NotBeNull();
        result!.Name.Should().Be("fresh");
        _cacheMock.Verify(c => c.SetAsync(key, It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetOrSetCacheAsync_Should_UseCustomExpiration()
    {
        var key = "test-key";
        var expiration = TimeSpan.FromMinutes(10);
        _cacheMock.Setup(c => c.GetAsync(key, It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);

        await _sut.GetOrSetCacheAsync<TestDto>(key, _ => Task.FromResult(new TestDto { Name = "val" }), expiration);

        _cacheMock.Verify(c => c.SetAsync(key, It.IsAny<byte[]>(),
            It.Is<DistributedCacheEntryOptions>(o => o.AbsoluteExpirationRelativeToNow == expiration),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetOrSetCacheAsync_Should_ReturnFactoryResult_WhenGetThrows()
    {
        var key = "test-key";
        _cacheMock.Setup(c => c.GetAsync(key, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Redis down"));

        var result = await _sut.GetOrSetCacheAsync<TestDto>(key, _ => Task.FromResult(new TestDto { Name = "fallback" }));

        result.Should().NotBeNull();
        result!.Name.Should().Be("fallback");
    }

    [Fact]
    public async Task GetOrSetCacheAsync_Should_ReturnEmptyBytes_WhenCacheIsEmpty()
    {
        var key = "test-key";
        _cacheMock.Setup(c => c.GetAsync(key, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<byte>());

        var result = await _sut.GetOrSetCacheAsync<TestDto>(key, _ => Task.FromResult(new TestDto { Name = "fresh" }));

        result!.Name.Should().Be("fresh");
    }

    // ==========================================
    // RemoveCacheAsync
    // ==========================================

    [Fact]
    public async Task RemoveCacheAsync_Should_RemoveKey()
    {
        var key = "test-key";

        await _sut.RemoveCacheAsync(key);

        _cacheMock.Verify(c => c.RemoveAsync(key, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RemoveCacheAsync_Should_NotThrow_WhenRemoveFails()
    {
        var key = "test-key";
        _cacheMock.Setup(c => c.RemoveAsync(key, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Redis down"));

        var act = () => _sut.RemoveCacheAsync(key);

        await act.Should().NotThrowAsync();
    }

    // ==========================================
    // SetAsync error handling
    // ==========================================

    [Fact]
    public async Task GetOrSetCacheAsync_Should_ReturnResult_WhenSetThrows()
    {
        var key = "test-key";
        _cacheMock.Setup(c => c.GetAsync(key, It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);
        _cacheMock.Setup(c => c.SetAsync(key, It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Redis down"));

        var result = await _sut.GetOrSetCacheAsync<TestDto>(key, _ => Task.FromResult(new TestDto { Name = "val" }));

        result.Should().NotBeNull();
        result!.Name.Should().Be("val");
    }

    private sealed class TestDto
    {
        public string Name { get; set; } = "";
    }
}
