using NUnit.Framework;
using System.Collections.Generic;
using UnityBenchmarkHarness;

public class ListBenchmarks {

	[Test]
	public void Create_Benchmark() {
		// Rewrite later
		/*
		var hold = new List<int>();
		BenchmarkRunner.Run(
			"CreateList",
			10,
			(count) => {
				var list = new List<int>(count);
				hold = list;
			},
			50 * 100,
			500 * 100,
			50 * 100 * 100
		).WriteToConsole();

		hold.Clear();*/
	}

	[Test]
	public void Add_Benchmark() {
		// Rewrite later
		/*BenchmarkRunner.Run(
			"AddToList",
			10,
			(count) => {
				var list = new List<int>();
				for ( var i = 0; i < count; i++ ) {
					list.Add(i);
				}
			},
			50 * 100,
			500 * 100,
			50 * 100 * 100
		).WriteToConsole();*/
	}
}