using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Node : ScriptableObject
{
	public enum State
	{
		Running,
		Success,
		Failure
	}

    [HideInInspector] public Blackboard blackboard;
	[TextArea] public string description;

	[HideInInspector] public State state = State.Running;
    [HideInInspector] public bool started = false;
    [HideInInspector] public string guid;
    [HideInInspector] public Vector2 position;

	public State Update()
	{
		if (!started)
		{
			OnStart();
			started = true;
		}

		state = OnUpdate();

		if (state != State.Running)
		{
			OnStop();
			started = false;
		}

		return state;
	}

	public virtual Node Clone()
	{
		return Instantiate(this);
	}

	protected abstract void OnStart();
	protected abstract void OnStop();
	protected abstract State OnUpdate();
}
