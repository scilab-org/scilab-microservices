namespace User.Application.Models.Filters;

public sealed record GetUsersFilter(
    string? SearchText,
    string? GroupName = null);
