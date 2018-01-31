using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrayTest : MonoBehaviour {

	void Start () {
		//SingleDimArray();
		//MultiDimArray();
	}

	void SingleDimArray() {
		for ( int i = 100; i < 10000000; i = i * 10 ) {
			TestHasItem(i);
		}
	}

	void TestHasItem(int size) {
		var array = new int[size];
		for ( int i = 0; i < array.Length - 1; i++ ) {
			int newValue = 0;
			while ( newValue == 0 ) {
				newValue = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
			}
			array[i] = newValue;
		}
		// Badest cause (latest item is 0)
		var value = 0;
		var sw = new System.Diagnostics.Stopwatch();
		sw.Start();
		var forResult = Retry(() => ForFind(array, value), 1000);
		sw.Stop();
		var forTicks = sw.ElapsedTicks;
		sw.Restart();
		Array.Sort(array);
		var binaryResult = Retry(() => BinaryFind(array, value), 1000);
		sw.Stop();
		var binaryTicks = sw.ElapsedTicks;
		Debug.Log($"[{array.Length}] Result: For: {forResult} {forTicks}, BinarySearch: {binaryResult} {binaryTicks} (ratio: {(float)forTicks/binaryTicks})");
	}

	bool Retry(Func<bool> func, int count) {
		var result = false;
		for ( int i = 0; i < count; i++ ) {
			result = func();
		}
		return result;
	}

	bool ForFind(int[] array, int value) {
		for ( int i = 0; i < array.Length; i++ ) {
			if ( array[i] == value ) {
				return true;
			}
		}
		return false;
	}

	bool BinaryFind(int[] array, int value) {
		return Array.BinarySearch(array, value) >= 0;
	}

	void MultiDimArray() {
		var array = new int[4, 3, 2];
		Debug.Log($"Length: {array.Length}, Rank: {array.Rank}");
		for ( int i = 0; i < array.Rank; i++ ) {
			Debug.Log($"GetLength({i}): {array.GetLength(i)}");
		}
		var v = 0;
		for ( int x = 0; x < array.GetLength(0); x++ ) {
			for ( int y = 0; y < array.GetLength(1); y++ ) {
				for ( int z = 0; z < array.GetLength(2); z++ ) {
					array[x, y, z] = v;
					v++;
				}
			}
		}
		PrintAnyArray(array);
	}

	void PrintAnyArray(Array array) {
		var index = new int[array.Rank];
		var done = false;
		while ( !done ) {
			Debug.Log($"[{GetIndexStr(index)}] = {array.GetValue(index)}");
			done = !FindNextIndex(array, index);
		}
	}

	bool FindNextIndex(Array array, int[] index) {
		for ( int i = index.Length - 1; i >= 0; i-- ) {
			index[i]++;
			if ( index[i] >= array.GetLength(i) ) {
				index[i] = 0;
			} else {
				return true;
			}
		}
		return false;
	}

	string GetIndexStr(int[] index) {
		var s = "";
		for ( int i = 0; i < index.Length; i++ ) {
			if(i > 0) {
				s += ", ";
			}
			s += index[i];
		}
		return s;
	}
}
