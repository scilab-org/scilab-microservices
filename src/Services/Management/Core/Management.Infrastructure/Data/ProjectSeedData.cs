using Management.Domain.Entities;
using Management.Domain.Enums;

namespace Management.Infrastructure.Data;

public static class ProjectSeedData
{
    #region Constants

    public static readonly Guid ProjectAIId =
        Guid.Parse("a1111111-1111-1111-1111-111111111111");

    public static readonly Guid ProjectMarketId =
        Guid.Parse("a2222222-2222-2222-2222-222222222222");

    #endregion

    #region Methods

    public static ProjectEntity[] GetProjects(string performedBy)
    {
        return new[]
        {
            ProjectEntity.Create(
                id: ProjectAIId,
                name: "AI Recommendation System",
                description: "Build AI-based recommendation system",
                code: "AI-REC-001",
                status: ProjectStatus.Draft,
                startDate: DateTimeOffset.UtcNow.AddMonths(-2),
                endDate: DateTimeOffset.UtcNow.AddMonths(4)),

            ProjectEntity.Create(
                id: ProjectMarketId,
                name: "Market Analysis",
                description: "Analyze market trends",
                code: "MK-ANA-002",
                status: ProjectStatus.Draft,
                startDate: DateTimeOffset.UtcNow.AddMonths(-1),
                endDate: DateTimeOffset.UtcNow.AddMonths(2))
        };
    }

    #endregion
}