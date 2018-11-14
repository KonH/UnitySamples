using UnityEngine.Assertions;

public struct NotNull<T> where T : class {
	public T Value {
		get {
			Assert.IsNotNull(_value);
			return _value;
		}
	}

	readonly T _value;
	
	public NotNull(T value) {
		Assert.IsNotNull(value);
		_value = value;
	}
	
	public static implicit operator T(NotNull<T> wrapper) {
		return wrapper.Value;
	}
}

public static class NotNullHelper {
	public static NotNull<T> AsNotNull<T>(this T instance) where T : class {
		return new NotNull<T>(instance);
	}
}
