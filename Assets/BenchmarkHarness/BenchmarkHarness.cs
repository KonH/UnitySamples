using System;
using System.Collections.Generic;
using System.Diagnostics;

public static class BenchmarkSummaryExtensions {
	public static void WriteToConsole(this List<BenchmarkSummary> summaries) {
		foreach ( var s in summaries ) {
			var str = $"{s.Name}\n";
			foreach ( var m in s.Measures ) {
				str += string.Format(
					"{0}: [{1:N}-{2:N}], avg. {3:N}\n",
					m.Name, m.Min, m.Max, m.Avg);
			}
			UnityEngine.Debug.Log(str);
		}
	}
}

class BenchmarkResult {
	public double Time   { get; }
	public long   Memory { get; }

	public BenchmarkResult(double time, long memory) {
		Time   = time;
		Memory = memory;
	}
}

public class BenchmarkMeasure {
	public string Name  { get; }
	public double Min   { get; }
	public double Max   { get; }
	public double Avg   { get; }

	public BenchmarkMeasure(string name, double min, double max, double avg) {
		Name = name;
		Min  = min;
		Max  = max;
		Avg  = avg;
	}
}

public class BenchmarkSummary {
	public string                 Name     { get; }
	public List<BenchmarkMeasure> Measures { get; }

	public BenchmarkSummary(string name, List<BenchmarkMeasure> measures) {
		Name     = name;
		Measures = measures;
	}

	public static List<BenchmarkSummary> TestWrapper<T>(string name, int repeats, Action<T, int> payload, params int[] iterations) where T : new() {
		var summaries = new List<BenchmarkSummary>();
		for ( var step = 0; step < iterations.Length; step++ ) {
			var curIters = iterations[step];
			var fullName = $"{name}<{(new T()).GetType().Name}>x{repeats}";
			var results = new List<BenchmarkResult>(repeats);
			for ( var r = 0; r < repeats; r++ ) {
				var list = new T();
				results.Add(BenchmarkRunner.Perform(() => payload(list, curIters)));
			}
			summaries.Add(BenchmarkSummary.CombineResults(fullName, results));
		}
		return summaries;
	}

	static BenchmarkSummary CombineResults(string name, ICollection<BenchmarkResult> results) {
		var measures = new List<BenchmarkMeasure>() {
			CreateTimeMeause(results),
			CreateMemMeasure(results)
		};
		return new BenchmarkSummary(name, measures);
	}

	static BenchmarkMeasure CreateMeause(string name, Func<BenchmarkResult, double> getter, ICollection<BenchmarkResult> results) {
		double min = double.MaxValue, max = 0, sum = 0;
		foreach ( var r in results ) {
			var cur = getter(r);
			if ( cur < min ) {
				min = cur;
			}
			if ( cur > max ) {
				max = cur;
			}
			sum += cur;
		}
		var avg = sum / results.Count;
		return new BenchmarkMeasure(name, min, max, avg);
	}

	static BenchmarkMeasure CreateTimeMeause(ICollection<BenchmarkResult> results) {
		return CreateMeause("Time", r => r.Time, results);
	}

	static BenchmarkMeasure CreateMemMeasure(ICollection<BenchmarkResult> results) {
		return CreateMeause("Memory", r => r.Memory, results);
	}
}

class BenchmarkRunner {
	Stopwatch _timer;
	long      _startMem;

	public static BenchmarkRunner Start() {
		GC.Collect();
		return new BenchmarkRunner() {
			_startMem = GC.GetTotalMemory(false),
			_timer    = Stopwatch.StartNew()
		};
	}

	public static BenchmarkResult Perform(Action action) {
		var b = Start();
		action();
		return b.Finish();
	}

	public BenchmarkResult Finish() {
		_timer.Stop();
		var memUsed = GC.GetTotalMemory(false) - _startMem;
		return new BenchmarkResult(_timer.Elapsed.TotalMilliseconds, memUsed);
	}
}
