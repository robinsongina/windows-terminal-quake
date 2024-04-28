﻿using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace Wtq.Services.SimpleTrayIcon;

public class SimpleTrayIconService : IHostedService
{
	public Task StartAsync(CancellationToken cancellationToken)
	{
		// var icon = new System.Drawing.Icon(typeof(Program), "WtqS.Demo.tray-icon.ico"); // Load an icon for the tray
		// var icon = Resources.icon;
		// using var menu = new TrayMenu(icon, "Tooltip", true);
		// var item = new TrayMenuItem { Content = "Item1" };
		// menu.Items.Add(item);
		// menu.Items.Add(new TrayMenuSeparator());
		// menu.Items.Add(new TrayMenuItem { Content = $"some content", IsChecked = true });
		// item.Click += (s, e) => ((TrayMenuItem)s).IsChecked = !((TrayMenuItem)s).IsChecked; // Attach an event

		// _ = Task.Factory.StartNew(
		// () =>
		// {
		// User32.RunLoop(_cts.Token); // Runs the message pump. Winforms, WPF, etc., will do this for you.
		// },
		// TaskCreationOptions.RunContinuationsAsynchronously);
		return Task.CompletedTask;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		// await _cts.CancelAsync().ConfigureAwait(false);
		return Task.CompletedTask;
	}
}