using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BlockManager
{
    const float dragDeadzone = 0.05f;

    public float SliderSpeed { get; set; }

    private Transform slider;
    private Transform boardParent;
    private BoardManager boardManager;
    private InputManager inputManager;
    private GraphicRaycaster raycaster;
    private MonsterBlock currentMonster;
    private MonsterBlock[] currentMonsters;

    private Vector2 dragNormal = Vector2.zero;

    public BlockManager(BoardManager boardManager, InputManager inputManager, GraphicRaycaster raycaster, Transform slider, Transform boardParent)
    {
        this.boardManager = boardManager;
        this.inputManager = inputManager;
        this.raycaster = raycaster;
        this.slider = slider;
        this.boardParent = boardParent;

        int smallestBoardDimension = boardManager.boardWidth < boardManager.boardHeight ? boardManager.boardWidth : boardManager.boardHeight;
        currentMonsters = new MonsterBlock[smallestBoardDimension];
    }

    public IEnumerator HandleInput()
    {
        if (inputManager.CheckInputUp()) ConfirmMove();
        else if (inputManager.CheckInputDown()) currentMonster = GetMonsterAtPosition(); 
        else if (inputManager.CheckInput() && currentMonster != null) DragSelectedBlocks();

        yield return new WaitForSeconds(0.1f);
    }

    private MonsterBlock GetMonsterAtPosition()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = inputManager.GetPointerPos();

        List<RaycastResult> raycasts = new List<RaycastResult>();

        raycaster.Raycast(pointerEventData, raycasts);

        Debug.Log(raycasts[0].gameObject.name);

        return raycasts.Where(x => x.gameObject.GetComponent<MonsterBlock>() != null)
                       .Select(x => x.gameObject.GetComponent<MonsterBlock>())
                       .First();
    }

    private void DragSelectedBlocks()
    {
        if (dragNormal != Vector2.zero)
        {
            MoveSlider();
            CheckIfOffBoard();
        }
        else
        {
            ShiftDragState();
            AddMonstersToSlider();
        }
    }

    private void ShiftDragState()
    {
        if (Input.GetAxis("Mouse X") < -dragDeadzone || Input.GetAxis("Mouse X") > dragDeadzone) dragNormal = Vector2.right;
        else if (Input.GetAxis("Mouse Y") < -dragDeadzone || Input.GetAxis("Mouse Y") > dragDeadzone) dragNormal = Vector2.up;
        else dragNormal = Vector2.zero;
    }

    private void AddMonstersToSlider()
    {
        MonsterBlock[] monsterBlocks = boardManager.GetTilesInRowOrColumn(currentMonster.transform.localPosition, dragNormal)
                                                   .Where(x => x.Monster != null)
                                                   .Select(x => x.Monster)
                                                   .ToArray();

        foreach (var block in monsterBlocks) block.transform.SetParent(slider);
    }

    private void MoveSlider()
    {
        Vector2 moveVector = dragNormal.x > 0 ? 
            SliderSpeed * dragNormal * Input.GetAxis("Mouse X") :
            SliderSpeed * dragNormal * Input.GetAxis("Mouse Y");

        slider.Translate(moveVector, slider.parent);
    }

    private void CheckIfOffBoard()
    {

    }

    private bool ConfirmMove()
    {
        dragNormal = Vector2.zero;

        while(slider.childCount > 0)
        {
            foreach (Transform child in slider)
            {
                child.SetParent(boardParent);
                child.GetComponent<MonsterBlock>().MoveBlockToTileCoord(boardManager);
            }
        }

        return false;
    }
}
