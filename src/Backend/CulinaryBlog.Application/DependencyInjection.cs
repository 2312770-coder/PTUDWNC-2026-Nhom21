using System.Reflection;
using CulinaryBlog.Application.Common.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CulinaryBlog.Application;

// Đăng ký toàn bộ service của tầng Application.
// Program.cs chỉ cần gọi builder.Services.AddApplication().
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // MediatR tự quét và đăng ký mọi IRequestHandler trong assembly này.
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        // FluentValidation tự quét mọi AbstractValidator.
        services.AddValidatorsFromAssembly(assembly);

        // Pipeline Behaviors - THỨ TỰ ĐĂNG KÝ CHÍNH LÀ THỨ TỰ CHẠY,
        // khớp SRS mục 6.3:
        //   1. Logging  -> 2. Validation -> 3. Caching
        //   -> 4. Handler -> 5. CacheInvalidation
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CacheInvalidationBehavior<,>));

        return services;
    }
}
