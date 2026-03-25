using EduManager.Application.Behaviors;
using EduManager.Application.Common.Mappings;
using EduManager.Application.Features.Base.Commands;
using EduManager.Application.Features.Base.Queries;
using EduManager.Domain.Common;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace EduManager.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });

        services.AddAutoMapper(cfg =>
        {
            cfg.LicenseKey = "eyJhbGciOiJSUzI1NiIsImtpZCI6Ikx1Y2t5UGVubnlTb2Z0d2FyZUxpY2Vuc2VLZXkvYmJiMTNhY2I1OTkwNGQ4OWI0Y2IxYzg1ZjA4OGNjZjkiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2x1Y2t5cGVubnlzb2Z0d2FyZS5jb20iLCJhdWQiOiJMdWNreVBlbm55U29mdHdhcmUiLCJleHAiOiIxODA0OTgyNDAwIiwiaWF0IjoiMTc3MzQ5Mzk3MCIsImFjY291bnRfaWQiOiIwMTljZWM3OWJjOWY3OGY1YTg1MTVjZGFmNjVmZDI5NyIsImN1c3RvbWVyX2lkIjoiY3RtXzAxa2twN21zcmJkczdnbXBnenZwY2s4NzF5Iiwic3ViX2lkIjoiLSIsImVkaXRpb24iOiIwIiwidHlwZSI6IjIifQ.ZZ1PxMBINyapH8Tams1MYtE0o72yHv0DBLQmzWplU0N8i_h-nSWmwTdCtKzntHUXR4kvm6pU4YMFK2ks-ld6sfp3Z0r8z4YLfraAEFpXhmd28jlFnLUE2kOsRjNqopH_Lv3d9LTLA9MGn4HbeWZ-ZKHUMvS37JvdCsLB6ngmEeUPSg4y6EqqqO_s8VE0cCaCbKGHk7L5qiog_8UlM6EcyPt_qtdx7nKrp77-Qn2xxdMylc8mA4qihkUCEjwvnt17BeTu9i7X1hQBTtKBXMAi9cs0SUw_dRbk5zof7chgu4gnTTc6uJZUDuzio9_LVSuf0XunVfDvZpWVSZa8hZaw2A";
        },
        typeof(DependencyInjection).Assembly);

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        RegisterBaseHandlersFromEndpoints(services);

        return services;
    }

    private static void RegisterBaseHandlersFromEndpoints(IServiceCollection services)
    {
        var endpointTypes = typeof(DependencyInjection).Assembly
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.BaseType?.IsGenericType == true
                && t.BaseType.GetGenericTypeDefinition() == typeof(BaseProfile<,,,>));

        foreach (var endpointType in endpointTypes)
        {
            var args = endpointType.BaseType!.GetGenericArguments();
            var entity = args[0];
            var createDto = args[1];
            var updateDto = args[2];
            var responseDto = args[3];

            // GetAll
            services.AddScoped(
                typeof(IRequestHandler<,>).MakeGenericType(
                    typeof(BaseGetAllQuery<,>).MakeGenericType(entity, responseDto),
                    typeof(Result<>).MakeGenericType(typeof(IEnumerable<>).MakeGenericType(responseDto))),
                typeof(BaseGetAllQueryHandler<,>).MakeGenericType(entity, responseDto));

            // GetById
            services.AddScoped(
                typeof(IRequestHandler<,>).MakeGenericType(
                    typeof(BaseGetByIdQuery<,>).MakeGenericType(entity, responseDto),
                    typeof(Result<>).MakeGenericType(responseDto)),
                typeof(BaseGetByIdQueryHandler<,>).MakeGenericType(entity, responseDto));

            // Create
            services.AddScoped(
                typeof(IRequestHandler<,>).MakeGenericType(
                    typeof(BaseCreateCommand<,,>).MakeGenericType(entity, createDto, responseDto),
                    typeof(Result<>).MakeGenericType(responseDto)),
                typeof(BaseCreateCommandHandler<,,>).MakeGenericType(entity, createDto, responseDto));

            // Update
            services.AddScoped(
                typeof(IRequestHandler<,>).MakeGenericType(
                    typeof(BaseUpdateCommand<,,>).MakeGenericType(entity, updateDto, responseDto),
                    typeof(Result<>).MakeGenericType(responseDto)),
                typeof(BaseUpdateCommandHandler<,,>).MakeGenericType(entity, updateDto, responseDto));

            // Delete
            services.AddScoped(
                typeof(IRequestHandler<,>).MakeGenericType(
                    typeof(BaseDeleteCommand<>).MakeGenericType(entity),
                    typeof(Result<bool>)),
                typeof(BaseDeleteCommandHandler<>).MakeGenericType(entity));
        }
    }
}