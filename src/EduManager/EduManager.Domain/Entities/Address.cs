using EduManager.Domain.Common;

namespace EduManager.Domain.Entities;

public class Address
{
    public string Division { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string Thana { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
}
