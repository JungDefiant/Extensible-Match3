using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class RaycastGetter
{
    private GraphicRaycaster raycaster;

    public RaycastGetter(GraphicRaycaster raycaster)
    {
        this.raycaster = raycaster;
    }

    public MonsterBlock GetMonsterAtPosition(Vector2 position)
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = position;

        List<RaycastResult> raycasts = new List<RaycastResult>();

        raycaster.Raycast(pointerEventData, raycasts);

        if (raycasts.Count < 1) return null;

        return raycasts.Where(x => x.gameObject.GetComponent<MonsterBlock>() != null)
                       .Select(x => x.gameObject.GetComponent<MonsterBlock>())
                       .First();
    }

    public MonsterBlock[] GetMonstersAtPosition(Vector2 position)
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = position;

        List<RaycastResult> raycasts = new List<RaycastResult>();

        raycaster.Raycast(pointerEventData, raycasts);

        if (raycasts.Count < 1) return null;

        return raycasts.Where(x => x.gameObject.GetComponent<MonsterBlock>() != null)
                       .Select(x => x.gameObject.GetComponent<MonsterBlock>())
                       .ToArray();
    }
}
