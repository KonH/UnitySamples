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
		Debug.Log(y1);

		object x2 = x;
		// int y2 = (int)x2; // InvalidCast exception raised, it isn't actual cast, it is unboxing to given type and it allowed only to specific type 
		Debug.Log(x2);

		int y3 = (byte)x2; // its is okay, correct unboxing and implicit cast
		Debug.Log(y3);
	}

	// Num representation case

	void WriteBits(int x) {
		Debug.LogFormat("{0} = {1}", x, System.Convert.ToString(x, 2).PadLeft(32, '0'));
	}

	void NumsTest() {
		Debug.Log(sizeof(int)); // 4

		WriteBits(int.MinValue); // 10000000_00000000_00000000_00000000
		WriteBits(0           ); // 00000000_00000000_00000000_00000000
		WriteBits(int.MaxValue); // 01111111_11111111_11111111_11111111

		var x = 10;
		WriteBits(x);            // 00000000_00000000_00000000_00001010

		var y = -10;
		WriteBits(y);            // 11111111_11111111_11111111_11110110

		var y2 = ~x + 1;
		WriteBits(y2);           // 11111111_11111111_11111111_11110110

		var y3 = ~y2 + 1;
		WriteBits(y3);           // 00000000_00000000_00000000_00001010
	}

	// GetHashCode/Equals case

	// Equals must be overrided, when GetHashCode overrided
	// (example is insane, but very simple)
	
	// Equals for classes use unique instance id

	class WrongEqClass {
		public string Name;

		public override int GetHashCode() {
			return string.Empty.GetHashCode();
		}
	}

	// bucket selected by hash_code % capacity
	// buckets: [ 1, -1, -1 ] // All items placed to one bucket, collision happens
	// entries: [ { key = {"1"}, value = "123", next = -1 }, { key = {"2"}, value = "321", next = 0 }, { key = null, value = null, next = -1 } ]
	// [""] => {"1"} => {"2"} => ...
	// But when we check contains, we pass throught all elements and call Equals on it (until we found one or next = -1)

	void DictClassTest() {
		var dict = new Dictionary<WrongEqClass, string>();
		dict.Add(new WrongEqClass() { Name = "1" }, "123");
		dict.Add(new WrongEqClass() { Name = "2" }, "321");
		Debug.Log(dict.ContainsKey(new WrongEqClass() { Name = "1" }));
	}

	void HashSetClassTest() {
		var set = new HashSet<WrongEqClass>();
		set.Add(new WrongEqClass() { Name = "1" });
		set.Add(new WrongEqClass() { Name = "2" });
		Debug.Log(set.Contains(new WrongEqClass() { Name = "1" }));
	}

	// Equals for structs check it members (reflection can be used), so results differ

	struct WrongEqStruct {
		public string Name;

		public override int GetHashCode() {
			return string.Empty.GetHashCode();
		}
	}

	void DictStructTest() {
		var dict = new Dictionary<WrongEqStruct, string>();
		dict.Add(new WrongEqStruct() { Name = "1" }, "123");
		dict.Add(new WrongEqStruct() { Name = "2" }, "321");
		Debug.Log(dict.ContainsKey(new WrongEqStruct() { Name = "1" }));
	}

	void HashSetStructTest() {
		var set = new HashSet<WrongEqStruct>();
		set.Add(new WrongEqStruct() { Name = "1" });
		set.Add(new WrongEqStruct() { Name = "2" });
		Debug.Log(set.Contains(new WrongEqStruct() { Name = "1" }));
	}

	void Update() {
		DictClassTest();
		HashSetClassTest();
		DictStructTest();
		HashSetStructTest();
	}
}
