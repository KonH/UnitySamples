using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class RotationExample : MonoBehaviour {
	public float      Speed = 10.0f;
	public Transform  Target;
	public UnityEvent StartCallback;
	public UnityEvent UpdateCallback;

	void Start() {
		StartCallback.Invoke();
	}

	void Update() {
		UpdateCallback.Invoke();
	}

	public void RotateAroundY_EA() {
		transform.localEulerAngles += Vector3.up * Speed * Time.deltaTime;
	}

	public void RotateAroundY_Q() {
		transform.localRotation *= Quaternion.AngleAxis(Speed * Time.deltaTime, Vector3.up);
	}

	public void RotateToTarget() {
		StartCoroutine(RotateToTargetCoro());
	}

	IEnumerator RotateToTargetCoro() {
		var targetRotation = Quaternion.LookRotation(Target.position - transform.position);
		var delta = 0.0f;
		while ( delta < 1.0f ) {
			transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, delta * Speed * Time.deltaTime);
			delta += Time.deltaTime;
			yield return null;
		}
	}
}
