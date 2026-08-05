using HashProcessingEngine.Application.DependencyInjection;
using HashProcessingEngine.Application.Options;
using HashProcessingEngine.Infrastructure.DependencyInjection;
using HashProcessingEngine.Worker.Consumers;
using HashProcessingEngine.Worker.Options;
using HashProcessingEngine.Worker.Services;
using System;
using System.Collections.Generic;
using System.Text;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<WorkerOptions>(builder.Configuration.GetSection(WorkerOptions.SectionName));

builder.Services
    .AddOptions<RabbitMqOptions>()
    .Bind(
        builder.Configuration.GetSection(
            RabbitMqOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddApplication();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddSingleton<HashMessageChannel>();

builder.Services.AddHostedService<RabbitMqHashConsumer>();

builder.Services.AddHostedService<HashWorkerService>();

var host = builder.Build();

host.Run();