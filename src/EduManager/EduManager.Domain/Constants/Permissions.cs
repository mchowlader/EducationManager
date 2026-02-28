namespace EduManager.Domain.Constants;

public class Permissions
{
    public const string View = "View";
    public const string Create = "Create";
    public const string Update = "Update";
    public const string Delete = "Delete";

    public static string For(string entiry, string action) => $"{entiry}.{action}";
}
