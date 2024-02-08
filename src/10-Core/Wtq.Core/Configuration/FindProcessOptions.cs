﻿namespace Wtq.Configuration;

public class FindProcessOptions
{
	public string? ProcessName { get; set; }

	public bool Filter(Process process)
	{
		if (process.MainWindowHandle == IntPtr.Zero)
		{
			return false;
		}

		if (!string.IsNullOrWhiteSpace(ProcessName))
		{
			return process.ProcessName.Equals(ProcessName, StringComparison.OrdinalIgnoreCase);
		}

		return false;
	}
}