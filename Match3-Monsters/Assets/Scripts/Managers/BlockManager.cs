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
    public MonsterBlock CurrentMonster { get; set; }

    private Transform slider;
    private Transform boardParent;
    private BoardManager boardManager;
    private InputManager inputManager;
    private GraphicRaycaster raycaster;
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
    
    public IEnumerator CombineMonsters(List<Tile[]> allMatches)
    {
        foreach (var match in allMatches)
        {
            Tile middleTile = match[0];
            bool allReachedDest = false;

            for (int i = 1; i < match.Length; i++)
            {
                Debug.Log(match[i]);
                MonsterBlock nextMonster = match[i].Monster;
                match[i].Monster = null;
                nextMonster.SetTileCoordinates(middleTile.Coordinates);
                nextMonster.MoveBlockToTileCoord(boardManager);
            }

            while (!allReachedDest)
            {
                if (GetMonstersAtPosition(middleTile.Coordinates).Length == match.Length)
                {
                    allReachedDest = true;
                }
                else yield return new WaitForSeconds(0.1f);
            }

            foreach(var monster in GetMonstersAtPosition(middleTile.Coordinates))
            {
                monster.gameObject.SetActive(false);
                middleTile.Monster = null;
            }
        }
    }

    public MonsterBlock GetMonsterAtPosition(Vector2 position)
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = position;

        List<RaycastResult> raycasts = new List<RaycastResult>();

        raycaster.Raycast(pointerEventData, raycasts);

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

        return raycasts.Where(x => x.gameObject.GetComponent<MonsterBlock>() != null)
                       .Select(x => x.gameObject.GetComponent<MonsterBlock>())
                       .ToArray();
    }

    public void DragSelectedBlocks()
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
        MonsterBlock[] monsterBlocks = boardManager.GetTilesInRowOrColumn(CurrentMonster.transform.localPosition, dragNormal)
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

    public bool ConfirmMove()
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
