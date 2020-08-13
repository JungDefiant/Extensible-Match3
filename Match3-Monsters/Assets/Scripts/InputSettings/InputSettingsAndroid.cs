using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class InputSettingsAndroid : IControllerInput
{
    public Vector2 GetPointerPos() => Input.GetTouch(0).position;

    public bool OnInput() => Input.touchCount > 0 ? Input.GetTouch(0).phase == TouchPhase.Moved : false;

    public bool OnInputDown() => Input.touchCount > 0 ? Input.GetTouch(0).phase == TouchPhase.Ended : false;

    public bool OnInputUp() => Input.touchCount > 0 ? Input.GetTouch(0).phase == TouchPhase.Began : false;
}
