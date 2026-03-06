namespace EduManager.Domain.Common;

public static class ErrorCodeGenerator
{
    public static string Generate()
    {
        var date = DateTime.UtcNow.ToString("yyyyMMdd");
        var random = Guid.NewGuid().ToString("N")[..6].ToUpper();
        return $"ERR-{date}-{random}";
    }
}
