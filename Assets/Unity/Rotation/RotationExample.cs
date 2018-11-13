using UnityEngine;
using UnityEngine.Events;

public class RotationExample : MonoBehaviour {
	public float Speed = 10.0f;
	public Transform Target;
	public UnityEvent StartCallback;
	public UnityEvent UpdateCallback;

	void Start() {
		StartCallback.Invoke();
	}
	
	void Update () {
		UpdateCallback.Invoke();
	}

	public void RotateAroundY_EA() {
		transform.localEulerAngles += Vector3.up * Speed * Time.deltaTime;
	}

	public void RotateAroundY_Q() {
		transform.localRotation *= Quaternion.AngleAxis(Speed * Time.deltaTime, Vector3.up);
	}
}
