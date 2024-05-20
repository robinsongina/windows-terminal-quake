﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wtq.Events;
using Wtq.Services;
using Wtq.Services.Apps;
using Microsoft.Extensions.Hosting;

namespace Wtq;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddSingletonHostedService<TService, TImplementation>(this IServiceCollection services)
		where TService : class
		where TImplementation : class, TService, IHostedService
	{
		Guard.Against.Null(services);

		services.AddSingleton<TService, TImplementation>();

		services.AddHostedService(p => (TImplementation)p.GetRequiredService<TService>());

		return services;
	}

	public static IServiceCollection AddWtqCore(this IServiceCollection services)
	{
		Guard.Against.Null(services);

		return services

			// Utils
			.AddSingleton<IRetry, Retry>()
			.AddSingleton<IWtqTween, WtqTween>()

			// Core App Logic
			.AddSingleton<IWtqAppToggleService, WtqAppToggleService>()
			.AddSingleton<IWtqBus, WtqBus>()
			.AddSingleton<IWtqAppRepo, WtqAppRepo>()
			.AddSingleton<IWtqProcessFactory, WtqProcessFactory>()
			.AddSingletonHostedService<IWtqHotKeyService, WtqHotKeyService>()
			.AddSingletonHostedService<IWtqFocusTracker, WtqFocusTracker>()

			.AddHostedService<WtqService>()

			.AddSingleton<WtqAppMonitorService>()
			.AddHostedService(p => p.GetRequiredService<WtqAppMonitorService>());
	}
}