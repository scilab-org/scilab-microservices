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
        public const string AddPaperProject = $"{Base}/{{projectId}}/papers";
        public const string GetSubProjects = $"{Base}/{{parentProjectId}}/subprojects";
        
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