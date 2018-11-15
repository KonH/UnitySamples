using System;
using UnityEngine;
using System.Linq;
using CSharp;

public class SystemTests : MonoBehaviour {
	
	[ContextMenu("Generate Childs")]
	void GenerateChilds() {
		FindFileFor(new object());
		GenerateTypesFrom(new object());
	}

	[ContextMenu("Other Tests")]
	void OtherTests() {
		TupleTests();
	}
	
	void GenerateTypesFrom(object obj) {
		var tuple = ReflectionTests.GetTypesFrom(obj);
		var rootName = tuple.Item1;
		var typeNames = tuple.Item2;
		var root = new GameObject(rootName);
		root.transform.SetParent(transform);
		foreach ( var childName in typeNames ) {
			var child = new GameObject(childName);
			child.transform.SetParent(root.transform);
		}
	}

	void FindFileFor(object obj) {
		var assembly = ReflectionTests.GetAssemblyFor(obj);
		Debug.Log(assembly.Location);
	}

	void Start() {
	}

	// Notes:
	
	// Microsoft.Win32.Registry:
	// - Can be used not only on Windows, at least in Editor/MacOS, it stored in ~/.mono/registry inside separated dirs
	// - Data will be saved in values.xml files
	// - Format is <values><value name="value" type="string">value</value></values>
	// - Anyway, I don't want to explore deeply
	
	// Microsoft.Win32.SafeHandles
	// - Can be used as safe wrappers for native resources
	// - But this low-level stuff is usually unrelated
	
	// Span<T> and Memory<T> can be accessed by reflection, but useless in that case
	// I'm don't sure how exactly it was hidden
	
	static void SpanTest() {
		var     array       = new int[] {0, 1, 2, 3};
		var     assembly    = new object().GetType().Assembly;
		var     commonType  = assembly.ExportedTypes.First(t => t.FullName.Contains("System.Span"));
		var     genericType = commonType.MakeGenericType(typeof(int));
		dynamic memory      = Activator.CreateInstance(genericType, /* data: */array, /* start: */1, /* len: */2);

		Debug.Log(memory);
		Debug.Log(memory.Length); // 2

		// Debug.Log(memory[0]); // ArgumentException: Property cannot have a managed pointer type.

		var contents = memory.ToArray();
		Debug.Log(contents[0]); // 1
		Debug.Log(contents[1]); // 2

		var part = memory.Slice(0, 1);
		Debug.Log(part);
		Debug.Log(part.ToArray()[0]); // 1
	}

	// Tuple
	
	static void TupleTests() {
		// GC allocations
		
		// Old style
		var tuple = Calculate1();
		Debug.Log(tuple);
		string str;
		int value;
		tuple.Deconstruct(out str, out value);
		Debug.Log(str + value.ToString());

		// C# 7.0 style, not working yet
		/*
		var (str2, value2) = Calculate2();
		Debug.Log(str2 + value2.ToString());
		var tuple2 = (1, 2).ToTuple();
		*/

		// No GC allocations

		var valueTuple = new ValueTuple<int, int>(30, 60);
		Debug.Log(valueTuple);
	}

	static Tuple<string, int> Calculate1() {
		return new Tuple<string, int>("str", 42);
	}
	
	/*
	static (string, int) Calculate2() {
		return ("str", 42);
	}
	*/
}
