using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ListTest : MonoBehaviour {

	void TestReallocate() {
		var log = "ListTest:\n";
		log += "Reallocate:\n";
		var list = new List<int>();
		var prevCapacity = 0;
		var failed = false;
		for ( var i = 0; i < int.MaxValue; i++ ) {
			try {
				list.Add(0);
			} catch ( OutOfMemoryException ) {
				failed = true;
			}
			if ( list.Capacity > prevCapacity ) {
				// coeff ~ 0.5
				log += $"Count: {list.Count}, capacity: {list.Capacity}, coeff: {(float)list.Count / list.Capacity}\n";
				prevCapacity = list.Capacity;
			}
			if ( failed ) {
				log += $"OOM on trying to add {i} element\n"; // 536 870 912
				break;
			}
		}
		Debug.Log(log);
	}

	void TestClearInside() {
		var log = "TestClearInside:\n";

		log += "Initial:\n";
		var list = new List<int>();
		var type = typeof(List<int>);
		log = GetListInfo(list, log);

		list.AddRange(Enumerable.Range(0, 10000));
		log += "\nAfter insert:\n";
		log = GetListInfo(list, log);

		list.Clear();
		log += "\nAfter clear:\n";
		log = GetListInfo(list, log);

		list.Capacity = 0;
		log += "\nAfter Capacity = 0\n";
		log = GetListInfo(list, log);

		var itemsFieldValue = type.GetField("_items", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(list);
		var emptyFieldValue = type.GetField("_items", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(new List<int>());
		log += $"\nIsEmptyArrayReused? {itemsFieldValue == emptyFieldValue}";

		Debug.Log(log);
	}

	string GetListInfo<T>(List<T> list, string str) {
		var type = typeof(List<T>);
		str += $"Count: {list.Count}, Capacity: {list.Capacity}\n";
		foreach ( var field in type.GetRuntimeFields() ) {
			str += $"\t{field.Name}:{field.FieldType.Name} = {field.GetValue(list)}\n";
		}
		return str;
	}
}
