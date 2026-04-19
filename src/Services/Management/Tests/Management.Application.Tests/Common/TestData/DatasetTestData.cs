namespace Management.Application.Tests.Common.TestData;

public static class DatasetTestData
{
    public static DatasetEntity CreateDatasetEntity(
        Guid? id = null,
        string name = "Test Dataset",
        string? description = "A test dataset")
    {
        return DatasetEntity.Create(
            id: id ?? Guid.NewGuid(),
            name: name,
            description: description);
    }

    public static CreateDatasetDto CreateCreateDatasetDto(
        Guid? projectId = null,
        string name = "Test Dataset",
        string? description = "A test dataset",
        UploadFileBytes? uploadFile = null)
    {
        return new CreateDatasetDto
        {
            ProjectId = projectId ?? Guid.NewGuid(),
            Name = name,
            Description = description,
            UploadFile = uploadFile
        };
    }

    public static UpdateDatasetDto CreateUpdateDatasetDto(
        string name = "Updated Dataset",
        string? description = "Updated description",
        DatasetStatus? status = DatasetStatus.Public,
        UploadFileBytes? uploadFile = null)
    {
        return new UpdateDatasetDto
        {
            Name = name,
            Description = description,
            Status = status,
            UploadFile = uploadFile
        };
    }

    public static UploadFileBytes CreateUploadFileBytes(
        string fileName = "test.csv",
        string contentType = "text/csv",
        byte[]? bytes = null)
    {
        return new UploadFileBytes
        {
            FileName = fileName,
            ContentType = contentType,
            Bytes = bytes ?? new byte[] { 1, 2, 3 }
        };
    }

    public static UploadFileResult CreateUploadFileResult(
        string? publicUrl = "https://minio.example.com/datasets/test.csv")
    {
        return new UploadFileResult
        {
            PublicURL = publicUrl
        };
    }
}
