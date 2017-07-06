using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBody : MonoBehaviour {

	public const float gravity = -9.81f;
	public const float motionThreshold = .4f;

	public float inelasticity = 0.75f;
	public float jumpSpeed = 5.0f;

	public Transform trans;
	public float speed;
	public Vector3 velocity;

	bool grounded = false;

	Vector3 curAccel;


	void Awake () {
		trans = this.transform;
	}

	void FixedUpdate () {
		bool input = false;

		List<Vector3> forces = new List<Vector3> ();
		forces.Add (new Vector3 (0f, gravity, 0f));

		if(grounded) {
			if(Input.GetKeyDown(KeyCode.Space)) {
				velocity += new Vector3 (0f, jumpSpeed, 0f);
			}
		}
		grounded = false;

		if(Input.GetKey(KeyCode.A)) {
			forces.Add (new Vector3 (-5.0f, 0f, 0f));
			input = true;
		}
		if(Input.GetKey(KeyCode.D)) {
			forces.Add (new Vector3 (5.0f, 0f, 0f));
			input = true;
		}
		if(Input.GetKey(KeyCode.W)) {
			forces.Add (new Vector3 (0f, 0f, 5.0f));
			input = true;
		}
		if(Input.GetKey(KeyCode.S)) {
			forces.Add (new Vector3 (0f, 0f, -5.0f));
			input = true;
		}

		Vector3 normalForce = Vector3.zero;

		RaycastHit hit;
		bool didHit = Physics.Raycast (transform.position,Vector3.down,out hit);
		if(didHit)
			normalForce = Vector3.Project (Vector3.up * -gravity, hit.normal);
		if (didHit && hit.distance <= 0.5f) {
			if (velocity.y <= motionThreshold) {
				if (Mathf.Abs (velocity.y) > motionThreshold) {
					velocity = new Vector3 (velocity.x, -velocity.y * inelasticity, velocity.z);
				} else {
					velocity = new Vector3 (velocity.x, 0f, velocity.z);
					forces.Add (normalForce);
					grounded = true;
				}
			}
		}
			
		Vector3 friction = normalForce.magnitude * .2f * -velocity.normalized;
		forces.Add (friction);

 		Vector3 acceleration = Vector3.zero;
		foreach(Vector3 force in forces) {
			acceleration += force;
		}
			
		trans.position += Time.fixedDeltaTime * (velocity + Time.fixedDeltaTime * acceleration * 0.5f);
		if(grounded) {
			trans.position = new Vector3 (trans.position.x, hit.point.y + 0.5f, trans.position.z);
		}

		velocity += Time.fixedDeltaTime * (acceleration + curAccel) * 0.5f;
		speed = velocity.magnitude;

		curAccel = acceleration;
	}
}
