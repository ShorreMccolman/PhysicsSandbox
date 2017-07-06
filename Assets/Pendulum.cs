using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct State
{
	public Vector3 position;
	public Vector3 velocity;
}

public struct Derivative
{
	public Vector3 dp;
	public Vector3 dv;
}

public enum IntegrationMethod
{
	Euler,
	Verlet,
	RK
}

public class Pendulum : MonoBehaviour {
	
	public const float gravity = -9.81f;

	public Transform pivotPoint;

	public IntegrationMethod integrationMethod;
	public bool fixedUpdate;
	public float power;

	State curState;
	float length;

	void Start()
	{
		length = Vector3.Distance (transform.position, pivotPoint.position);

		curState = new State ();
		curState.position = transform.position;
		curState.velocity = Vector3.zero;
	}

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Space)) {
			curState.velocity += Vector3.forward * power;
		}

		if (fixedUpdate)
			return;

		switch(integrationMethod) {
		case IntegrationMethod.Euler:
			curState = EulerIntegrate (curState, Time.time, Time.deltaTime);
			break;
		case IntegrationMethod.Verlet:
			curState = VerletIntegrate (curState, Time.time, Time.deltaTime);
			break;
		case IntegrationMethod.RK:
			curState = RKIntegrate (curState, Time.time, Time.deltaTime);
			break;
		}
		transform.position = curState.position;
	}

	void FixedUpdate () {
		if (!fixedUpdate)
			return;
		
		switch(integrationMethod) {
		case IntegrationMethod.Euler:
			curState = EulerIntegrate (curState, Time.fixedTime, Time.fixedDeltaTime);
			break;
		case IntegrationMethod.Verlet:
			curState = VerletIntegrate (curState, Time.fixedTime, Time.fixedDeltaTime);
			break;
		case IntegrationMethod.RK:
			curState = RKIntegrate (curState, Time.fixedTime, Time.fixedDeltaTime);
			break;
		}
		transform.position = curState.position;
	}

	public State EulerIntegrate(State state, float time, float dt)
	{
		state.position = state.position + state.velocity * dt;
		state.velocity = state.velocity + Acceleration (state, time) * dt;

		return state;
	}

	public State VerletIntegrate(State state, float time, float dt)
	{
		Derivative a, b;

		a = Evaluate (state, time, 0f, new Derivative ());
		b = Evaluate (state, time, dt, a);

		Vector3 dxdt = (a.dp + b.dp) * 0.5f;
		Vector3 dvdt = (a.dv + b.dv) * 0.5f;

		state.position = state.position + dxdt * dt;
		state.velocity = state.velocity + dvdt * dt;

		return state;
	}

	public State RKIntegrate(State state, float time, float dt)
	{
		Derivative a, b, c, d;

		a = Evaluate (state, time, 0f, new Derivative ());
		b = Evaluate (state, time, dt * 0.5f, a);
		c = Evaluate (state, time, dt * 0.5f, b);
		d = Evaluate (state, time, dt, c);

		Vector3 dxdt = 1f / 6f * (a.dp + 2f * (b.dp + c.dp) + d.dp);
		Vector3 dvdt = 1f / 6f * (a.dv + 2f * (b.dv + c.dv) + d.dv);

		state.position = state.position + dxdt * dt;
		state.velocity = state.velocity + dvdt * dt;

		return state;
	}

	public Derivative Evaluate(State theState, float time, float dt, Derivative der)
	{
		State state = new State ();
		state.position = theState.position + der.dp * dt;
		state.velocity = theState.velocity + der.dv * dt;

		Derivative output = new Derivative ();
		output.dp = state.velocity;
		output.dv = Acceleration (state, time + dt);
		return output;
	}

	public Vector3 Acceleration(State state, float time)
	{
		List<Vector3> forces = new List<Vector3> ();
		forces.Add (new Vector3 (0f, gravity, 0f));

		Vector3 direction = (pivotPoint.position - state.position).normalized;
		float val = -gravity * (pivotPoint.position.y - state.position.y) / length;
		val += state.velocity.sqrMagnitude / length;
		forces.Add (direction * val);

		Vector3 acceleration = Vector3.zero;
		foreach(Vector3 force in forces) {
			acceleration += force;
		}
		return acceleration;
	}
}
