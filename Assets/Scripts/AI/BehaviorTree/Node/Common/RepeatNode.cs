using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeatNode : DecoratorNode
{
	public enum RepeatType
	{
		Infinite,
		Count,
		Duration,
	}

    [HideInInspector][SerializeField] private RepeatType _repeatType = RepeatType.Infinite;
    [HideInInspector][SerializeField] private int _count = 1;
    [HideInInspector][SerializeField] private float _duration = 1f;

	private int _successCount = 0;
	private float _startTime = 0f;

    protected override void OnStart()
	{
		_successCount = 0;
        _startTime = Time.time;
	}

	protected override void OnStop()
	{
	}

	protected override State OnUpdate()
	{
		switch (_repeatType)
		{
			case RepeatType.Infinite:
				return OnUpdateRepeatInfinite();
			case RepeatType.Count:
				return OnUpdateRepeatCount();
            case RepeatType.Duration:
				return OnUpdateRepeatDuration();
			default:
				break;
        }

		return State.Running;
	}

	private State OnUpdateRepeatInfinite()
	{
		child.Update();
        return State.Running;
    }

	private State OnUpdateRepeatCount()
	{
		State state = child.Update();

        switch (state)
		{
			case State.Success:
			{
                _successCount++;

                if (_successCount >= _count)
                    return State.Success;
				else
					return State.Running;
            }
			default:
				break;
		}
        return state;
    }

	private State OnUpdateRepeatDuration()
	{
        State state = child.Update();

        if (Time.time - _startTime >= _duration)
            return State.Success;

		switch (state)
		{
			case State.Success:
				return State.Running;
			default:
				break;
		}

        return state;
    }
}
