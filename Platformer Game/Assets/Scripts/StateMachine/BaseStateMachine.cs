using UnityEngine;
using System;
using System.Collections.Generic;

public abstract class BaseStateMachine<StateEnum> : MonoBehaviour where StateEnum: Enum
{
    protected Dictionary<StateEnum, BaseState<StateEnum>> States = new Dictionary<StateEnum, BaseState<StateEnum>>();
    protected bool IsTransitioningState = false;
    public BaseState<StateEnum> CurrentState{get;protected set;}
    void Start(){
        CurrentState.EnterState();
    }
    void Update()
    {
        StateEnum nextStateKey = CurrentState.GetNextState();
        if (!IsTransitioningState && nextStateKey.Equals(CurrentState.StateKey))
        {
            CurrentState.UpdateState();
        } else if (!IsTransitioningState)
        {
            TransitionToState(nextStateKey);
        }
    }
    public void TransitionToState(StateEnum stateKey)
    {
        IsTransitioningState=true;
        CurrentState.ExitState();
        CurrentState = States[stateKey];
        CurrentState.EnterState();
        IsTransitioningState=false;
    }
}

