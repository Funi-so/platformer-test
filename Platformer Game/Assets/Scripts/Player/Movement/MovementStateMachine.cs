using System;
using UnityEngine;

public class MovementStateMachine : BaseStateMachine<MovementStateMachine.MovementStates>
{
    public enum MovementStates
    {
        Grounded,
        Running,
        Turnaround,
        Jumping,
        Airborne,
    }

    void Awake()
    {
        InitializeStates();
        MovementContext.noor = Noor.GetInstance();
        MovementContext.jumpBuffer = false;
        MovementContext.inputVector = Vector2.zero;
        MovementContext.groundNormal = Vector3.zero;
        MovementContext.camera = Camera.main;

        CurrentState = States[MovementStates.Grounded];
    }

    private void InitializeStates()
    {
        States.Add(MovementStates.Grounded, new GroundedMovState(MovementStates.Grounded));
        States.Add(MovementStates.Running, new RunningMovState(MovementStates.Running));
        States.Add(MovementStates.Turnaround, new TurnaroundMovState(MovementStates.Turnaround));
        States.Add(MovementStates.Jumping, new JumpingMovState(MovementStates.Jumping));
        States.Add(MovementStates.Airborne, new AirborneMovState(MovementStates.Airborne));
    }
}
