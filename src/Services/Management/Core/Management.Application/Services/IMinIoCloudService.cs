namespace Management.Application.Services;

public interface IMinIoCloudService
{
    #region Methods

    Task<List<UploadFileResult>> UploadFilesAsync(string? fileId,
        List<UploadFileBytes> files,
        string bucketName,
        bool isPublicBucket = false,
        CancellationToken ct = default);

    Task<string> GetShareLinkAsync(string bucketName, string objectName, int expireTime);

    #endregion
}