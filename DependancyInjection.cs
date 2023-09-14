#if NET
using Microsoft.Extensions.DependencyInjection;

namespace _3ai.solutions.Translator;

public static class DependencyInjection
{
    public static IServiceCollection AddTranslationService<T>(this IServiceCollection services) where T : ITranslationRepository
    {
        services.AddScoped<ITranslationRepository>(sp => sp.GetRequiredService<T>());
        services.AddScoped<TranslationService>();
        return services;
    }
}
#endif
