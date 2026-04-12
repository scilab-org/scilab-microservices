namespace User.Application.Tests.Common.TestData;

public static class UserTestData
{
    public static UserDto CreateUserDto(
        string id = "user-id-001",
        string username = "jdoe",
        string email = "jdoe@example.com",
        string firstName = "John",
        string lastName = "Doe",
        bool enabled = true) =>
        new()
        {
            Id = id,
            Username = username,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Enabled = enabled,
            EmailVerified = true,
            CreatedTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            AvatarUrl = AppConstants.Bucket.DefaultAvatar,
            Groups = []
        };

    public static CreateUserDto CreateUserDtoRequest(
        string username = "jdoe",
        string email = "jdoe@example.com",
        string firstName = "John",
        string lastName = "Doe",
        string initialPassword = "SecurePass123!",
        bool temporaryPassword = false,
        UploadFileBytes? avatarImage = null,
        List<string>? groupNames = null) =>
        new()
        {
            Username = username,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            InitialPassword = initialPassword,
            TemporaryPassword = temporaryPassword,
            AvatarImage = avatarImage,
            GroupNames = groupNames
        };

    public static UpdateUserDto UpdateUserDtoRequest(
        string? firstName = "Jane",
        string? lastName = "Smith",
        bool enabled = true,
        UploadFileBytes? avatarImage = null,
        List<string>? groupNames = null) =>
        new()
        {
            FirstName = firstName,
            LastName = lastName,
            Enabled = enabled,
            AvatarImage = avatarImage,
            GroupNames = groupNames
        };

    public static List<UserDto> CreateUserDtoList(int count = 3)
    {
        return Enumerable.Range(1, count)
            .Select(i => CreateUserDto(
                id: $"user-id-{i:D3}",
                username: $"user{i}",
                email: $"user{i}@example.com",
                firstName: $"First{i}",
                lastName: $"Last{i}"))
            .ToList();
    }

    public static UploadFileBytes CreateAvatarBytes() =>
        new()
        {
            FileName = "avatar.png",
            ContentType = "image/png",
            Bytes = new byte[] { 0x89, 0x50, 0x4E, 0x47 }
        };

    public static Actor SystemActor() => Actor.System("test-system");
    public static Actor UserActor(string userId = "actor-user-001") => Actor.User(userId);
}
