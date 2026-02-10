namespace User.Api.Constants;

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

    public static class Users
    {
        #region Constants

        public const string Tags = "Users";

        private const string Base = "/users";

        public const string Create = Base;

        public const string GetAll = Base;

        public const string GetById = $"{Base}/{{userId}}";

        public const string Update = $"{Base}/{{userId}}";

        public const string Deactivate = $"{Base}/{{userId}}/deactivate";

        #endregion
    }

    public static class Groups
    {
        #region Constants

        public const string Tags = "Groups";

        private const string Base = "/groups";

        public const string GetAll = Base;

        #endregion
    }

    public static class Roles
    {
        #region Constants

        public const string Tags = "Roles";

        private const string Base = "/roles";

        public const string GetAll = Base;

        public const string GetGroupRoles = "/groups/{groupId}/roles";

        public const string AddToGroup = "/groups/{groupId}/roles";

        public const string RemoveFromGroup = "/groups/{groupId}/roles";

        #endregion
    }
}
