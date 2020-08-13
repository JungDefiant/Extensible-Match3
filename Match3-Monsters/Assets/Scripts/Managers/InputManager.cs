using System.Collections;
using UnityEngine;

public enum InputMode { Move, Wait }

public class InputManager
{
    public InputMode InputMode { get; set; }
    public BlockManager blockManager { get; set; }
    public IControllerInput ControllerInput { get; private set; }

    public IEnumerator HandleInput()
    {
        if (CheckInputUp())
        {
            blockManager.ConfirmMove();
            InputMode = InputMode.Wait;
        }
        else if (CheckInputDown()) blockManager.CurrentMonster = blockManager.GetMonsterAtPosition(GetPointerPos());
        else if (CheckInput() && blockManager.CurrentMonster != null) blockManager.DragSelectedBlocks();

        yield return new WaitForSeconds(0.1f);
    }

    public bool CheckInputUp() => ControllerInput.OnInputUp() 
        && InputMode == InputMode.Move;

    public bool CheckInputDown() => ControllerInput.OnInputDown() 
        && InputMode == InputMode.Move;

    public bool CheckInput() => ControllerInput.OnInput() 
        && InputMode == InputMode.Move;

    public Vector2 GetPointerPos() => ControllerInput.GetPointerPos();
}
