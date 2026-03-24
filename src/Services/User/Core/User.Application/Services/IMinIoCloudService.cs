namespace User.Application.Services;

public interface IMinIoCloudService
{
    #region Methods

    Task<List<UploadFileResult>> UploadFilesAsync(
        List<UploadFileBytes> files,
        string bucketName,
        bool isPublicBucket = false,
        CancellationToken ct = default);

    Task<UploadFileResult> UploadFileAsync(
        UploadFileBytes file,
        string bucketName,
        string objectName,
        bool isPublicBucket = false,
        CancellationToken ct = default);

    Task<string> GetShareLinkAsync(string bucketName, string objectName, int expireTime);

    #endregion

}
