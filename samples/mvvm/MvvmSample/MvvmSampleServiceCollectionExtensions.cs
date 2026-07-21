using Microsoft.Extensions.DependencyInjection;
using MvvmSample.Example;

namespace MvvmSample;

/// <summary>DI registration for the sample.</summary>
public static class MvvmSampleServiceCollectionExtensions
{
    /// <summary>
    /// Register the sample's services and view models. View models are <b>transient</b> — one per
    /// component instance, matching the component's own lifetime. Call from Program.cs:
    /// <c>builder.Services.AddMvvmSample();</c>
    /// </summary>
    public static IServiceCollection AddMvvmSample(this IServiceCollection services)
    {
        services.AddSingleton<IGreetingService, GreetingService>();
        services.AddTransient<SampleViewModel>();
        return services;
    }
}
