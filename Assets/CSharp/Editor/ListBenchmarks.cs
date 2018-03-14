using NUnit.Framework;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using UnityEngine.TestTools;
using System.Collections;
using UnityEngine;

public class ListBenchmarks {

	class SimpleBenchmark : IDisposable {
		string _name;
		int _iters;
		Stopwatch _sw;
		long _memoryBefore;

		public SimpleBenchmark(string name, int iters) {
			_name = name;
			_iters = iters;
			_sw = new Stopwatch();
			_memoryBefore = GC.GetTotalMemory(false);

			_sw.Start();
		}

		public void Dispose() {
			_sw.Stop();
			var memUsed = GC.GetTotalMemory(false) - _memoryBefore;
			var msg = string.Format(
				"{0} ({1:N0}) = time: {2:N0}, memory: {3:N0}",
				_name, _iters, _sw.ElapsedTicks, memUsed
			);
			UnityEngine.Debug.Log(msg);
		}
	}

	[Test]
	public void TestAdd() {
		TestAdd<List<int>>(
			50 * 100,
			500 * 100,
			50 * 100 * 100);
	}

	void TestWrapper<T>(string name, Action<T, int> payload, params int[] iterations) where T : IList<int>, new() {
		for ( var step = 0; step < iterations.Length; step++ ) {
			var curIters = iterations[step];
			var list = new T();
			GC.Collect();
			using ( new SimpleBenchmark($"{name}<{list.GetType().Name}>", curIters) ) {
				payload(list, curIters);
			}
		}
	}

	void TestAdd<T>(params int[] iterations) where T : IList<int>, new() {
		TestWrapper<T>("TestAdd", (list, curIters) => {
			for ( var i = 0; i < curIters; i++ ) {
				list.Add(i);
			}
		}, iterations);
	}
}
