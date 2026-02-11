using System;
using System.Collections.Generic;

namespace GraphProcessor
{
	/// <summary>
	/// Central registry for pinned panel views. Used by ToolbarView and BaseGraphView to add menu/toolbar entries.
	/// </summary>
	public static class PinnedViewRegistry
	{
		public struct Entry
		{
			public Type		viewType;
			public string	displayName;

			public string MenuLabel => "View/" + displayName;
			public string ToolbarLabel => "Show " + displayName;
		}

		static readonly Entry[] entries = new[]
		{
			new Entry { viewType = typeof(ProcessorView), displayName = "Processor" },
			new Entry { viewType = typeof(ExposedParameterView), displayName = "Parameters" },
		};

		public static IReadOnlyList<Entry> Entries => entries;
	}
}
