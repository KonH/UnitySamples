using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using System.Threading;
using CSharp;

public class SystemTests : MonoBehaviour {
	
	[ContextMenu("Generate Childs")]
	void GenerateChilds() {
		FindFileFor(new object());
		GenerateTypesFrom(new object());
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
	
	[ContextMenu("Other Tests")]
	void OtherTests() {
		StringTest();
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
	
	static void TupleTest() {
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
	
	// FormattableString
	
	static void FormattableStringTest() {
		var               argument = "arg";
		FormattableString s        = $"http://google.com/{argument}";
		ExploreFormattableStr(s);
	}

	static void ExploreFormattableStr(FormattableString s) {
		Debug.Log(s.ToString());
		Debug.Log(s.Format);
		Debug.Log(s.ArgumentCount);
		Debug.Log(s.GetArgument(0));
	}

	// GC
	// GC.TryStartNoGCRegion not implemented
	// RegisterForFullGCNotification not implemented

	class DisposableClass : IDisposable {
		~DisposableClass() {
			Debug.Log("Clear data in finalizer: " + Thread.CurrentThread.ManagedThreadId);
			// GameObject go = new GameObject("go"); // Internal_CreateGameObject can only be called from the main thread.
		}
		
		public void Dispose() {
			Debug.Log("Clear data in dispose call: " + Thread.CurrentThread.ManagedThreadId);
			GC.SuppressFinalize(this);
		}
	}
	
	static void GCTest() {
		Debug.Log($"GC.MaxGeneration: {GC.MaxGeneration}");
		
		// Disposing (deterministic) vs finalization (non-deterministic with possible multi-threaded issues)
		Debug.Log("Running tests in: " + Thread.CurrentThread.ManagedThreadId);
		using ( var obj1 = new DisposableClass() ) {
			Debug.Log(obj1);
		}
		CreateOtherObject();
		GC.Collect();
		GC.WaitForPendingFinalizers();
	}

	static void CreateOtherObject() {
		var obj2 = new DisposableClass();
		Debug.Log(obj2);
	}
	
	// Lazy<T>

	class LazyClass {
		public LazyClass() {
			Debug.Log("Create instance");
		}

		public LazyClass(string str) {
			Debug.Log("Create instance with " + str);
		}
	}
	
	static Lazy<LazyClass> LazyInstance1 = new Lazy<LazyClass>();
	static Lazy<LazyClass> LazyInstance2 = new Lazy<LazyClass>(() => new LazyClass("123"));
	
	static void LazyTest() {
		Debug.Log(LazyInstance1.IsValueCreated);
		Debug.Log(LazyInstance1.Value);
		Debug.Log(LazyInstance2.Value);
	}
	
	// ArraySegment<T>
	// struct with unclean interface compared to Span<T>

	static void ArraySegmentTest() {
		var array = new int[] { 0, 1, 2, 3, 4 };
		var segment = new ArraySegment<int>(array, 2, 1);
		Debug.Log(segment.Array[segment.Offset + 0]);
	}
	
	// Void: error CS0673: System.Void cannot be used from C#. Consider using `void'

	static void NullableTest() {
		Debug.Log(Nullable.GetUnderlyingType(typeof(int?))); // int32
		Debug.Log(typeof(int?) == typeof(Nullable<int>)); // true
		int? value = null;
		Debug.Log(value.HasValue);
		Debug.Log(value.Value); // InvalidOperationException
	}
	
	// Activator

	class PublicClass {
	}
	
	class PrivateClass {
		private PrivateClass() {}
	}

	static void ActivatorTest() {
		Debug.Log(Activator.CreateInstance<PublicClass>());
		Debug.Log(Activator.CreateInstance(typeof(PublicClass)));
		// var pr = new PrivateClass(); // can't
		// Debug.Log(Activator.CreateInstance<PrivateClass>()); // can't: MissingMethodException
		Debug.Log(typeof(PublicClass).Assembly);
		Debug.Log(typeof(PublicClass).FullName);
		var handle = Activator.CreateInstance("CSharp", "SystemTests+PublicClass");
		Debug.Log(handle.Unwrap()); // instance itself
		var lease = handle.InitializeLifetimeService() as ILease; // using for .NET Remoting
		Debug.Log(lease.CurrentState); // Initial
		Debug.Log(handle.GetLifetimeService()); // null
		//Debug.Log(handle.CreateObjRef());
	}

	// BitConverter
	
	static void BitConverterTest() {
		Debug.Log(BitConverter.IsLittleEndian);
		WriteBytes(false, true);
		WriteBytes(char.MinValue, char.MaxValue);
		WriteBytes(short.MinValue, short.MaxValue);
		WriteBytes(ushort.MinValue, ushort.MaxValue);
		WriteBytes(int.MinValue, int.MaxValue);
		WriteBytes(uint.MinValue, uint.MaxValue);
		WriteBytes(long.MinValue, long.MaxValue);
		WriteBytes(ulong.MinValue, ulong.MaxValue);
		WriteBytes(float.MinValue, float.MaxValue);
		WriteBytes(double.MinValue, double.MaxValue);
	}

	// Can't use generics here :(
	static void WriteBytes(bool t1, bool t2) => WriteBytes(t1, t2, BitConverter.GetBytes(t1), BitConverter.GetBytes(t2));
	static void WriteBytes(char t1, char t2) => WriteBytes(t1, t2, BitConverter.GetBytes(t1), BitConverter.GetBytes(t2));
	static void WriteBytes(short t1, short t2) => WriteBytes(t1, t2, BitConverter.GetBytes(t1), BitConverter.GetBytes(t2));
	static void WriteBytes(ushort t1, ushort t2) => WriteBytes(t1, t2, BitConverter.GetBytes(t1), BitConverter.GetBytes(t2));
	static void WriteBytes(int t1, int t2) => WriteBytes(t1, t2, BitConverter.GetBytes(t1), BitConverter.GetBytes(t2));
	static void WriteBytes(uint t1, uint t2) => WriteBytes(t1, t2, BitConverter.GetBytes(t1), BitConverter.GetBytes(t2));
	static void WriteBytes(long t1, long t2) => WriteBytes(t1, t2, BitConverter.GetBytes(t1), BitConverter.GetBytes(t2));
	static void WriteBytes(ulong t1, ulong t2) => WriteBytes(t1, t2, BitConverter.GetBytes(t1), BitConverter.GetBytes(t2));
	static void WriteBytes(double t1, double t2) => WriteBytes(t1, t2, BitConverter.GetBytes(t1), BitConverter.GetBytes(t2));
	static void WriteBytes(float t1, float t2) => WriteBytes(t1, t2, BitConverter.GetBytes(t1), BitConverter.GetBytes(t2));

	static void WriteBytes<T>(T min, T max, byte[] b1, byte[] b2) {
		Debug.Log($"[{typeof(T)}] {min}-{max} => {GetBytesPart(b1)} - {GetBytesPart(b2)}");
	}
	
	// Because of '\0'
	static void WriteBytes(char min, char max, byte[] b1, byte[] b2) {
		Debug.Log($"[{typeof(char)}] {(int)min}-{(int)max} => {GetBytesPart(b1)} - {GetBytesPart(b2)}");
	}
	
	static string GetBytesPart(byte[] bytes) {
		var str = $"[{bytes.Length}]";
		for ( var i = 0; i < bytes.Length; i++ ) {
			str += $" {bytes[i]} ";
		}
		return str;
	}
	
	// String

	static void StringTest() {
		var str1 = "12345";
		var str2 = "123\045";
		Debug.Log(str1.Length); // 5
		Debug.Log(str1); // 12345
		Debug.Log(str2.Length); // 6
		Debug.Log(str2); // 123
		
		var str3 = new string(new char[] { '1', '\0', '2' });
		Debug.Log(str3.Length); // 3
		Debug.Log(str3); // 1

		unsafe {
			var array = new char[] {'1', '\0', '2'};
			fixed ( char* p = array ) {
				var str4 = new string(p);
				Debug.Log(str4.Length); // 1
				Debug.Log(str4);        // 1
			}
		}

		// Continue
	}
}
