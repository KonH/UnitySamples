using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrayTest : MonoBehaviour {

	void Start () {
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
		// TODO
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
