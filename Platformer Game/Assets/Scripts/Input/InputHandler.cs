using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : ScriptableObject, InputActions.IInGameActions
{
    private InputActions _inputActions;
    private InputActions.InGameActions _inGameActions;

    void OnEnable()
    {
        Debug.Log("InputEnable");
        if(_inputActions == null)
            _inputActions = new InputActions();
        _inGameActions = _inputActions.InGame;
        _inGameActions.SetCallbacks(this);
        _inGameActions.Enable();
    }

    void OnDisable()
    {
        Debug.Log("InputDisable");
        _inGameActions.Disable();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    public void OnLookVector(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    public void OnMoveVector(InputAction.CallbackContext context)
    {
        Noor noorInstance = Noor.GetInstance();
        if (context.canceled)
        {
            noorInstance.SetMoveInput(Vector2.zero);
            Debug.Log("stopd");
            return;
        }
        Debug.Log("walkin");
        noorInstance.SetMoveInput(context.ReadValue<Vector2>());
    }

}
