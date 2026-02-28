using AutoMapper;
using EduManager.Application.Features.Base.Commands;
using EduManager.Application.Features.Base.Queries;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace EduManager.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        services.AddAutoMapper(typeof(DependencyInjection).Assembly); 

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        // Generic Handlers register
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(BaseCreateCommandHandler<,,>).Assembly));
        //services.AddScoped(typeof(IRequestHandler<,>), typeof(BaseGetAllQueryHandler<,>));
        //services.AddScoped(typeof(IRequestHandler<,>), typeof(BaseGetByIdQueryHandler<,>));
        //services.AddScoped(typeof(IRequestHandler<,>), typeof(BaseCreateCommandHandler<,,>));
        //services.AddScoped(typeof(IRequestHandler<,>), typeof(BaseUpdateCommandHandler<,,>));
        //services.AddScoped(typeof(IRequestHandler<,>), typeof(BaseDeleteCommandHandler<>));

        return services;
    }
}
