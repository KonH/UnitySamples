using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rotorz.Games.Reflection;

namespace Namespace0 {
	public class Comp0 : MonoBehaviour {
	}
	namespace Namespace1 {
		public class Comp1 : MonoBehaviour {
		}
		namespace Namespace2 {
			public class Comp2 : MonoBehaviour {
			}
		}
	}
}

public class TypeSelector : MonoBehaviour {

	[ClassExtends(typeof(Component))]
	public ClassTypeReference Reference;

	void Start() {
		var type = Reference.Type;
		gameObject.AddComponent(type);
	}
}
