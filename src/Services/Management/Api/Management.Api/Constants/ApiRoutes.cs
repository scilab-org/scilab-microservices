namespace Management.Api.Constants;

public sealed class ApiRoutes
{
    public static class System
    {
        #region Constants

        public const string Tags = "System";

        private const string BaseAdmin = "/admin/system";

        public const string InitializeData = $"{BaseAdmin}/initialize-data";

        #endregion
    }
    public static class Project
    {
        #region Constants
        public const string Tags = "Projects";
        
        private const string Base = "/projects";
        private const string BaseAdmin = "/admin/projects";
        public const string Create = $"{BaseAdmin}";
        public const string Update = $"{BaseAdmin}/{{projectId}}";
        public const string Delete = $"{BaseAdmin}/{{projectId}}";
        public const string GetProjects = $"{BaseAdmin}";
        public const string GetProjectById = $"{Base}/{{projectId}}";
        public const string GetAvailableProjectUsers = $"{Base}/{{projectId}}/users/available";
        public const string GetProjectsByUserId = $"{Base}/users/{{userId}}";
        public const string GetMyProjects = $"{Base}/me";
        public const string GetMyProjectRole = $"{Base}/{{projectId}}/my-role";
        #endregion
    }
    public static class ProjectPaper
    {
        public const string Tags = "Project Papers";

        private const string Base = "/projects";
        private const string BaseManager = "/manager/projects";

        public const string CreateProjectPaper = $"{BaseManager}/{{projectId}}/papers";
        public const string GetProjectPapers = $"{Base}/{{projectId}}/papers";
        public const string GetAvailablePapers = $"{BaseManager}/{{projectId}}/papers/available";
        public const string DeleteProjectPapers = $"{BaseManager}/{{projectId}}/papers/remove";
    }
    public static class Member
    {
        #region Constants
        public const string Tags = "Members";

        private const string Base = "/projects";
        private const string BaseAdmin = "/admin/projects";
        private const string BaseProjectManager = "/manager/projects";
        public const string AddProjectManagers    = $"{BaseAdmin}/{{projectId}}/managers";
        public const string DeleteProjectManagers = $"{BaseAdmin}/{{projectId}}/managers/remove";
        public const string AddProjectMembers    = $"{BaseProjectManager}/{{projectId}}/members";
        public const string DeleteProjectMembers = $"{BaseProjectManager}/{{projectId}}/members/remove";
        public const string GetProjectMembers    = $"{Base}/{{projectId}}/members";
        public const string UpdateProjectMemberRole = $"{BaseProjectManager}/{{projectId}}/members/{{memberId}}/role";

        #endregion
    }
    
    public static class Dataset
    {
        #region Constants
        public const string Tags = "Datasets";
        
        private const string Base = "/datasets";
        private const string BaseProjectManager = "/manager/datasets";
        public const string Create = $"{BaseProjectManager}";
        public const string Update = $"{BaseProjectManager}/{{datasetId}}";
        public const string Delete = $"{BaseProjectManager}/{{datasetId}}";
        public const string GetDatasets = $"{Base}";
        public const string GetDatasetById = $"{Base}/{{datasetId}}";
        
        #endregion
    }
}