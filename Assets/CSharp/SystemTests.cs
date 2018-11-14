using System;
using UnityEngine;
using System.Linq;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;

public class SystemTests : MonoBehaviour {
	
	[ContextMenu("Generate Childs")]
	void GenerateChilds() {
		GenerateTypesFrom(new object());
	}

	void GenerateTypesFrom(object obj) {
		var assembly = obj.GetType().Assembly;
		var root = new GameObject(assembly.FullName);
		root.transform.SetParent(transform);
		var types = assembly.ExportedTypes.Where(t => t.IsPublic);
		foreach ( var type in types ) {
			var typeName = "unknown";
			if ( type.IsInterface ) {
				typeName = "interface";
			} else if ( type.IsClass ) {
				typeName = "class";
			} else if ( type.IsEnum ) {
				typeName = "enum";
			} else if ( type.IsValueType ) {
				typeName = "struct";
			} else if ( type.IsPrimitive ) {
				typeName = "primitive";
			} else {
				Debug.LogError(type.FullName);
			}
			var childName = string.Format("{0} [{1}]", type.FullName, typeName);
			var child     = new GameObject(childName);
			child.transform.SetParent(root.transform);
		}
	}

	void Start() {
		SpanTest();
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
}
