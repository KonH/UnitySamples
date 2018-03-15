using NUnit.Framework;
using System.Collections.Generic;

public class ListBenchmarks {

	[Test]
	public void Add_Benchmark() {
		TestAdd<List<int>>(
			50 * 100,
			500 * 100,
			50 * 100 * 100)
			.WriteToConsole();
	}

	List<BenchmarkSummary> TestAdd<T>(params int[] iterations) where T : IList<int>, new() {
		return BenchmarkSummary.TestWrapper<T>("TestAdd", 10, (list, curIters) => {
			for ( var i = 0; i < curIters; i++ ) {
				list.Add(i);
			}
		}, iterations);
	}
}
