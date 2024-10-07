using System.Collections.Generic;
using System.Diagnostics;

namespace RG_GameCamera.Utils;

public static class Profiler
{
	private static readonly Dictionary<string, Stopwatch> timeSegments = new Dictionary<string, Stopwatch>();

	public static void Start(string key)
	{
		Stopwatch value = null;
		if (timeSegments.TryGetValue(key, out value))
		{
			value.Reset();
			value.Start();
		}
		else
		{
			value = new Stopwatch();
			value.Start();
			timeSegments.Add(key, value);
		}
	}

	public static void Stop(string key)
	{
		timeSegments[key].Stop();
	}

	public static string[] GetResults()
	{
		string[] array = new string[timeSegments.Count];
		int num = 0;
		foreach (KeyValuePair<string, Stopwatch> timeSegment in timeSegments)
		{
			long elapsedMilliseconds = timeSegment.Value.ElapsedMilliseconds;
			long num2 = timeSegment.Value.ElapsedTicks / (Stopwatch.Frequency / 1000000);
			array[num++] = timeSegment.Key + " " + elapsedMilliseconds + " [ms] | " + num2 + " [us]";
		}
		return array;
	}
}
