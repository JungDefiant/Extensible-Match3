using UnityEngine;

public enum InputMode { Move, Wait }

public class InputManager
{
    public InputMode InputMode { get; set; }
    public IControllerInput ControllerInput { get; private set; }

    public InputManager()
    {
        #if UNITY_EDITOR
            ControllerInput = new InputSettingsWindows();
        #endif

        #if UNITY_ANDROID
            ControllerInput = new InputSettingsAndroid();
        #endif
    }

    public bool CheckInputUp() => ControllerInput.OnInputUp() 
        && InputMode == InputMode.Move;

    public bool CheckInputDown() => ControllerInput.OnInputDown() 
        && InputMode == InputMode.Move;

    public bool CheckInput() => ControllerInput.OnInput() 
        && InputMode == InputMode.Move;

    public Vector2 GetPointerPos() => ControllerInput.GetPointerPos();
}
