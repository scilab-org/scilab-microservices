namespace Common.Constants;

public sealed class AppConstants
{
    #region Common

    public const int MaxAttempts = 3;

    #endregion

    #region Bucket

    public static class Bucket
    {
        public const string Datasets = "datasets";
    }

    #endregion
    
    #region Service

    public static class Service
    {
        public const string Lab = "lab";
    }

    #endregion

    #region File Content Type

    public static class FileContentType
    {
        public const string OctetStream = "application/octet-stream";
    }

    #endregion
}