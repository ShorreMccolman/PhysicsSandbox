using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring : MonoBehaviour {

	public const float gravity = -9.81f;

	public Transform pivotPoint;
	public Transform bob;

	public IntegrationMethod integrationMethod;
	public bool fixedUpdate;
	public float springConst;

	float rest;
	State curState;

	void Start()
	{
		rest = Vector3.Distance (bob.position, pivotPoint.position);

		curState = new State ();
		curState.position = bob.position;
		curState.velocity = Vector3.zero;
	}

	void Update()
	{
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
		bob.position = curState.position;
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
		bob.position = curState.position;
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

		Vector3 direction = springConst*(pivotPoint.position - new Vector3(0f,rest,0f) - state.position);
		forces.Add (direction);

		Vector3 acceleration = Vector3.zero;
		foreach(Vector3 force in forces) {
			acceleration += force;
		}
		return acceleration;
	}
}
