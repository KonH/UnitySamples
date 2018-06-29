using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LangThings : MonoBehaviour {

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

	void Update() {
		var cc = new CommonClass();
		cc.Method();
		(cc as InterfaceA).Method();
		(cc as InterfaceB).Method();
	}
}
