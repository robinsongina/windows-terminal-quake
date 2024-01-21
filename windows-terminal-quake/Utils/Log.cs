﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;
using System.IO;

namespace Wtq.Utils;

public static class Log
{
	private static ILoggerFactory _factory;

	public static void Configure(IConfiguration configuration)
	{
		var here = Path.GetDirectoryName(new Uri(typeof(Log).Assembly.Location).LocalPath);

		Serilog.Log.Logger = new LoggerConfiguration()
			//.MinimumLevel.Is(QSettings.Instance.LogLevel)

			.WriteTo.Console()

			.WriteTo.File(
				path: Path.Combine(here, "logs/.txt"),
				fileSizeLimitBytes: 10_000_000,
				rollingInterval: RollingInterval.Day,
				retainedFileCountLimit: 3)
			.CreateLogger();

		var provider = new SerilogLoggerProvider(Serilog.Log.Logger);
		_factory = new SerilogLoggerFactory(Serilog.Log.Logger);
		_factory.AddProvider(provider);
	}

	public static Microsoft.Extensions.Logging.ILogger For<T>()
	{
		return _factory.CreateLogger<T>();
	}

	public static Microsoft.Extensions.Logging.ILogger For(Type type)
	{
		return _factory.CreateLogger(type);
	}

	public static Microsoft.Extensions.Logging.ILogger For(string category)
	{
		return _factory.CreateLogger(category);
	}
}