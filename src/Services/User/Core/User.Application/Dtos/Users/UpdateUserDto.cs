namespace User.Application.Dtos.Users;

public sealed class UpdateUserDto
{
    #region Fields, Properties and Indexers

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public bool Enabled { get; set; } = true;

    public UploadFileBytes? AvatarImage { get; set; }

    public List<string>? GroupNames { get; set; }
    public string? OcrId { get; set; }

    #endregion
}
