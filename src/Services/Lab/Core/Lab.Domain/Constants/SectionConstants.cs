namespace Lab.Domain.Constants;

public static class SectionConstants
{
    public const string ReferencesTitle = "References";
    public const string ReferenceTitle  = "Reference";

    public static bool IsReferenceSection(string? title) =>
        title != null &&
        (title.Equals(ReferencesTitle, StringComparison.OrdinalIgnoreCase) ||
         title.Equals(ReferenceTitle,  StringComparison.OrdinalIgnoreCase));
}
