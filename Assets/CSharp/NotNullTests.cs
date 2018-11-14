using System;
using UnityEngine;
using UnityEngine.Assertions;

public class NotNullTests : MonoBehaviour {

	class SomeClass {
		public string Str { get; }

		public SomeClass(string value) {
			Str = value;
		}
	}
	
	void Start () {
		// Without exceptions it doesn't make sense
		Assert.raiseExceptions = true;
		
		// Works as usual
		var validObj1 = new NotNull<SomeClass>(new SomeClass("str"));
		SomeMethod(validObj1);
		
		// Alternative syntax
		var validObj2 = new SomeClass("str").AsNotNull();
		SomeMethod(validObj2);
		
		// In this case we get exception when creating instance
		try {
			NotNull<SomeClass> invalidObj = new NotNull<SomeClass>(null);
			SomeMethod(invalidObj);
		} catch ( Exception e ) {
			Debug.LogError(e);
		}

		// In this case we get exception when getting value
		try {
			NotNull<SomeClass> invalidObj = default(NotNull<SomeClass>);
			SomeMethod(invalidObj);
		} catch ( Exception e ) {
			Debug.LogError(e);
		}
	}

	void SomeMethod(NotNull<SomeClass> instanceWrapper) {
		SomeMethodInternal(instanceWrapper);
	}

	void SomeMethodInternal(SomeClass instance) {
		Debug.LogFormat("SomeMethod called with '{0}'", (instance != null) ? instance.Str : "null");
	}
}
