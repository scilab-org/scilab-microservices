namespace User.Application.Tests.Common.TestData;

public static class RoleTestData
{
    public static RoleDto CreateRoleDto(
        string id = "role-id-001",
        string name = "view-data",
        string description = "Can view data",
        bool composite = false,
        bool clientRole = false) =>
        new()
        {
            Id = id,
            Name = name,
            Description = description,
            Composite = composite,
            ClientRole = clientRole
        };

    public static List<RoleDto> CreateRoleDtoList(int count = 3) =>
        Enumerable.Range(1, count)
            .Select(i => CreateRoleDto(
                id: $"role-id-{i:D3}",
                name: $"role-{i}",
                description: $"Role {i} description"))
            .ToList();
}
