﻿namespace Wtq.Core.Configuration;

public enum PreferMonitor
{
	/// <summary>
	/// The monitor where the mouse cursor is currently at.
	/// </summary>
	WithCursor = 0,

	/// <summary>
	/// The monitor at the index specified as specified by "MonitorIndex" (0-based).
	/// </summary>
	AtIndex,

	/// <summary>
	/// The monitor considered "primary" by the OS.
	/// </summary>
	Primary,
}