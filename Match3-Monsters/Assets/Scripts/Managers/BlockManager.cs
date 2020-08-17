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
    const float offboardThresholdFactor = 0.7f;

    public TileSlider Slider { get; set; }
    public MonsterBlock CurrentMonster { get; set; }
    public RaycastGetter Raycaster { get; set; }

    private Transform boardParent;
    private BoardManager boardManager;
    private Vector2 dragNormal = Vector2.zero;

    public BlockManager(BoardManager boardManager, GraphicRaycaster raycaster, TileSlider slider, Transform boardParent)
    {
        this.boardManager = boardManager;
        this.boardParent = boardParent;

        Slider = slider;

        Raycaster = new RaycastGetter(raycaster);
    }
    
    public IEnumerator CombineMonsters(List<Tile[]> allMatches)
    {
        foreach (var match in allMatches)
        {
            MonsterBlock[] allMonsters = new MonsterBlock[match.Length];
            Tile middleTile = match[match.Length / 2];
            bool allReachedDest = false;

            for (int i = 0; i < match.Length; i++)
            {
                Debug.Log("Next Monster: " + match[i].Monster);
                Debug.Log("Middle Tile: " + middleTile);
                allMonsters[i] = match[i].Monster;
                match[i].Monster = null;
                allMonsters[i].SetTileCoordinates(middleTile.Coordinates);
                allMonsters[i].MoveBlockToTileCoord(boardManager);
            }

            while (!allReachedDest)
            {
                yield return new WaitForSeconds(0.01f);

                allReachedDest = true;

                foreach (var monster in allMonsters)
                {
                    if (!monster.HasReachedDest(middleTile.Coordinates))
                    {
                        allReachedDest = false;
                        break;
                    }
                }

                Debug.Log("Reached Dest? " + allReachedDest);
            }

            foreach(var monster in allMonsters)
            {
                middleTile.Monster = null;
                UnityEngine.Object.Destroy(monster.gameObject);
                Debug.Log("Combined!");
            }
        }

        allMatches.Clear();
    }

    public void DragSelectedBlocks()
    {
        if (dragNormal != Vector2.zero)
        {
            MoveSlider();
            CheckIfOffBoard(dragNormal);
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

        foreach (var block in monsterBlocks)
        {
            block.StopAllCoroutines();
            block.SetPosition(BoardManager.VectorIntToCoords(block.TileCoordinates));
            block.transform.SetParent(Slider.transform);
        }

        Slider.transform.localPosition = Vector2.zero;
    }

    private void MoveSlider()
    {
        Vector2 moveVector = dragNormal.x > 0 ?
            Slider.Speed * dragNormal * Input.GetAxis("Mouse X") :
            Slider.Speed * dragNormal * Input.GetAxis("Mouse Y");

        Slider.transform.Translate(moveVector, Slider.transform.parent);
    }

    public void ConfirmMove()
    {
        dragNormal = Vector2.zero;

        while (Slider.transform.childCount > 0)
        {
            foreach (Transform child in Slider.transform)
            {
                MonsterBlock monster = child.GetComponent<MonsterBlock>();
                monster.SetTileCoordinates(child.localPosition);
                child.SetParent(boardParent);
                monster.MoveBlockToTileCoord(boardManager);
            }
        }
    }

    private void ResetSliderPosition(Vector2 normal)
    {
        float xThreshold = (BoardManager.boardStep * offboardThresholdFactor * normal.x) - (BoardManager.boardStep * normal.x);
        float yThreshold = (BoardManager.boardStep * offboardThresholdFactor * normal.y) - (BoardManager.boardStep  * normal.y);

        Debug.Log($"{xThreshold} , {yThreshold}");

        Slider.transform.localPosition = new Vector2(xThreshold, yThreshold);
    }
    
    private void CheckIfOffBoard(Vector2 direction)
    {
        float xThreshold = BoardManager.boardStep * offboardThresholdFactor;
        float yThreshold = BoardManager.boardStep * offboardThresholdFactor;

        float xPos = Slider.transform.localPosition.x;
        float yPos = Slider.transform.localPosition.y;

        if (xPos > xThreshold || yPos > yThreshold)
        {
            RotateMonstersOnSlider(true, direction);
            Debug.Log("Upper threshold");
        }
        else if (xPos < -xThreshold || yPos < -yThreshold)
        {
            RotateMonstersOnSlider(false, direction);
            Debug.Log("Lower threshold");
        }
    }

    private void RotateMonstersOnSlider(bool isStart, Vector2 normal)
    {
        normal.Normalize();

        Transform parent = Slider.transform;
        int xStep = Mathf.RoundToInt(normal.x);
        int yStep = Mathf.RoundToInt(normal.y);

        if (isStart) parent.GetChild(parent.childCount - 1).SetAsFirstSibling();
        else parent.GetChild(0).SetAsLastSibling();

        float childXPos;
        float childYPos;

        for (int i = 0; i < parent.childCount * normal.x; i++)
        {
            Transform child = parent.GetChild(i);

            Debug.Log(child.GetSiblingIndex());

            childXPos = (BoardManager.boardStep * i) + BoardManager.boardRoot.x;

            child.localPosition = new Vector2(childXPos, child.localPosition.y);
        }

        for (int j = 0; j < parent.childCount * normal.y; j++)
        {
            Transform child = parent.GetChild(j);

            Debug.Log(child.GetSiblingIndex());

            childYPos = (BoardManager.boardStep * j) + BoardManager.boardRoot.y;

            child.localPosition = new Vector2(child.localPosition.x, childYPos);
        }

        ResetSliderPosition(isStart ? normal : normal *= -1);
    }
}
