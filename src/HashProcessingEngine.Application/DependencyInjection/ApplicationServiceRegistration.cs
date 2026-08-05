using HashProcessingEngine.Application.Interfaces;
using HashProcessingEngine.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace HashProcessingEngine.Application.DependencyInjection;
public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IHashProcessingService, HashGenerationService>();
        services.AddScoped<IHashQueryService, HashQueryService>();

        return services;
    }
}
