using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LangTests : MonoBehaviour {

	// Partial classes case:
	// - Class can contains method, which isn't implemented
	// - It can be private
	// - It can't return value
	// - If other part of class contains implementation, it's called
	// - In other case, it will be ignored
	
	sealed partial class SplitClass {
		public void PublicMethod() {
			Debug.Log("PublicMethod called");
			PartialMethod();
		}

		partial void PartialMethod();
	}

	
	sealed partial class SplitClass {
		partial void PartialMethod() {
			Debug.Log("PartialMethod called");
		}
	}

	struct CustomEnumIterNonGeneric /*: IEnumerator */ {
		int _pos;
		int[] _array;

		public CustomEnumIterNonGeneric(int[] array) {
			_pos = -1;
			_array = array;
		}

		public object Current => _array[_pos]; // GC allocation

		public bool MoveNext() {
			_pos++;
			return _pos < _array.Length;
		}
	}

	// Custom enumerators case

	struct CustomEnumIterGeneric /*: IEnumerator<int>*/ {
		int _pos;
		int[] _array;

		public CustomEnumIterGeneric(int[] array) {
			_pos = -1;
			_array = array;
		}

		public int Current => _array[_pos]; // No GC allocation

		public bool MoveNext() {
			_pos++;
			return _pos < _array.Length;
		}
	}

	class CustomEnumClassNonGeneric {
		int[] _array = { 1, 2, 3 };

		// Interface implementation isn't required, only MoveNext() && Current
		public CustomEnumIterNonGeneric /*IEnumerator*/ GetEnumerator() {
			return new CustomEnumIterNonGeneric(_array);
		}
	}

	class CustomEnumClassGeneric {
		int[] _array = { 1, 2, 3 };

		// Interface implementation isn't required, only MoveNext() && Current
		public CustomEnumIterGeneric /*IEnumerator*/ GetEnumerator() {
			return new CustomEnumIterGeneric(_array);
		}
	}

	void TestCustomEnumerationNonGeneric() {
		var collection = new CustomEnumClassNonGeneric();
		foreach ( var item in collection ) {
			Debug.Log(item);
		}
	}

	void TestCustomEnumerationGeneric() {
		var collection = new CustomEnumClassGeneric();
		foreach ( var item in collection ) {
			Debug.Log(item);
		}
	}

	// Interface calls case

	interface InterfaceA {
		void Method();
	}

	interface InterfaceB {
		void Method();
	}

	class CommonClass : InterfaceA, InterfaceB {
		public void Method() {
			Debug.Log("Called only on CommonClass instance");
		}

		void InterfaceA.Method() {
			Debug.Log("Called only on InterfaceA instance");
		}

		void InterfaceB.Method() {
			Debug.Log("Called only on InterfaceB instance");
		}
	}

	// Stackallock case

	void OrdinaryArrayTest() {
		var x = new int[10];
		for ( var i = 0; i < x.Length; i++ ) {
			x[i] = i;
		}
	}

	void StackallocArrayTest() {
		unsafe {
			var len = 10;
			int* x = stackalloc int[len];
			for ( var i = 0; i < len; i++ ) {
				x[i] = i;
			}
		}
	}

	// Conditional case

	[System.Diagnostics.Conditional("NOT_EXISTS_DEFINE")]
	static void DebugMethod(string message) {
		Debug.Log(message);
	}

	void ConditionalTest() {
		// Method called only if conditional define is exists
		// (removed by compiler)
		DebugMethod("direct invoke");

		// But it remains in assembly and can be called by reflection
		var type = GetType();
		var method = type.GetMethod("DebugMethod", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
		method.Invoke(null, new object[] { "reflection invoke" });
	}

	// Casting/boxing case

	void CastBoxCase() {
		byte x = 10;
		int y1 = x; // it is okay, implicit cast

		object x2 = x;
		// int y2 = (int)x2; // InvalidCast exception raised, it isn't actual cast, it is unboxing to given type and it allowed only to specific type 

		int y3 = (byte)x2; // its is okay, correct unboxing and implicit cast
	}


	void Update() {
		CastBoxCase();
	}
}
