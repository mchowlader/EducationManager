using EduManager.Application.DTOs.Feature.TenantFeature;
using EduManager.Domain.Common;
using MediatR;

namespace EduManager.Application.Features.TenantFeature.Commands;

public record DeleteTenantCommand(long id) : IRequest<Result<ApiResponse<object>>>;