using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : ScriptableObject, InputActions.IInGameActions
{
    private InputActions _inputActions;
    private InputActions.InGameActions _inGameActions;

    void OnEnable()
    {
        //Debug.Log("InputEnable");
        if(_inputActions == null)
            _inputActions = new InputActions();
        _inGameActions = _inputActions.InGame;
        _inGameActions.SetCallbacks(this);
        _inGameActions.Enable();
    }

    void OnDisable()
    {
        //Debug.Log("InputDisable");
        _inGameActions.Disable();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if(context.started)
            Noor.GetInstance().SetJumpBuffer();
    }

    public void OnLookVector(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    public void OnMoveVector(InputAction.CallbackContext context)
    {
        if (context.canceled)
        {
            Noor.GetInstance().SetMoveInput(Vector2.zero);
            //Debug.Log("stopd");
            return;
        }
        //Debug.Log("walkin");
        Noor.GetInstance().SetMoveInput(context.ReadValue<Vector2>());
    }

}
