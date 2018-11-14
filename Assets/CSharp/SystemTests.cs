using UnityEngine;
using System.Linq;

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
}
