namespace Common.Configurations;

public sealed class ApiClientCfg
{
    public static class Keycloak
    {
        #region Constants

        public const string Section = "ApiClients:Keycloak";

        public const string BaseUrl = "BaseUrl";

        public const string Realm = "Realm";

        public const string ClientId = "ClientId";

        public const string ClientSecret = "ClientSecret";

        public const string Scopes = "Scopes";

        public const string GrantType = "GrantType";

        #endregion
    }

    public static class UserService
    {
        #region Constants

        public const string Section = "ApiClients:UserService";

        public const string BaseUrl = "BaseUrl";

        #endregion
    }

    public static class LabService
    {
        #region Constants

        public const string Section = "ApiClients:LabService";

        public const string BaseUrl = "BaseUrl";

        #endregion
    }
}