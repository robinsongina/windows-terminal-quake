﻿using Wtq.Core.Data;

namespace Wtq.Core.Configuration;

public class WtqAppOptions
{
	public AttachMode? AttachMode { get; set; }

	// TODO: Use dict key?
	[NotNull]
	[Required]
	public string? Name { get; set; }

	public IEnumerable<HotKeyOptions> HotKeys { get; set; } = [];

	[NotNull]
	[Required]
	public string? FileName { get; set; }

	public string? ProcessName { get; set; }

	public string? Arguments { get; set; }

	/// <summary>
	/// <para>If "PreferMonitor" is set to "AtIndex", this setting determines what monitor to choose.</para>
	/// <para>Zero based, eg. 0, 1, etc.</para>
	/// <para>Defaults to "0".</para>
	/// </summary>
	public int? MonitorIndex { get; set; }

	/// <summary>
	/// <para>What monitor to preferrably drop the terminal.</para>
	/// <para>"WithCursor" (default), "Primary" or "AtIndex".</para>
	/// </summary>
	public PreferMonitor? PreferMonitor { get; set; }

	/// <summary>
	/// <para>Make the window see-through (applies to the entire window, including the title bar).</para>
	/// <para>0 (invisible) - 100 (opaque).</para>.
	/// <para>Defaults to "100".</para>
	/// </summary>
	public int? Opacity { get; set; }

	/// <summary>
	/// <para>When to show the terminal window icon on the taskbar.</para>
	/// <para>"AlwaysHidden", "AlwaysVisible" or "WhenTerminalVisible".</para>
	/// <para>Defaults to "AlwaysHidden".</para>
	/// </summary>
	public TaskBarIconVisibility? TaskbarIconVisibility { get; set; }

	#region Sizes

	public HorizontalAlign? HorizontalAlign { get; set; }

	/// <summary>
	/// <para>How much room to leave between the top of the terminal and the top of the screen, in pixels.</para>
	/// <para>Defaults to "0".</para>
	/// </summary>
	public int? VerticalOffset { get; set; }

	/// <summary>
	/// <para>Vertical screen coverage as a percentage (0-100).</para>
	/// <para>Defaults to "100".</para>
	/// </summary>
	public float? VerticalScreenCoverage { get; set; }

	/// <summary>
	/// <para>Horizontal screen coverage, as a percentage.</para>
	/// <para>Defaults to "100".</para>
	/// </summary>
	public float? HorizontalScreenCoverage { get; set; }

	#endregion Sizes

	public bool Filter(Process process, bool isStartedByWtq)
	{
		ArgumentNullException.ThrowIfNull(process);

		if (process.MainWindowHandle == nint.Zero)
		{
			return false;
		}

		// TODO: Make some notes about webbrowsers being a pain with starting a new process.
		try
		{
			if (process.MainModule == null)
			{
				return false;
			}

			if (process.MainWindowHandle == nint.Zero)
			{
				return false;
			}

			// TODO: Handle extensions either or not being there.
			var fn1 = Path.GetFileNameWithoutExtension(ProcessName ?? FileName);
			var fn2 = Path.GetFileNameWithoutExtension(process.MainModule.FileName);

			if (fn2.Contains("windowsterminal", StringComparison.OrdinalIgnoreCase))
			{
				var dbg = 2;
			}

			if (!fn1.Equals(fn2, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}

			if (isStartedByWtq)
			{
				if (!process.StartInfo.Environment.TryGetValue("WTQ_START", out var val))
				{
					return false;
				}

				if (string.IsNullOrWhiteSpace(val))
				{
					return false;
				}

				if (!val.Equals(Name, StringComparison.OrdinalIgnoreCase))
				{
					return false;
				}
			}

			return true;
		}
		catch
		{
			// TODO: Remove try/catch, use safe property access methods on "Process" instead.
		}

		return false;
	}

	public bool HasHotKey(WtqKeys key, WtqKeyModifiers modifiers)
	{
		return HotKeys.Any(hk => hk.Key == key && hk.Modifiers == modifiers);
	}

	public override string ToString()
	{
		//return FindExisting?.ProcessName ?? StartNew?.FileName ?? "<unknown>";
		return Name;
	}
}