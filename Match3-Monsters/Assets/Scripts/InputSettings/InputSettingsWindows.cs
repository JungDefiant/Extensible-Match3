using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class InputSettingsWindows : IControllerInput
{
    public Vector2 GetPointerPos() => Input.mousePosition;

    public bool OnInput() => Input.GetMouseButton(0);

    public bool OnInputDown() => Input.GetMouseButtonDown(0);

    public bool OnInputUp() => Input.GetMouseButtonUp(0);
}
