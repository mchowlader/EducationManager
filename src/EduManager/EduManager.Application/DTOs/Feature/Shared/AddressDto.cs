namespace EduManager.Application.DTOs.Feature.Shared;

public record AddressDto(
    string Division,
    string District,
    string Thana,
    string City,
    string PostalCode
);