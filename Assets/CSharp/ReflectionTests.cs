using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CSharp {
	public static class ReflectionTests {
		public static Assembly GetAssemblyFor(object obj) {
			return obj.GetType().Assembly;
		}
	
		public static Tuple<string, List<string>> GetTypesFrom(object obj) {
			var assembly = GetAssemblyFor(obj);
			var rootName = assembly.FullName;
			var typeNames = new List<string>();
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
				typeNames.Add(childName);
			}
			return new Tuple<string, List<string>>(rootName, typeNames);
		}

		[Obsolete]
		public static string SomeValue = "myValue";
		
		public static void FindValueUpderAttribute() {
			var type = typeof(ReflectionTests);
			var fields = type.GetFields();
			Debug.Log("Fields: " + fields.Length);
			foreach ( var field in fields ) {
				var attrs = field.GetCustomAttributes(typeof(ObsoleteAttribute));
				if ( attrs.Any() ) {
					var value = field.GetValue(null);
					Debug.LogFormat("Field: '{0}' has given attribute, field value is '{1}'", field.Name, value);
				}
			}
		}
	}
}