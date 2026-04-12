namespace User.Application.Tests.Common.TestData;

public static class GroupTestData
{
    public static GroupDto CreateGroupDto(
        string id = "group-id-001",
        string name = "Researchers",
        string path = "/Researchers") =>
        new()
        {
            Id = id,
            Name = name,
            Path = path
        };

    public static List<GroupDto> CreateGroupDtoList(int count = 3) =>
        Enumerable.Range(1, count)
            .Select(i => CreateGroupDto(
                id: $"group-id-{i:D3}",
                name: $"Group{i}",
                path: $"/Group{i}"))
            .ToList();
}
