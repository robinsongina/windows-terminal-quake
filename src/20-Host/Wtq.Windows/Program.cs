﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.IO;
using System.Threading.Tasks;
using Wtq.Configuration;
using Wtq.Core;
using Wtq.Core.Service;
using Wtq.Core.Services;
using Wtq.Services;
using Wtq.Services.AnimationTypeProviders;
using Wtq.Services.ScreenBoundsProviders;
using Wtq.Services.TerminalBoundsProviders;
using Wtq.Utils;
using Wtq.Win32;
using Wtq.Windows;
using Wtq.WinForms;

namespace Wtq;

public sealed class Program
{
	private readonly IHost _host;
	private readonly Microsoft.Extensions.Logging.ILogger _log;

	public Program()
	{
		//Kernel32.AllocConsole();

		Console.WriteLine("Hello, World!");

		// Configuration.
		var config = new ConfigurationBuilder()
			.SetBasePath(App.PathToAppDir)
			.AddJsonFile(f =>
			{
				var path = Path.GetFileName(App.PathToAppConf);

				f.Optional = false;
				f.Path = path;
				f.OnLoadException = x =>
				{
					// TODO: Logging and configuration are currently kinda dependent on one another.
					//_log.LogError(x.Exception, "Error loading configuration file '{File}': {Message}", path, x.Exception.Message);
					Console.WriteLine($"Error loading configuration file '{path}': {x.Exception.Message}");
				};
			})
			.Build();

		// Logging.
		Utils.Log.Configure(config);

		_log = Wtq.Utils.Log.For(typeof(Program));

		_host = new HostBuilder()
			.ConfigureAppConfiguration(opt =>
			{
				opt.AddConfiguration(config);
			})
			.ConfigureServices(opt =>
			{
				opt
					.AddOptionsWithValidateOnStart<WtqOptions>()
					.Bind(config);

				opt
					// Utils
					.AddSingleton<IRetry, Retry>()

					// Core App Logic
					.AddSingleton<IAnimationProvider, AnimationProvider>()
					.AddSingleton<IScreenBoundsProvider, ScreenWithCursorScreenBoundsProvider>()
					.AddSingleton<ITerminalBoundsProvider, MovingTerminalBoundsProvider>()
					.AddSingleton<IWtqProcessFactory, WtqProcessFactory>()
					.AddSingleton<IWtqAppToggleService, WtqAppToggleService>()
					.AddSingleton<WtqAppMonitorService>()
					.AddSingleton<IWtqBus, WtqBus>()
					.AddHostedService(p => p.GetRequiredService<WtqAppMonitorService>())
					.AddHostedService<WtqService>()
					.AddSingleton<IWtqAppRepo, WtqAppRepo>()
					.AddHostedService<WtqHotkeyService>()

					.AddSingletonHostedService<IWtqFocusTracker, WtqFocusTracker>()

					// Platform-specific.
					.AddWin32ProcessService()
					.AddWinFormsScreenCoordsProvider()
					.AddWinFormsHotkeyService()
					.AddWinFormsTrayIcon()
					//.AddSharpHookGlobalHotkeys()
					//.AddSimpleTrayIcon()
					;
			})
			.UseSerilog()
			.Build();
	}

	public async Task RunAsync()
	{
		try
		{
			await _host
				.RunAsync()
				.ConfigureAwait(false);
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error running application: {ex}");
		}
	}

	public static async Task Main(string[] args)
	{
		await new Program().RunAsync();
	}
}