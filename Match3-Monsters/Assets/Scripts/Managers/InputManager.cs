using System.Collections;
using UnityEngine;

public enum InputMode { Move, Wait }

public class InputManager
{
    public InputMode InputMode { get; set; }
    public BlockManager blockManager { get; set; }
    public IControllerInput ControllerInput { get; set; }

    public InputManager(BlockManager blockManager)
    {
        this.blockManager = blockManager;
    }

    public IEnumerator HandleInput()
    {

        if (CheckInputUp())
        {
            blockManager.ConfirmMove();
            InputMode = InputMode.Wait;
        }
        else if (CheckInputDown()) blockManager.CurrentMonster = blockManager.Raycaster.GetMonsterAtPosition(GetPointerPos());
        else if (CheckInput() && blockManager.CurrentMonster != null) blockManager.DragSelectedBlocks();

        yield return new WaitForSeconds(0.001f);
    }

    public bool CheckInputUp() => ControllerInput.OnInputUp() 
        && InputMode == InputMode.Move;

    public bool CheckInputDown() => ControllerInput.OnInputDown() 
        && InputMode == InputMode.Move;

    public bool CheckInput() => ControllerInput.OnInput() 
        && InputMode == InputMode.Move;

    public Vector2 GetPointerPos() => ControllerInput.GetPointerPos();
}
