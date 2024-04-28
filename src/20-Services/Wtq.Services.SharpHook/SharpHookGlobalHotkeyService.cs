﻿using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using SharpHook;
using SharpHook.Native;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Wtq.Configuration;
using Wtq.Events;
using Wtq.Services.Apps;

namespace Wtq.Services.SharpHook;

public sealed class SharpHookGlobalHotKeyService : IDisposable, IHostedService
{
	private readonly SimpleGlobalHook _hook;
	private readonly IWtqBus _bus;
	private readonly IOptionsMonitor<WtqOptions> _opts;
	private readonly IWtqAppRepo _appRepo;
	private UioHookEvent? _last;

	// TODO: Make specific to HotKey combinations.
	// private List<Func<HotKeyInfo, Task>> _registrations = [];
	public SharpHookGlobalHotKeyService(
		IOptionsMonitor<WtqOptions> opts,
		IWtqBus bus,
		IWtqAppRepo appRepo)
	{
		// _hook = new TaskPoolGlobalHook();
		// We only need keyboard events (at the moment), and mouse events cause debug sessions to be really slow.
		_hook = new SimpleGlobalHook(globalHookType: GlobalHookType.Keyboard);

		_bus = bus;
		_opts = opts;
		_appRepo = appRepo;

		_hook.KeyPressed += (s, a) =>
		{
			// Ignore repetitions.
			if (_last != null && a.RawEvent.Mask == _last.Value.Mask && a.RawEvent.Keyboard.KeyCode == _last.Value.Keyboard.KeyCode)
			{
				return;
			}

			var app = GetAppForHotKey(a.RawEvent.Mask.ToWtqKeyModifiers(), a.Data.KeyCode.ToWtqKeys());

			if (a.RawEvent.Mask == ModifierMask.LeftCtrl && a.Data.KeyCode == KeyCode.Vc2)
			{
				a.SuppressEvent = true;
				Console.WriteLine("SUPPRESS");

				// TODO: Put something in between ingesting HotKeys and publishing functional events.
				_bus.Publish(new WtqEvent()
				{
					App = app,
				});

				// var inf = new HotKeyInfo()
				// {
				// Key = WtqKeys.A,
				// Modifiers = WtqKeyModifiers.Alt,
				// };

				// foreach (var r in _registrations)
				// {
				// Task.Run(async () => await r(inf));
				// }
			}

			Console.WriteLine($"KEY PRESSED: [{a.RawEvent.Mask}] {a.Data.KeyCode}");

			_last = a.RawEvent;
		};

		_hook.KeyReleased += (s, a) =>
		{
			_last = null;
		};
	}

	public void Dispose()
	{
		_hook.Dispose();
	}

	public WtqApp? GetAppForHotKey(KeyModifiers keyMods, Keys key)
	{
		var opt = _opts.CurrentValue.Apps.FirstOrDefault(app => app.HasHotKey(key, keyMods));
		if (opt == null)
		{
			return null;
		}

		return _appRepo.GetAppByNameRequired(opt.Name);
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		new Thread(_hook.Run)
		{
			Name = "SharpHook",
		}.Start();

		return Task.CompletedTask;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		_hook.Dispose();

		return Task.CompletedTask;
	}
}