using System.Reflection;
using HashProcessingEngine.Api.ExceptionHandling;
using HashProcessingEngine.Application.DependencyInjection;
using HashProcessingEngine.Application.Options;
using HashProcessingEngine.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOptions<HashBatchOptions>()
    .Bind(builder.Configuration.GetSection(HashBatchOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Hash Processing Engine API",
        Version = "v1",
        Description =
            "REST API for generating SHA1 hashes, " +
            "publishing them through RabbitMQ, " +
            "and retrieving processing statistics from MariaDB."
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    options.IncludeXmlComments(xmlPath);
});

builder.Services
    .AddOptions<HashGenerationOptions>()
    .Bind(builder.Configuration.GetSection(HashGenerationOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services
    .AddOptions<RabbitMqOptions>()
    .Bind(builder.Configuration.GetSection(RabbitMqOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandler>();

app.UseAuthorization();

app.MapControllers();

app.Run();