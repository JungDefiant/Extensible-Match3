using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text;
using System.Threading.Tasks;

public interface IControllerInput
{
    Vector2 GetPointerPos();
    bool OnInputUp();
    bool OnInputDown();
    bool OnInput();
}
