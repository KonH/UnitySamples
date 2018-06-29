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

	void Start() {
		var a = new SplitClass();
		a.PublicMethod();
	}
}
