using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

public static class PackageManagerTest {
	static ListRequest _list;
	static RemoveRequest _remove;

	[MenuItem("Custom/PackageManager/List")]
	static void List() {
		EditorApplication.update = OnListUpdate;
		_list = Client.List();
	}

	static void OnListUpdate() { // List embedded packages
		Debug.Log($"Client.List: IsCompleted: {_list.IsCompleted}, Status: {_list.Status}, Error: {_list.Error}, Result: {_list.Result}");
		if (_list.IsCompleted) {
			EditorApplication.update = null;
			var count = 0;
			foreach ( var item in _list.Result ) {
				Debug.Log(
					$"[{count}] name: {item.name}, displayName: {item.displayName}, packageId: {item.packageId}, " +
					$"version: {item.version}, resolvedPath: {item.resolvedPath}, category: {item.category}, description: {item.description}");
				count++;
			}
			Debug.Log($"Packages: {count}");
		}
	}

	[MenuItem("Custom/PackageManager/Remove")]
	static void Remove() { // Can't remove embedded packages
		EditorApplication.update = OnRemoveUpdate;
		_remove = Client.Remove("com.unity.standardevents");
	}

	static void OnRemoveUpdate() {
		Debug.Log($"Client.Remove: IsCompleted: {_remove.IsCompleted}, Status: {_remove.Status}, Error.errorCode: {_remove.Error?.errorCode}, Error.message: {_remove.Error?.message}, PackageIdOrName: {_remove.PackageIdOrName}");
		if (_remove.IsCompleted) {
			EditorApplication.update = null;
		}
	}
}
