using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour {

	public float power = 5.0f;
	Rigidbody body;

	// Use this for initialization
	void Start () {
		body = GetComponent<Rigidbody> ();
	}
	

	void LateUpdate () {

		if(Input.GetKey(KeyCode.A)) {
			body.AddForce (Vector3.left * power);
		}
		if(Input.GetKey(KeyCode.W)) {
			body.AddForce (Vector3.forward * power);
		}
		if(Input.GetKey(KeyCode.S)) {
			body.AddForce (Vector3.back * power);
		}
		if(Input.GetKey(KeyCode.D)) {
			body.AddForce (Vector3.right * power);
		}
		if(Input.GetKeyDown(KeyCode.Space)) {
			Debug.LogError ("Jump!");
			body.AddForce (new Vector3(0f,10f,0f),ForceMode.Impulse);
		}

	}
}
